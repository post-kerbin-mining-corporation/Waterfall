using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
 

  public class EffectScaleIntegrator: EffectIntegrator
  {
    
    List<Vector3> initialVectorValues;
    public List<EffectScaleModifier> handledModifiers;
    
    public EffectScaleIntegrator(WaterfallEffect effect, EffectScaleModifier mod)
    {
      Utils.Log(String.Format("[EffectScaleIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, mod.fxName), LogType.Modifiers);
      xforms = new List<Transform>();
      transformName = mod.transformName;
      parentEffect = effect;
      List<Transform> roots = parentEffect.GetModelTransforms();
      foreach (Transform t in roots)
      {
        Transform t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectScaleIntegrator]: Unable to find transform {0} on modifier {1}", transformName, mod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


     
      handledModifiers = new List<EffectScaleModifier>();
      handledModifiers.Add(mod);


      initialVectorValues = new List<Vector3>();
      for (int i = 0; i < xforms.Count; i++)
      {

        initialVectorValues.Add(xforms[i].localScale);
      }
    }
    public void AddModifier(EffectScaleModifier newMod)
    {
      handledModifiers.Add(newMod);
    }
    public void RemoveModifier(EffectScaleModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }
    public void Update()
    {
      List<Vector3> applyValues = initialVectorValues;
      foreach (EffectScaleModifier mod in handledModifiers)
      {
        List<Vector3> modResult = mod.Get(parentEffect.parentModule.GetControllerValue(mod.controllerName));

        if (mod.effectMode == EffectModifierMode.REPLACE)
          applyValues = modResult;

        if (mod.effectMode == EffectModifierMode.MULTIPLY)
          for (int i = 0; i < applyValues.Count;i++)
            applyValues[i] = Vector3.Scale(applyValues[i], modResult[i]);

        if (mod.effectMode == EffectModifierMode.ADD)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] + modResult[i];

        if (mod.effectMode == EffectModifierMode.SUBTRACT)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] - modResult[i];

      }
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localScale = applyValues[i];
      }

    }
  }

 
  
}
