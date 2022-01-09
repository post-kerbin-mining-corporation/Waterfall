using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectColorIntegrator : EffectIntegrator
  {
    public string                    colorName;
    public List<EffectColorModifier> handledModifiers;

    private readonly Material[]  m;
    private readonly List<Color> initialColorValues;

    public EffectColorIntegrator(WaterfallEffect effect, EffectColorModifier colorMod)
    {
      Utils.Log(String.Format("[EffectColorIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, colorMod.fxName), LogType.Modifiers);
      xforms        = new();
      transformName = colorMod.transformName;
      parentEffect  = effect;
      var roots = parentEffect.GetModelTransforms();
      foreach (var t in roots)
      {
        var t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectColorIntegrator]: Unable to find transform {0} on modifier {1}", transformName, colorMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


      // float specific
      colorName        = colorMod.colorName;
      handledModifiers = new();
      handledModifiers.Add(colorMod);


      initialColorValues = new();
      m                  = new Material[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
        initialColorValues.Add(m[i].GetColor(colorName));
      }
    }

    public void AddModifier(EffectColorModifier newMod)
    {
      handledModifiers.Add(newMod);
    }

    public void RemoveModifier(EffectColorModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }

    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;
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

      for (int i = 0; i < m.Length; i++)
      {
        m[i].SetColor(colorName, applyValues[i]);
      }
    }
  }
}