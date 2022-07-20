using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectLightColorIntegrator : EffectIntegrator
  {
    public string                         colorName;
    protected readonly Color[] modifierData;
    protected readonly Color[] initialValues;
    protected readonly Color[] workingValues;

    private readonly Light[]     l;

    public EffectLightColorIntegrator(WaterfallEffect effect, EffectLightColorModifier floatMod) : base(effect, floatMod)
    {
      // light-color specific
      colorName        = floatMod.colorName;
      l = new Light[xforms.Count];
      modifierData = new Color[xforms.Count];
      initialValues = new Color[xforms.Count];
      workingValues = new Color[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();
        initialValues[i] = l[i].color;
      }
    }

    public override void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      Array.Copy(initialValues, workingValues, l.Length);

      foreach (var mod in handledModifiers)
      {
        if (mod.Controller != null)
        {
          float[] controllerData = mod.Controller.Get();
          ((EffectLightColorModifier)mod).Get(controllerData, modifierData);
          Integrate(mod.effectMode, workingValues, modifierData);
        }
      }

      for (int i = 0; i < l.Length; i++)
        l[i].color = workingValues[i];
    }
  }
}
