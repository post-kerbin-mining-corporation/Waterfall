using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectIntegrator
  {
    public string transformName;
    protected WaterfallEffect parentEffect;
    protected List<Transform> xforms;
    public List<EffectModifier> handledModifiers;
    public virtual void AddModifier(EffectModifier mod) => handledModifiers.Add(mod);
    public virtual void RemoveModifier(EffectModifier mod) => handledModifiers.Remove(mod);

    public EffectIntegrator(WaterfallEffect effect, EffectModifier mod)
    {
      Utils.Log($"[EffectIntegrator]: Initializing integrator for {effect.name} on modifier {mod.fxName}", LogType.Modifiers);
      transformName = mod.transformName;
      parentEffect = effect;

      xforms = new();
      var roots = parentEffect.GetModelTransforms();
      foreach (var t in roots)
      {
        if (t.FindDeepChild(transformName) is Transform t1 && t1 != null)
          xforms.Add(t1);
        else
          Utils.LogError($"[EffectIntegrator]: Unable to find transform {mod.transformName} on modifier {mod.fxName}");
      }

      handledModifiers = new();
      handledModifiers.Add(mod);
    }
  }

  public class EffectFloatIntegrator : EffectIntegrator
  {
    public string                    floatName;

    private readonly Material[] m;
    private readonly Renderer[] r;

    private readonly List<float> initialFloatValues;

    private readonly bool testIntensity;

    public EffectFloatIntegrator(WaterfallEffect effect, EffectFloatModifier floatMod) : base(effect, floatMod)
    {
      // float specific
      floatName        = floatMod.floatName;

      foreach (string nm in WaterfallConstants.ShaderPropertyHideFloatNames)
      {
        if (floatName == nm)
          testIntensity = true;
      }

      initialFloatValues = new();
      m                  = new Material[xforms.Count];
      r                  = new Renderer[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        r[i] = xforms[i].GetComponent<Renderer>();
        m[i] = r[i].material;
        initialFloatValues.Add(m[i].GetFloat(floatName));
      }
    }


    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      var applyValues = initialFloatValues.ToList();

      foreach (var floatMod in handledModifiers)
      {
        var modResult = (floatMod as EffectFloatModifier).Get(parentEffect.parentModule.GetControllerValue(floatMod.controllerName));

        if (floatMod.effectMode == EffectModifierMode.REPLACE)
          applyValues = modResult;

        if (floatMod.effectMode == EffectModifierMode.MULTIPLY)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] * modResult[i];

        if (floatMod.effectMode == EffectModifierMode.ADD)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] + modResult[i];

        if (floatMod.effectMode == EffectModifierMode.SUBTRACT)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] - modResult[i];
      }

      for (int i = 0; i < m.Length; i++)
      {
        if (testIntensity)
        {
          if (r[i].enabled && applyValues[i] < Settings.MinimumEffectIntensity)
          {
            r[i].enabled = false;
          }
          else if (!r[i].enabled && applyValues[i] >= Settings.MinimumEffectIntensity)
          {
            r[i].enabled = true;
            m[i].SetFloat(floatName, applyValues[i]);
          }
          else if (r[i].enabled && applyValues[i] >= Settings.MinimumEffectIntensity)
          {
            m[i].SetFloat(floatName, applyValues[i]);
          }
        }
        else
        {
          m[i].SetFloat(floatName, applyValues[i]);
        }
      }
    }
  }
}