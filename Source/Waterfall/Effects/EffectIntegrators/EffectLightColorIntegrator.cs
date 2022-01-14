using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectLightColorIntegrator : EffectIntegrator
  {
    public string                         colorName;
    public List<EffectLightColorModifier> handledModifiers;

    private readonly Light[]     l;
    private readonly List<Color> initialColorValues;

    public EffectLightColorIntegrator(WaterfallEffect effect, EffectLightColorModifier floatMod)
    {
      Utils.Log(String.Format("[EffectLightColorIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, floatMod.fxName), LogType.Modifiers);
      xforms        = new();
      transformName = floatMod.transformName;
      parentEffect  = effect;
      var roots = parentEffect.GetModelTransforms();
      foreach (var t in roots)
      {
        var t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectLightColorIntegrator]: Unable to find transform {0} on modifier {1}", transformName, floatMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


      // float specific
      colorName        = floatMod.colorName;
      handledModifiers = new();
      handledModifiers.Add(floatMod);

      initialColorValues = new();

      l = new Light[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();


        initialColorValues.Add(l[i].color);
      }
    }

    public void AddModifier(EffectLightColorModifier newMod)
    {
      handledModifiers.Add(newMod);
    }

    public void RemoveModifier(EffectLightColorModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }

    public void Update()
    {
      if (handledModifiers.Count > 0)
      {
        var applyValues = initialColorValues.ToList();
        foreach (var colorMod in handledModifiers)
        {
          var modResult = colorMod.Get(parentEffect.parentModule.GetControllerValue(colorMod.controllerName));

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