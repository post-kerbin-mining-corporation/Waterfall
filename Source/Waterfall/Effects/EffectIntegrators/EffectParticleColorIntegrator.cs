using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Waterfall.EffectModifiers;

namespace Waterfall
{

  public class EffectParticleColorIntegrator : EffectIntegrator
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
    protected readonly Color[] modifierData;
    protected readonly Color[] initialValues;
    protected readonly Color[] workingValues;

    private readonly WaterfallParticleEmitter[] emits;

    public EffectParticleColorIntegrator(WaterfallEffect effect, EffectParticleColorModifier particleMod) : base(effect, particleMod)
    {

      emits = new WaterfallParticleEmitter[xforms.Count];

      modifierData = new Color[xforms.Count];
      initialValues = new Color[xforms.Count];
      workingValues = new Color[xforms.Count];

      particleParamName = particleMod.paramName;
      emits = new WaterfallParticleEmitter[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        emits[i] = xforms[i].GetComponent<WaterfallParticleEmitter>();
        emits[i].Get(particleParamName, out initialValues[i]);
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
          ((EffectParticleColorModifier)mod).Get(controllerData, modifierData);
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
