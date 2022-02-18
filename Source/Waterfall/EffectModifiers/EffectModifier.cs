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
    [Persistent] public string controllerName   = "";
    [Persistent] public string transformName    = "";
    public string modifierTypeName = "";

    [Persistent] public bool   useRandomness;
    [Persistent] public string randomnessController = nameof(RandomnessController);
    [Persistent] public float  randomnessScale          = 1f;

    public WaterfallEffect parentEffect;

    // The combination mode of this effect with the base
    public EffectModifierMode effectMode = EffectModifierMode.REPLACE;

    // The Transform that holds the thing the effect should modify
    protected List<Transform> xforms;
    protected List<float> controllerData = new();
    protected float           randomValue;

    public EffectIntegrator integrator;
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
      node.TryGetValue("name",           ref fxName);
      node.TryGetEnum("combinationType", ref effectMode, EffectModifierMode.REPLACE);
      Utils.Log($"[EffectModifier]: Loading modifier {fxName}", LogType.Modifiers);
    }

    /// <summary>
    ///   Initialize the effect
    /// </summary>
    public virtual void Init(WaterfallEffect effect)
    {
      parentEffect = effect;
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

    public virtual ConfigNode Save()
    {
      var node = ConfigNode.CreateConfigFromObject(this);
      node.AddValue("name", fxName);
      node.AddValue("combinationType", effectMode.ToString());
      return node;
    }

    /// <summary>
    ///   Apply the effect with the various combine modes
    /// </summary>
    /// <param name="strength"></param>
    public virtual void Apply(List<float> strength)
    {
      if (useRandomness)
      {
        parentEffect.parentModule.GetControllerValue(randomnessController, controllerData);
        randomValue = controllerData[0] * randomnessScale;
      }

      switch (effectMode)
      {
        case EffectModifierMode.REPLACE:
          ApplyReplace(strength);
          break;
        case EffectModifierMode.ADD:
          ApplyAdd(strength);
          break;
        case EffectModifierMode.MULTIPLY:
          ApplyMultiply(strength);
          break;
        case EffectModifierMode.SUBTRACT:
          ApplySubtract(strength);
          break;
      }
    }

    protected virtual void ApplyReplace(List<float> strength) { }

    protected virtual void ApplyAdd(List<float> strength) { }

    protected virtual void ApplyMultiply(List<float> strength) { }

    protected virtual void ApplySubtract(List<float> strength) { }

    /// <summary>
    /// Returns true if this specific integrator is ideal for this modifier (ie EffectFloatIntegrator for an EffectFloatModifier and the associated transform matches
    /// </summary>
    /// <param name="integrator"></param>
    /// <returns></returns>
    public virtual bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectIntegrator;

    public abstract EffectIntegrator CreateIntegrator();

    public virtual void CreateOrAttachToIntegrator<T>(List<T> integrators) where T : EffectIntegrator
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
    }

    public virtual void RemoveFromIntegrator<T>(List<T> integrators) where T : EffectIntegrator
    {
      if (integrators?.FirstOrDefault(x => x.handledModifiers.Contains(this)) is T integrator)
      {
        integrator.RemoveModifier(this);
        integrator = null;
      }
    }
  }
}
