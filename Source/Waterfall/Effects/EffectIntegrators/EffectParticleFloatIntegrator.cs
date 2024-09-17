using System;

namespace Waterfall
{

  public class EffectParticleFloatIntegrator : EffectIntegrator_Float
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

    /// TODO: Apply the testIntensity flow to this
    public EffectParticleFloatIntegrator(WaterfallEffect effect, EffectParticleFloatModifier particleMod): base (effect, particleMod, false)
    {

      emits = new WaterfallParticleSystem[xforms.Count];
      particleParamName = particleMod.paramName;


      emits = new WaterfallParticleSystem[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        emits[i] = xforms[i].GetComponent<WaterfallParticleSystem>();
        emits[i].Get(particleParamName, out initialValues[i]);
      }
    }

    protected override bool Apply_TestIntensity()
    {
      bool anyActive = true;

      if (testIntensity)
      {
        anyActive = false;
        for (int i = 0; i < emits.Length; i++)
        {
          emits[i].Set(particleParamName, workingValues[i]);
        }
      }
      else
      {
        for (int i = 0; i < emits.Length; i++)
        {
          emits[i].Set(particleParamName, workingValues[i]);
        }
      }
      return anyActive;
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
