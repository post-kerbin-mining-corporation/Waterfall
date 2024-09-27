using System;
using Unity.Profiling;
using UnityEngine;

namespace Waterfall
{
  public class MultiColorData
  {
    public readonly ParticleSystemGradientMode mode;
    public Color color1 = new();
    public Color color2 = new();

    public MultiColorData() { }
    public MultiColorData(Color c1)
    {
      mode = ParticleSystemGradientMode.Color;
      color1 = c1;
    }
    public MultiColorData(Color c1, Color c2)
    {
      mode = ParticleSystemGradientMode.TwoColors;
      color1 = c1;
      color2 = c2;
    }
    public MultiColorData(Gradient c1)
    {
      mode = ParticleSystemGradientMode.Gradient;
      /// Not supported yet
    }
    public MultiColorData(Gradient c1, Gradient c2)
    {
      mode = ParticleSystemGradientMode.TwoGradients;
      /// Not supported yet
    }
    public static MultiColorData operator +(MultiColorData a, MultiColorData b)
    {
      return a.mode switch
      {
        ParticleSystemGradientMode.Color => new(a.color1 + b.color1),
        ParticleSystemGradientMode.TwoColors => new(a.color1 + b.color1, a.color2 + b.color2),
        _ => a,
      };
    }
    public static MultiColorData operator -(MultiColorData a, MultiColorData b)
    {
      return a.mode switch
      {
        ParticleSystemGradientMode.Color => new(a.color1 - b.color1),
        ParticleSystemGradientMode.TwoColors => new(a.color1 - b.color1, a.color2 - b.color2),
        _ => a,
      };
    }

    public static MultiColorData operator *(MultiColorData a, MultiColorData b)
    {
      return a.mode switch
      {
        ParticleSystemGradientMode.Color => new(a.color1 * b.color1),
        ParticleSystemGradientMode.TwoColors => new(a.color1 * b.color1, a.color2 * b.color2),
        _ => a,
      };
    }
  }
  public class EffectParticleMultiColorIntegrator : EffectIntegrator
  {
    private string _paramName;
    private ParticleSystemGradientMode curveMode;

    public string paramName
    {
      get { return _paramName; }
      set
      {
        _paramName = value;
      }
    }

    protected readonly MultiColorData[] modifierData;
    protected readonly MultiColorData[] initialValues;
    protected readonly MultiColorData[] workingValues;

    private readonly ParticleSystem[] systems;

    public EffectParticleMultiColorIntegrator(WaterfallEffect effect, EffectParticleMultiColorModifier mod) : base(effect, mod)
    {
      modifierData = new MultiColorData[xforms.Count];
      initialValues = new MultiColorData[xforms.Count];
      workingValues = new MultiColorData[xforms.Count];

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
          curveMode = ParticleUtils.GetParticleSystemColorMode(paramName, systems[i]);
          initialValues[i] = new();
          GetParticleSystemMultiValue(paramName, systems[i], ref initialValues[i]);
        }
      }
    }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.IntegratorMultiNumeric.Update");

    internal void GetParticleSystemMultiValue(string paramName, ParticleSystem sys, ref MultiColorData valueStruct)
    {
      if (curveMode == ParticleSystemGradientMode.Color)
      {
        ParticleUtils.GetParticleSystemValue(paramName, sys, out Color c1);
        valueStruct = new(c1);
      }
      if (curveMode == ParticleSystemGradientMode.TwoColors)
      {
        ParticleUtils.GetParticleSystemValue(paramName, sys, out Color c1, out Color c2);
        valueStruct = new(c1, c2);
      }
      if (curveMode == ParticleSystemGradientMode.Gradient)
      {
        Gradient c2 = new();
        ParticleUtils.GetParticleSystemValue(paramName, sys, out Gradient c1);
        valueStruct = new(c1, c2);
      }
      if (curveMode == ParticleSystemGradientMode.TwoGradients)
      {
        ParticleUtils.GetParticleSystemValue(paramName, sys, out Gradient c1);
        ParticleUtils.GetParticleSystemValue(paramName, sys, out Gradient c2);
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
        if (mod.Controller != null)
        {
          float[] controllerData = mod.Controller.Get();
          ((EffectParticleMultiColorModifier)mod).Get(controllerData, modifierData);
          s_Integrate.Begin();
          Integrate(mod.effectMode, workingValues, modifierData);
          s_Integrate.End();
        }
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
        if (curveMode == ParticleSystemGradientMode.Color)
        {
          ParticleUtils.SetParticleSystemValue(paramName, systems[i], workingValues[i].color1);
        }
        if (curveMode == ParticleSystemGradientMode.TwoColors)
        {
          ParticleUtils.SetParticleSystemValue(paramName, systems[i], workingValues[i].color1, workingValues[i].color2);
        }
        if (curveMode == ParticleSystemGradientMode.Gradient)
        {
          /// Not supported yet
        }
        if (curveMode == ParticleSystemGradientMode.TwoGradients)
        {
          /// Not supported yet
        }
      }
    }

  }
}
