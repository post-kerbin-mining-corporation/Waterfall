using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectPositionIntegrator : EffectIntegrator_Vector3
  {

    public EffectPositionIntegrator(WaterfallEffect effect, EffectPositionModifier posMod) : base(effect, posMod)
    {
      for (int i = 0; i < xforms.Count; i++)
        initialValues[i] = xforms[i].localPosition;
    }

    protected override void Apply()
    {
      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localPosition = workingValues[i];
    }
  }
}
