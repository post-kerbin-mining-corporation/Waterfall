using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectColorIntegrator : EffectIntegrator
  {
    public string                    colorName;

    private readonly Material[]  m;
    private readonly List<Color> initialColorValues;

    public EffectColorIntegrator(WaterfallEffect effect, EffectColorModifier colorMod) : base(effect, colorMod)
    {
      // color specific
      colorName        = colorMod.colorName;

      initialColorValues = new();
      m                  = new Material[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
        initialColorValues.Add(m[i].GetColor(colorName));
      }
    }

    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      var applyValues = initialColorValues.ToList();
      foreach (var colorMod in handledModifiers)
      {
        var modResult = (colorMod as EffectColorModifier).Get(parentEffect.parentModule.GetControllerValue(colorMod.controllerName));

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

      for (int i = 0; i < m.Length; i++)
      {
        m[i].SetColor(colorName, applyValues[i]);
      }
    }
  }
}