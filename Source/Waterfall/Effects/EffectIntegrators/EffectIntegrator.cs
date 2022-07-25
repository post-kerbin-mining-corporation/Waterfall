using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Waterfall
{
  public abstract class EffectIntegrator
  {
    public string transformName;
    protected WaterfallEffect parentEffect;
    protected List<Transform> xforms = new();
    public List<EffectModifier> handledModifiers = new();
    public virtual void AddModifier(EffectModifier mod)
    {
      handledModifiers.Add(mod);
      if (mod.Controller != null)
      {
        mod.Controller.referencingModifierCount++; // the original code also evaluated controllers from the integrator, so we need to account for that here
      }
    }
    public virtual void RemoveModifier(EffectModifier mod) => handledModifiers.Remove(mod);

    public EffectIntegrator(WaterfallEffect effect, EffectModifier mod)
    {
      Utils.Log($"[EffectIntegrator]: Initializing integrator for {effect.name} on modifier {mod.fxName}", LogType.Modifiers);
      transformName = mod.transformName;
      parentEffect = effect;

      var roots = parentEffect.GetModelTransforms();
      foreach (var t in roots)
      {
        if (t.FindDeepChild(transformName) is Transform t1 && t1 != null)
          xforms.Add(t1);
        else
          Utils.LogError($"[EffectIntegrator]: Unable to find transform {mod.transformName} on modifier {mod.fxName}");
      }

      AddModifier(mod);
    }

    protected abstract void Apply();

    public void Integrate(EffectModifierMode mode, float[] items, float[] modifiers)
    {
      int count = Math.Min(items.Length, modifiers.Length);
      for (int i = 0; i < count; i++)
        items[i] = mode switch
        {
          EffectModifierMode.REPLACE => modifiers[i],
          EffectModifierMode.MULTIPLY => items[i] * modifiers[i],
          EffectModifierMode.ADD => items[i] + modifiers[i],
          EffectModifierMode.SUBTRACT => items[i] - modifiers[i],
          _ => items[i]
        };
    }
    public void Integrate(EffectModifierMode mode, Vector3[] items, Vector3[] modifiers)
    {
      int count = Math.Min(items.Length, modifiers.Length);
      for (int i = 0; i < count; i++)
        items[i] = mode switch
        {
          EffectModifierMode.REPLACE => modifiers[i],
          EffectModifierMode.MULTIPLY => Vector3.Scale(items[i], modifiers[i]),
          EffectModifierMode.ADD => items[i] + modifiers[i],
          EffectModifierMode.SUBTRACT => items[i] - modifiers[i],
          _ => items[i]
        };
    }
    public void Integrate(EffectModifierMode mode, Color[] items, Color[] modifiers)
    {
      int count = Math.Min(items.Length, modifiers.Length);
      for (int i = 0; i < count; i++)
        items[i] = mode switch
        {
          EffectModifierMode.REPLACE => modifiers[i],
          EffectModifierMode.MULTIPLY => items[i] * modifiers[i],
          EffectModifierMode.ADD => items[i] + modifiers[i],
          EffectModifierMode.SUBTRACT => items[i] - modifiers[i],
          _ => items[i]
        };
    }

    protected static readonly ProfilerMarker s_ListPrep = new ProfilerMarker("Waterfall.Integrator.ListPrep");
    protected static readonly ProfilerMarker s_Modifiers = new ProfilerMarker("Waterfall.Integrator.Modifiers");
    protected static readonly ProfilerMarker s_Integrate = new ProfilerMarker("Waterfall.Integrator.Integrate");
    protected static readonly ProfilerMarker s_Apply = new ProfilerMarker("Waterfall.Integrator.Apply");

  }

  public abstract class EffectIntegratorTyped<T_Value> : EffectIntegrator
  {
    protected readonly T_Value[] modifierData;
    protected readonly T_Value[] initialValues;
    protected readonly T_Value[] workingValues;

    public EffectIntegratorTyped(WaterfallEffect effect, EffectModifier mod) : base(effect, mod)
    {
      modifierData = new T_Value[xforms.Count];
      initialValues = new T_Value[xforms.Count];
      workingValues = new T_Value[xforms.Count];
    }
  }

  public abstract class EffectIntegrator_Float : EffectIntegratorTyped<float>
  {
    public EffectIntegrator_Float(WaterfallEffect effect, EffectModifier_Float mod) : base(effect, mod) { }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.Integrator_Float.Update");

    public void Update()
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
          ((EffectModifier_Float)mod).Get(controllerData, modifierData);
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
  }

  public abstract class EffectIntegrator_Color : EffectIntegratorTyped<Color>
  {
    public EffectIntegrator_Color(WaterfallEffect effect, EffectModifier_Color mod) : base(effect, mod) { }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.Integrator_Color.Update");

    public void Update()
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
          ((EffectModifier_Color)mod).Get(controllerData, modifierData);
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
  }

  public abstract class EffectIntegrator_Vector3 : EffectIntegratorTyped<Vector3>
  {
    public EffectIntegrator_Vector3(WaterfallEffect effect, EffectModifier_Vector3 mod) : base(effect, mod) { }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.Integrator_Vector3.Update");

    public void Update()
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
          ((EffectModifier_Vector3)mod).Get(controllerData, modifierData);
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
  }
}
