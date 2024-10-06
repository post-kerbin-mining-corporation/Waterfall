using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectRotationIntegrator : EffectIntegrator_Vector3
  {
    public EffectRotationIntegrator(WaterfallEffect effect, EffectRotationModifier mod) : base(effect, mod)
    {
      for (int i = 0; i < xforms.Count; i++)
        initialValues[i] = xforms[i].localEulerAngles;
    }

    protected override void Apply()
    {
      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localEulerAngles = workingValues[i];
    }
  }
}
