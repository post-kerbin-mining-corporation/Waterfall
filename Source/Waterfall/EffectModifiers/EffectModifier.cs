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
    public string controllerName   = "";
    public string transformName    = "";
    public string modifierTypeName = "";

    public bool   useRandomness;
    public string randomnessController = nameof(RandomnessController);
    public float  randomScale          = 1f;

    public WaterfallEffect parentEffect;

    // The combination mode of this effect with the base
    public EffectModifierMode effectMode = EffectModifierMode.REPLACE;

    // The Transform that holds the thing the effect should modify
    protected List<Transform> xforms;
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
      node.TryGetValue("name",           ref fxName);
      node.TryGetValue("controllerName", ref controllerName);
      node.TryGetValue("transformName",  ref transformName);
      node.TryGetEnum("combinationType", ref effectMode, EffectModifierMode.REPLACE);
      node.TryGetValue("randomnessScale",      ref randomScale);
      node.TryGetValue("useRandomness",        ref useRandomness);
      node.TryGetValue("randomnessController", ref randomnessController);
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
      var node = new ConfigNode();
      node.AddValue("name",                 fxName);
      node.AddValue("controllerName",       controllerName);
      node.AddValue("transformName",        transformName);
      node.AddValue("combinationType",      effectMode.ToString());
      node.AddValue("useRandomness",        useRandomness);
      node.AddValue("randomnessController", randomnessController);
      node.AddValue("randomnessScale",      randomScale);
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
        randomValue = parentEffect.parentModule.GetControllerValue(randomnessController)[0] * randomScale;
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
