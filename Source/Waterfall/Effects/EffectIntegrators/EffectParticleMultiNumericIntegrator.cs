using System;
using Unity.Profiling;
using UnityEngine;

namespace Waterfall
{
  public class MultiNumericData
  {
    public readonly ParticleSystemCurveMode mode;
    public float const1 = 0f;
    public float const2 = 0f;

    public FastFloatCurve curve1 = new();
    public FastFloatCurve curve2 = new();
    public MultiNumericData() { }
    public MultiNumericData(float c1)
    {
      mode = ParticleSystemCurveMode.Constant;
      const1 = c1;
    }
    public MultiNumericData(float c1, float c2)
    {
      mode = ParticleSystemCurveMode.TwoConstants;
      const1 = c1;
      const2 = c2;
    }
    public MultiNumericData(FastFloatCurve c1)
    {
      mode = ParticleSystemCurveMode.Curve;
      curve1 = c1;
    }
    public MultiNumericData(FastFloatCurve c1, FastFloatCurve c2)
    {
      mode = ParticleSystemCurveMode.TwoCurves;
      curve1 = c1;
      curve2 = c2;
    }
    public static MultiNumericData operator +(MultiNumericData a, MultiNumericData b)
    {
      return a.mode switch
      {
        ParticleSystemCurveMode.Constant => new(a.const1 + b.const1),
        ParticleSystemCurveMode.TwoConstants => new(a.const1 + b.const1, a.const2 + b.const2),
        _ => a,
      };
    }
    public static MultiNumericData operator -(MultiNumericData a, MultiNumericData b)
    {
      return a.mode switch
      {
        ParticleSystemCurveMode.Constant => new(a.const1 - b.const1),
        ParticleSystemCurveMode.TwoConstants => new(a.const1 - b.const1, a.const2 - b.const2),
        _ => a,
      };
    }

    public static MultiNumericData operator *(MultiNumericData a, MultiNumericData b)
    {
      return a.mode switch
      {
        ParticleSystemCurveMode.Constant => new(a.const1 * b.const1),
        ParticleSystemCurveMode.TwoConstants => new(a.const1 * b.const1, a.const2 * b.const2),
        _ => a,
      };
    }
  }
  public class EffectParticleMultiNumericIntegrator : EffectIntegrator
  {
    private string _paramName;
    private ParticleSystemCurveMode curveMode;

    public string paramName
    {
      get { return _paramName; }
      set
      {
        _paramName = value;
      }
    }

    protected readonly MultiNumericData[] modifierData;
    protected readonly MultiNumericData[] initialValues;
    protected readonly MultiNumericData[] workingValues;

    private readonly ParticleSystem[] systems;

    public EffectParticleMultiNumericIntegrator(WaterfallEffect effect, EffectParticleMultiNumericModifier mod) : base(effect, mod)
    {
      modifierData = new MultiNumericData[xforms.Count];
      initialValues = new MultiNumericData[xforms.Count];
      workingValues = new MultiNumericData[xforms.Count];

      paramName = mod.paramName;
      systems = new ParticleSystem[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        modifierData[i] = new();
        workingValues[i] = new();
        initialValues[i] = new();

        systems[i] = xforms[i].GetComponent<ParticleSystem>();

        if (systems[i] == null)
        {
          Utils.LogError($"Integrator for {paramName} for modifier {mod.fxName} in module {effect.parentModule.moduleID} failed to find a particleSystems on transform {transformName}");
        }
        else
        {
          curveMode = ParticleUtils.GetParticleSystemMode(paramName, systems[i]);
          initialValues[i] = new();
          GetParticleSystemMultiValue(paramName, systems[i], ref initialValues[i]);
        }
      }
    }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.IntegratorMultiNumeric.Update");

    internal void GetParticleSystemMultiValue(string paramName, ParticleSystem sys, ref MultiNumericData valueStruct)
    {
      if (curveMode == ParticleSystemCurveMode.Constant)
      {
        ParticleUtils.GetParticleSystemValue(paramName, sys, out float c1);
        valueStruct = new(c1);
      }
      if (curveMode == ParticleSystemCurveMode.TwoConstants)
      {
        ParticleUtils.GetParticleSystemValue(paramName, sys, out float c1, out float c2);
        valueStruct = new(c1, c2);
      }
      if (curveMode == ParticleSystemCurveMode.Curve)
      {
        FastFloatCurve c1 = new();
        FastFloatCurve c2 = new();
        ParticleUtils.GetParticleSystemValue(paramName, sys, ref c1);
        valueStruct = new(c1, c2);
      }
      if (curveMode == ParticleSystemCurveMode.TwoCurves)
      {
        FastFloatCurve c1 = new();
        FastFloatCurve c2 = new();
        ParticleUtils.GetParticleSystemValue(paramName, sys, ref c1);
        ParticleUtils.GetParticleSystemValue(paramName, sys, ref c2);
        valueStruct = new(c1, c2);
      }
    }
    public override void Update()
    {
      s_Update.Begin();

      s_ListPrep.Begin();
      Array.Copy(initialValues, workingValues, initialValues.Length);
      s_ListPrep.End();

      s_Modifiers.Begin();
      foreach (var mod in handledModifiers)
      {
        float[] controllerData = mod.Controller.Get();
        ((EffectParticleMultiNumericModifier)mod).Get(controllerData, modifierData);
        s_Integrate.Begin();
        Integrate(mod.effectMode, workingValues, modifierData);
        s_Integrate.End();
      }
      s_Modifiers.End();

      s_Apply.Begin();
      Apply();
      s_Apply.End();

      s_Update.End();
    }
    protected override void Apply()
    {
      for (int i = 0; i < systems.Length; i++)
      {
        if (curveMode == ParticleSystemCurveMode.Constant)
        {
          ParticleUtils.SetParticleSystemValue(paramName, systems[i], workingValues[i].const1);
        }
        if (curveMode == ParticleSystemCurveMode.TwoConstants)
        {
          ParticleUtils.SetParticleSystemValue(paramName, systems[i], workingValues[i].const1, workingValues[i].const2);
        }
        if (curveMode == ParticleSystemCurveMode.Curve)
        {
          ParticleUtils.SetParticleSystemValue(paramName, systems[i], workingValues[i].curve1);
        }
        if (curveMode == ParticleSystemCurveMode.TwoCurves)
        {
          ParticleUtils.SetParticleSystemValue(paramName, systems[i], workingValues[i].curve1, workingValues[i].curve2);
        }
      }
    }

  }
}
