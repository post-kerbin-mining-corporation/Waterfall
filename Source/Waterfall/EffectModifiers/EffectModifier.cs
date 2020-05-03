using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
  /// Base effect modifer class
  /// </summary>
  public class EffectModifier
  {
    // This is the game name of the effect
    public string fxName = "";
    // This is the name of the controller that should be associated with the module
    public string controllerName = "";
    public string transformName = "";
    public string modifierTypeName = "";
    // The Transform that holds the thing the effect should modify
    protected Transform xform;
    protected WaterfallEffect parentEffect;

    // The combination mode of this effect with the base
    public EffectModifierMode effectMode = EffectModifierMode.REPLACE;


    public EffectModifier() { }
    public EffectModifier(ConfigNode node) { Load(node); }
    public virtual void Load(ConfigNode node)
    {
      node.TryGetValue("name", ref fxName);
      node.TryGetValue("controllerName", ref controllerName);
      node.TryGetValue("transformName", ref transformName);
      node.TryGetEnum<EffectModifierMode>("combinationType", ref effectMode, EffectModifierMode.REPLACE);
      Utils.Log(String.Format("[EffectModifier]: Loding modifier {0} ", fxName));
    }
    /// <summary>
    /// Initialize the effect
    /// </summary>
    public virtual void Init(WaterfallEffect parentEffect)
    {
      Utils.Log(String.Format("[EffectModifier]: Initializing modifier {0} ", fxName));
      Transform root = parentEffect.GetModelTransform();
      xform = root.FindDeepChild(transformName);
      if (xform == null)
      {
        Utils.LogError(String.Format("[EffectModifier]: Unabled to find transform {0} on modifier {1}", transformName, fxName));
      }
    }
    public virtual ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.AddValue("name", fxName);
      node.AddValue("controllerName", controllerName);
      node.AddValue("transformName", transformName);
      node.AddValue("combinationType", effectMode.ToString());


      return node;
    }
    /// <summary>
    /// Apply the effect with the various combine modes
    /// </summary>
    /// <param name="strength"></param>
    public virtual void Apply(float strength)
    {
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

    protected virtual void ApplyReplace(float strength)
    { }

    protected virtual void ApplyAdd(float strength)
    { }

    protected virtual void ApplyMultiply(float strength)
    { }

    protected virtual void ApplySubtract(float strength)
    { }

  }

  


}
