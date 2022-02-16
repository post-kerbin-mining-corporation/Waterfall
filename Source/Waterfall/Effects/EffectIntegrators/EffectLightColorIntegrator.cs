using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectLightColorIntegrator : EffectIntegrator
  {
    public string                         colorName;

    private readonly Light[]     l;
    private readonly List<Color> initialColorValues;

    public EffectLightColorIntegrator(WaterfallEffect effect, EffectLightColorModifier floatMod) : base(effect, floatMod)
    {
      // light-color specific
      colorName        = floatMod.colorName;
      initialColorValues = new();
      l = new Light[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();
        initialColorValues.Add(l[i].color);
      }
    }

    public void Update()
    {
      if (handledModifiers.Count > 0)
      {
        var applyValues = initialColorValues.ToList();
        foreach (var colorMod in handledModifiers)
        {
          var modResult = (colorMod as EffectLightColorModifier).Get(parentEffect.parentModule.GetControllerValue(colorMod.controllerName));

          if (colorMod.effectMode == EffectModifierMode.REPLACE)
            applyValues = modResult;

          if (colorMod.effectMode == EffectModifierMode.MULTIPLY)
            for (int i = 0; i < applyValues.Count; i++)
              applyValues[i] = applyValues[i] * modResult[i];

          if (colorMod.effectMode == EffectModifierMode.ADD)
            for (int i = 0; i < applyValues.Count; i++)
              applyValues[i] = applyValues[i] + modResult[i];

          if (colorMod.effectMode == EffectModifierMode.SUBTRACT)
            for (int i = 0; i < applyValues.Count; i++)
              applyValues[i] = applyValues[i] - modResult[i];
        }

        for (int i = 0; i < l.Length; i++)
        {
          l[i].color = applyValues[i];
        }
      }
    }
  }
}