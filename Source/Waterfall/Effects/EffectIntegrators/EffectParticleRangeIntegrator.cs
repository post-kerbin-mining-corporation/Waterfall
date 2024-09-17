using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{

  public class EffectParticleRangeIntegrator : EffectIntegrator_Vector2
  {

    private string particleParamName;
    public string paramName
    {
      get { return particleParamName; }
      set
      {
        particleParamName = value;
      }
    }

    private readonly WaterfallParticleSystem[] emits;

    public EffectParticleRangeIntegrator(WaterfallEffect effect, EffectParticleRangeModifier particleMod) : base(effect, particleMod)
    {

      particleParamName = particleMod.paramName;
      emits = new WaterfallParticleSystem[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        emits[i] = xforms[i].GetComponent<WaterfallParticleSystem>();
        emits[i].Get(particleParamName, out initialValues[i]);
      }
    }

    protected override void Apply()
    {
      for (int i = 0; i < emits.Length; i++)
        emits[i].Set(paramName, workingValues[i]);
    }
  }
}
