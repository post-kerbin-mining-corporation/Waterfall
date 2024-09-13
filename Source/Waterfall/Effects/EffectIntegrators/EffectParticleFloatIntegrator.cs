using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{

  public class EffectParticleFloatIntegrator : EffectIntegrator
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
    protected readonly float[] modifierData;
    protected readonly float[] initialValues;
    protected readonly float[] workingValues;

    private readonly WaterfallParticleSystem[] emits;

    public EffectParticleFloatIntegrator(WaterfallEffect effect, EffectParticleFloatModifier particleMod): base (effect, particleMod)
    {

      emits = new WaterfallParticleSystem[xforms.Count];

      modifierData = new float[xforms.Count];
      initialValues = new float[xforms.Count];
      workingValues = new float[xforms.Count];

      particleParamName = particleMod.paramName;


      emits = new WaterfallParticleSystem[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        emits[i] = xforms[i].GetComponent<WaterfallParticleSystem>();
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
          ((EffectParticleFloatModifier)mod).Get(controllerData, modifierData);
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
