using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectPositionIntegrator : EffectIntegrator
  {
    public List<EffectPositionModifier> handledModifiers;

    private readonly List<Vector3> initialVectorValues;

    public EffectPositionIntegrator(WaterfallEffect effect, EffectPositionModifier posMod)
    {
      Utils.Log(String.Format("[EffectPositionIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, posMod.fxName), LogType.Modifiers);
      xforms        = new();
      transformName = posMod.transformName;
      parentEffect  = effect;
      var roots = parentEffect.GetModelTransforms();
      foreach (var t in roots)
      {
        var t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectPositionIntegrator]: Unable to find transform {0} on modifier {1}", transformName, posMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


      handledModifiers = new();
      handledModifiers.Add(posMod);


      initialVectorValues = new();
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
      if (handledModifiers.Count == 0)
        return;
      var applyValues = initialVectorValues.ToList();
      foreach (var posMod in handledModifiers)
      {
        var modResult = posMod.Get(parentEffect.parentModule.GetControllerValue(posMod.controllerName));

        if (posMod.effectMode == EffectModifierMode.REPLACE)
          applyValues = modResult;

        if (posMod.effectMode == EffectModifierMode.MULTIPLY)
          for (int i = 0; i < applyValues.Count; i++)
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