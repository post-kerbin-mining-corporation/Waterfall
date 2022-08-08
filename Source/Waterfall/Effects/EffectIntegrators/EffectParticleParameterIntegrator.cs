using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{

  public class EffectParticleParameterIntegrator : EffectIntegrator
  {

    private string particleParamName;

    protected readonly Vector2[] modifierData;
    protected readonly Vector2[] initialValues;
    protected readonly Vector2[] workingValues;

    private readonly WaterfallParticleEmitter[] emits;

    public EffectParticleParameterIntegrator(WaterfallEffect effect, EffectParticleSystemModifier particleMod): base (effect, particleMod)
    {

      emits = new WaterfallParticleEmitter[xforms.Count];

      modifierData = new Vector2[xforms.Count];
      initialValues = new Vector2[xforms.Count];
      workingValues = new Vector2[xforms.Count];

      particleParamName = particleMod.paramName;


      emits = new WaterfallParticleEmitter[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        emits[i] = xforms[i].GetComponent<WaterfallParticleEmitter>();
        initialValues[i] = emits[i].Get(particleParamName);
      }
    }

    public override void Update()
    {

      if (handledModifiers.Count == 0)
        return;

      Array.Copy(initialValues, workingValues, emits.Length);

      foreach (var mod in handledModifiers)
      {
        if (mod.Controller != null)
        {
          float[] controllerData = mod.Controller.Get();
          ((EffectParticleSystemModifier)mod).Get(controllerData, modifierData);
          Integrate(mod.effectMode, workingValues, modifierData);
        }
      }

      for (int i = 0; i < emits.Length; i++)
      {
        emits[i].Set(particleParamName, workingValues[i]);
      }

    }
  }



}
