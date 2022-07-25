using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectScaleIntegrator : EffectIntegrator_Vector3
  { 
    public EffectScaleIntegrator(WaterfallEffect effect, EffectScaleModifier mod) : base(effect, mod)
    {
      for(int i = 0; i < xforms.Count; i++)
        initialValues[i] = xforms[i].localScale;
    }

    protected override void Apply()
    {
      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localScale = workingValues[i];
    }
  }
}
