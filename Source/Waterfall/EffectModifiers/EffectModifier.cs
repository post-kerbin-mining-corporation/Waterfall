using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public enum EffectModifierMode
  {
    REPLACE,
    ADD,
    SUBTRACT,
    MULTIPLY
  }

  /// <summary>
  ///   Base effect modifer class
  /// </summary>
  public abstract class EffectModifier
  {
    // This is the game name of the effect
    public string fxName = "";

    // This is the name of the controller that should be associated with the module
    [Persistent] public string controllerName = "";
    [Persistent] public string transformName = "";
    public string modifierTypeName = "";

    [Persistent] public bool useRandomness;
    [Persistent] public string randomnessController = nameof(RandomnessController);
    [Persistent] public float randomnessScale = 1f;

    public WaterfallEffect parentEffect;

    // The combination mode of this effect with the base
    public EffectModifierMode effectMode = EffectModifierMode.REPLACE;

    // The Transform that holds the thing the effect should modify
    protected List<Transform> xforms;
    protected float randomValue;

    public EffectIntegrator integrator;
    public WaterfallController Controller { get; private set; }
    protected WaterfallController randomController;
    public virtual bool ValidForIntegrator => true;

    public EffectModifier()
    {
      xforms = new();
    }

    public EffectModifier(ConfigNode node) : this()
    {
      Load(node);
    }

    public virtual void Load(ConfigNode node)
    {
      ConfigNode.LoadObjectFromConfig(this, node);
      node.TryGetValue("name", ref fxName);
      node.TryGetEnum("combinationType", ref effectMode, EffectModifierMode.REPLACE);
      Utils.Log($"[EffectModifier]: Loading modifier {fxName}", LogType.Modifiers);
    }

    /// <summary>
    ///   Initialize the effect
    /// </summary>
    public virtual void Init(WaterfallEffect effect)
    {
      parentEffect = effect;
      Controller = parentEffect.parentModule.AllControllersDict.TryGetValue(controllerName, out var controller) ? controller : null;
      if (Controller == null)
      {
        Utils.LogWarning($"[EffectModifier]: Controller {controllerName} not found for modifier {fxName} in effect {effect.name} in module {effect.parentModule.moduleID}");
      }
      else
      {
        Controller.referencingModifierCount++;
      }

      randomController = parentEffect.parentModule.AllControllersDict.TryGetValue(randomnessController, out controller) ? controller : null;

      if (randomController == null && useRandomness)
      {
        Utils.LogError($"[EffectModifier]: Randomness controller {randomnessController} not found for modifier {fxName} in effect {effect.name} in module {effect.parentModule.moduleID}");
      }
      else if (randomController != null)
      {
        randomController.referencingModifierCount++;
      }

      Utils.Log($"[EffectModifier]: Initializing modifier {fxName}", LogType.Modifiers);
      var roots = parentEffect.GetModelTransforms();
      xforms = new();
      foreach (var t in roots)
      {
        var t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError($"[EffectModifier]: Unable to find transform {transformName} on modifier {fxName}");
        }
        else
        {
          xforms.Add(t1);
        }
      }
    }

    protected abstract string ConfigNodeName { get; }

    public virtual ConfigNode Save()
    {
      var node = ConfigNode.CreateConfigFromObject(this);
      node.AddValue("name", fxName);
      node.AddValue("combinationType", effectMode.ToString());

      node.name = ConfigNodeName;

      return node;
    }

    /// <summary>
    /// Returns true if this specific integrator is ideal for this modifier (ie EffectFloatIntegrator for an EffectFloatModifier and the associated transform matches
    /// </summary>
    /// <param name="integrator"></param>
    /// <returns></returns>
    public virtual bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectIntegrator;

    public abstract EffectIntegrator CreateIntegrator();

    public void CreateOrAttachToIntegrator<T>(List<T> integrators) where T : EffectIntegrator
    {
      if (Controller != null)
      {
        if (integrators == null || !ValidForIntegrator) return;
        T target = integrators.FirstOrDefault(x => IntegratorSuitable(x));
        if (target == null)
        {
          target = CreateIntegrator() as T;
          integrators.Add(target);
        }
        else target.AddModifier(this);
        integrator = target;


      } else 
      {
        Utils.LogWarning($"[EffectModifier]: Controller {controllerName} is null and will not be added to the integrator list");
      }
    }

    public void RemoveFromIntegrator<T>(List<T> integrators) where T : EffectIntegrator
    {
      if (integrator != null)
      {
        integrator.RemoveModifier(this);
      }
      // It would be nice to remove the integrator from the list if it no longer has any modifiers.
      // However, that might leave weird values in the effect/light/transform that could get picked up as the initial values if the integrator gets recreated.
      // We could try and force a final update on the integrator when removing it, but some of them have no effect if the renderer is disabled.
      // It's a lot of complexity to manage and this only ever happens in the editor, so we don't really care about the performance impact of leaving useless integrators in the list
      integrator = null;
    }
  }

  public abstract class EffectModifier_Color : EffectModifier
  {
    public FloatCurve rCurve = new();
    public FloatCurve gCurve = new();
    public FloatCurve bCurve = new();
    public FloatCurve aCurve = new();

    public EffectModifier_Color() : base() { }
    public EffectModifier_Color(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      rCurve.Load(node.GetNode("rCurve"));
      gCurve.Load(node.GetNode("gCurve"));
      bCurve.Load(node.GetNode("bCurve"));
      aCurve.Load(node.GetNode("aCurve"));
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.AddNode(Utils.SerializeFloatCurve("rCurve", rCurve));
      node.AddNode(Utils.SerializeFloatCurve("gCurve", gCurve));
      node.AddNode(Utils.SerializeFloatCurve("bCurve", bCurve));
      node.AddNode(Utils.SerializeFloatCurve("aCurve", aCurve));
      return node;
    }

    public void Get(float[] input, Color[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < output.Length; i++)
        {
          float inValue = input[i];
          output[i] = new(rCurve.Evaluate(inValue) + randomValue,
                          gCurve.Evaluate(inValue) + randomValue,
                          bCurve.Evaluate(inValue) + randomValue,
                          aCurve.Evaluate(inValue) + randomValue);
        }
      }
      else if (input.Length == 1)
      {
        float inValue = input[0];
        Color color = new Color(
          rCurve.Evaluate(inValue) + randomValue,
          gCurve.Evaluate(inValue) + randomValue,
          bCurve.Evaluate(inValue) + randomValue,
          aCurve.Evaluate(inValue) + randomValue);
        for (int i = 0; i < output.Length; i++)
          output[i] = color;
      }
    }
  }
  public abstract class EffectModifier_Vector2 : EffectModifier
  {
    public FloatCurve xCurve = new();
    public FloatCurve yCurve = new();

    public EffectModifier_Vector2() : base() { }
    public EffectModifier_Vector2(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      xCurve.Load(node.GetNode("xCurve"));
      yCurve.Load(node.GetNode("yCurve"));
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.AddNode(Utils.SerializeFloatCurve("xCurve", xCurve));
      node.AddNode(Utils.SerializeFloatCurve("yCurve", yCurve));
      return node;
    }
    public void Get(float[] input, Vector2[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < xforms.Count; i++)
        {
          float inValue = input[i];
          output[i] = new(xCurve.Evaluate(inValue) + randomValue,
                          yCurve.Evaluate(inValue) + randomValue);
        }
      }
      else if (input.Length == 1)
      {
        float inValue = input[0];
        Vector2 vec = new(
          xCurve.Evaluate(inValue) + randomValue,
          yCurve.Evaluate(inValue) + randomValue);
        for (int i = 0; i < xforms.Count; i++)
          output[i] = vec;
      }
    }
  }
  public abstract class EffectModifier_Vector3 : EffectModifier
  {
    public FloatCurve xCurve = new();
    public FloatCurve yCurve = new();
    public FloatCurve zCurve = new();

    public EffectModifier_Vector3() : base() { }
    public EffectModifier_Vector3(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      xCurve.Load(node.GetNode("xCurve"));
      yCurve.Load(node.GetNode("yCurve"));
      zCurve.Load(node.GetNode("zCurve"));
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.AddNode(Utils.SerializeFloatCurve("xCurve", xCurve));
      node.AddNode(Utils.SerializeFloatCurve("yCurve", yCurve));
      node.AddNode(Utils.SerializeFloatCurve("zCurve", zCurve));
      return node;
    }
    public void Get(float[] input, Vector3[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < xforms.Count; i++)
        {
          float inValue = input[i];
          output[i] = new(xCurve.Evaluate(inValue) + randomValue,
                          yCurve.Evaluate(inValue) + randomValue,
                          zCurve.Evaluate(inValue) + randomValue);
        }
      }
      else if (input.Length == 1)
      {
        float inValue = input[0];
        Vector3 vec = new(
          xCurve.Evaluate(inValue) + randomValue,
          yCurve.Evaluate(inValue) + randomValue,
          zCurve.Evaluate(inValue) + randomValue);
        for (int i = 0; i < xforms.Count; i++)
          output[i] = vec;
      }
    }
  }

  public abstract class EffectModifier_Float : EffectModifier
  {
    public FloatCurve curve = new();

    public EffectModifier_Float() : base() { }
    public EffectModifier_Float(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      curve.Load(node.GetNode("floatCurve"));
    }
    public override ConfigNode Save()
    {
      var node = base.Save();
      node.AddNode(Utils.SerializeFloatCurve("floatCurve", curve));
      return node;
    }

    public void Get(float[] input, float[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < output.Length; i++)
          output[i] = curve.Evaluate(input[i]) + randomValue;
      }
      else if (input.Length == 1)
      {
        float data = curve.Evaluate(input[0]) + randomValue;
        for (int i = 0; i < output.Length; i++)
          output[i] = data;
      }
    }
  }
}
