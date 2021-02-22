using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
 

  public class EffectPositionIntegrator: EffectIntegrator
  {
    
    List<Vector3> initialVectorValues;
    public List<EffectPositionModifier> handledModifiers;
    
    public EffectPositionIntegrator(WaterfallEffect effect, EffectPositionModifier posMod)
    {
      Utils.Log(String.Format("[EffectPositionIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, posMod.fxName), LogType.Modifiers);
      xforms = new List<Transform>();
      transformName = posMod.transformName;
      parentEffect = effect;
      List<Transform> roots = parentEffect.GetModelTransforms();
      foreach (Transform t in roots)
      {
        Transform t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectPositionIntegrator]: Unable to find transform {0} on modifier {1}", transformName, posMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


     
      handledModifiers = new List<EffectPositionModifier>();
      handledModifiers.Add(posMod);


      initialVectorValues = new List<Vector3>();
      for (int i = 0; i < xforms.Count; i++)
      {

        initialVectorValues.Add(xforms[i].localPosition);
      }
    }
    public void AddModifier(EffectPositionModifier newMod)
    {
      handledModifiers.Add(newMod);
    }
    public void RemoveModifier(EffectPositionModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }

    public void Update()
    {
      List<Vector3> applyValues = initialVectorValues;
      foreach (EffectPositionModifier posMod in handledModifiers)
      {
        List<Vector3> modResult = posMod.Get(parentEffect.parentModule.GetControllerValue(posMod.controllerName));

        if (posMod.effectMode == EffectModifierMode.REPLACE)
          applyValues = modResult;

        if (posMod.effectMode == EffectModifierMode.MULTIPLY)
          for (int i = 0; i < applyValues.Count;i++)
            applyValues[i] = Vector3.Scale(applyValues[i], modResult[i]);

        if (posMod.effectMode == EffectModifierMode.ADD)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] + modResult[i];

        if (posMod.effectMode == EffectModifierMode.SUBTRACT)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] - modResult[i];

      }
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localPosition = applyValues[i];
      }

    }
  }

 
  
}
