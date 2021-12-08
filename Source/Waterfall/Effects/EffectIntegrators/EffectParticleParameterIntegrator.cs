using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{

  public class EffectParticleParameterIntegrator : EffectIntegrator
  {

    //Material[] m;
    //Renderer[] r;
    public string paramName;

    List<Vector2> initialValues;
    public List<EffectParticleSystemModifier> handledModifiers;

    WaterfallParticleEmitter[] emits;

    public EffectParticleParameterIntegrator(WaterfallEffect effect, EffectParticleSystemModifier particleMod)
    {
      Utils.Log(String.Format("[EffectParticleParameterIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, particleMod.fxName), LogType.Modifiers);
      xforms = new List<Transform>();
      transformName = particleMod.transformName;
      parentEffect = effect;
      List<Transform> roots = parentEffect.GetModelTransforms();
      foreach (Transform t in roots)
      {
        Transform t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectParticleParameterIntegrator]: Unable to find transform {0} on modifier {1}", transformName, particleMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


      // float specific
      paramName = particleMod.paramName;
      handledModifiers = new List<EffectParticleSystemModifier>();
      handledModifiers.Add(particleMod);


      // TODO: REWRITE
      //foreach (string nm in WaterfallConstants.ShaderPropertyHideFloatNames)
      //{
      //  if (paramName == nm)
      //    testIntensity = true;
      //}
      emits = new WaterfallParticleEmitter[xforms.Count];
      initialValues = new List<Vector2>();
      for (int i = 0; i < xforms.Count; i++)
      {
        emits[i] = xforms[i].GetComponent<WaterfallParticleEmitter>();
        // r[i] = xforms[i].GetComponent<Renderer>();
        //  m[i] = r[i].material;
        initialValues.Add(emits[i].Get(paramName));
      }
    }
    public void AddModifier(EffectParticleSystemModifier newMod)
    {
      handledModifiers.Add(newMod);
    }
    public void RemoveModifier(EffectParticleSystemModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }
    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;

      List<Vector2> applyValues = initialValues.ToList();

      foreach (EffectParticleSystemModifier particleMod in handledModifiers)
      {
        List<Vector2> modResult = particleMod.Get(parentEffect.parentModule.GetControllerValue(particleMod.controllerName));

        if (particleMod.effectMode == EffectModifierMode.REPLACE)
          applyValues = modResult;

        if (particleMod.effectMode == EffectModifierMode.MULTIPLY)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] * modResult[i];

        if (particleMod.effectMode == EffectModifierMode.ADD)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] + modResult[i];

        if (particleMod.effectMode == EffectModifierMode.SUBTRACT)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] - modResult[i];

      }

      for (int i = 0; i < emits.Length; i++)
      {
        emits[i].Set(paramName, applyValues[i]);
      }

    }
  }



}
