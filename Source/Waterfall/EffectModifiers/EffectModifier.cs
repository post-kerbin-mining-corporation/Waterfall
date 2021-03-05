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

    public bool useRandomness = false;
    public string randomnessController = "random";
    public float randomScale = 1f;
    // The Transform that holds the thing the effect should modify
    protected List<Transform> xforms;
    public WaterfallEffect parentEffect;
    protected float randomValue = 0f;


    // The combination mode of this effect with the base
    public EffectModifierMode effectMode = EffectModifierMode.REPLACE;


    public EffectModifier()
    {
      xforms = new List<Transform>();
    }
    public EffectModifier(ConfigNode node) : this() { Load(node); }
    public virtual void Load(ConfigNode node)
    {
      node.TryGetValue("name", ref fxName);
      node.TryGetValue("controllerName", ref controllerName);
      node.TryGetValue("transformName", ref transformName);
      node.TryGetEnum<EffectModifierMode>("combinationType", ref effectMode, EffectModifierMode.REPLACE);
      node.TryGetValue("randomnessScale", ref randomScale);
      node.TryGetValue("useRandomness", ref useRandomness);
      node.TryGetValue("randomnessController", ref randomnessController);
      Utils.Log(String.Format("[EffectModifier]: Loading modifier {0} ", fxName), LogType.Modifiers);
    }
    /// <summary>
    /// Initialize the effect
    /// </summary>
    public virtual void Init(WaterfallEffect effect)
    {
      parentEffect = effect;
      Utils.Log(String.Format("[EffectModifier]: Initializing modifier {0} ", fxName), LogType.Modifiers);
      List<Transform> roots = parentEffect.GetModelTransforms();
      xforms = new List<Transform>();
      foreach (Transform t in roots)
      {
        Transform t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectModifier]: Unable to find transform {0} on modifier {1}", transformName, fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }
    }
    public virtual ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.AddValue("name", fxName);
      node.AddValue("controllerName", controllerName);
      node.AddValue("transformName", transformName);
      node.AddValue("combinationType", effectMode.ToString());
      node.AddValue("useRandomness", useRandomness);
      node.AddValue("randomnessController", randomnessController);
      node.AddValue("randomnessScale", randomScale);



      return node;
    }
    /// <summary>
    /// Apply the effect with the various combine modes
    /// </summary>
    /// <param name="strength"></param>
    public virtual void Apply(List<float> strength)
    {
      
      if (useRandomness)
      {
        randomValue = parentEffect.parentModule.GetControllerValue(randomnessController)[0] * randomScale;

        //Utils.Log($"{useRandomness} {parentEffect.parentModule.GetControllerValue(randomnessController)} {randomScale} {randomValue}");
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

    protected virtual void ApplyReplace(List<float> strength)
    { }

    protected virtual void ApplyAdd(List<float> strength)
    { }

    protected virtual void ApplyMultiply(List<float> strength)
    { }

    protected virtual void ApplySubtract(List<float> strength)
    { }

  }

  


}
