using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectPositionIntegrator : EffectIntegrator
  {
    private readonly List<Vector3> initialVectorValues;

    public EffectPositionIntegrator(WaterfallEffect effect, EffectPositionModifier posMod) : base(effect, posMod)
    {
      initialVectorValues = new();
      foreach (var x in xforms)
      {
        initialVectorValues.Add(x.localPosition);
      }
    }

    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      var applyValues = initialVectorValues.ToList();
      foreach (var posMod in handledModifiers)
      {
        var modResult = (posMod as EffectPositionModifier).Get(parentEffect.parentModule.GetControllerValue(posMod.controllerName));

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