using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectScaleIntegrator : EffectIntegrator
  {
    private readonly List<Vector3> initialVectorValues;

    public EffectScaleIntegrator(WaterfallEffect effect, EffectScaleModifier mod) : base(effect, mod)
    {
      initialVectorValues = new();
      foreach (var x in xforms)
      {
        initialVectorValues.Add(x.localScale);
      }
    }

    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      var applyValues = initialVectorValues.ToList();
      foreach (var mod in handledModifiers)
      {
        var modResult = (mod as EffectScaleModifier).Get(parentEffect.parentModule.GetControllerValue(mod.controllerName));

        if (mod.effectMode == EffectModifierMode.REPLACE)
          applyValues = modResult;

        if (mod.effectMode == EffectModifierMode.MULTIPLY)
          for (int i = 0; i < applyValues.Count; i++)
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