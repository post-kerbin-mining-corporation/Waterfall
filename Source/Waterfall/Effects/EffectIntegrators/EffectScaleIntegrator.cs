using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectScaleIntegrator : EffectIntegrator
  {
    protected readonly Vector3[] modifierData;
    protected readonly Vector3[] initialValues;
    protected readonly Vector3[] workingValues;
    
    public EffectScaleIntegrator(WaterfallEffect effect, EffectScaleModifier mod) : base(effect, mod)
    {
      modifierData = new Vector3[xforms.Count];
      initialValues = new Vector3[xforms.Count];
      workingValues = new Vector3[xforms.Count];

      for(int i = 0; i < xforms.Count; i++)
        initialValues[i] = xforms[i].localScale;
    }

    public override void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      Array.Copy(initialValues, workingValues, initialValues.Length);

      for (int i = 0; i < handledModifiers.Count; i++)
      {
        var mod = handledModifiers[i];
        if (mod.Controller != null)
        {
          float[] controllerData = mod.Controller.Get();
          ((EffectScaleModifier) mod).Get(controllerData, modifierData);
          Integrate(mod.effectMode, workingValues, modifierData);
        }
      }

      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localScale = workingValues[i];
    }
  }
}
