using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectRotationIntegrator : EffectIntegrator
  {
    private readonly List<Vector3> initialVectorValues;

    public EffectRotationIntegrator(WaterfallEffect effect, EffectRotationModifier mod) : base(effect, mod)
    {
      initialVectorValues = new();
      foreach (var x in xforms)
      {
        initialVectorValues.Add(x.localEulerAngles);
      }
    }

    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      var applyValues = initialVectorValues.ToList();
      foreach (var mod in handledModifiers)
      {
        var modResult = (mod as EffectRotationModifier).Get(parentEffect.parentModule.GetControllerValue(mod.controllerName));

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
        xforms[i].localEulerAngles = applyValues[i];
      }
    }
  }
}