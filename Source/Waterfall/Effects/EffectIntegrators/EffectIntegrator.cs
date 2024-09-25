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
    public void AddModifier(EffectModifier mod)
    {
      handledModifiers.Add(mod);
      if (mod.Controller != null)
      {
        mod.Controller.referencingModifierCount++; // the original code also evaluated controllers from the integrator, so we need to account for that here
      }
    }
    public void RemoveModifier(EffectModifier mod)
    {
      // if this was the last modifier, then this integrator will end up being removed.  Do a final update so that we get the initial values applied again.
      // NOTE this isn't really guaranteed to do anything useful, and we're not currently removing empty integrators anyway
      // see EffectModifier.RemoveFromIntegrator
      if (handledModifiers.Remove(mod) && handledModifiers.Count == 0)
      {
        Update();
      }
    }

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

    // REVIEW: there are some problems here:
    // A) 2 virtual calls might hurt performance - how do we get this down to 1 and keep the code sharing where it should be?
    // This might be overthinking it - it's only 2 virtual calls per integrator
    // Should the common part just be a protected function that the virtual derived classes have to call?
    // This might make profiling markup annoying
    // B) it's a code smell that the bool return value is only meaningful for some of the integrators (float integrators that control visibility)
    // C) It's strange that the boolean for TestIntensity lives in the EffectIntegrator_Float but the threshold and application logic is in the derived classes
    public abstract void Update();
    protected abstract void Apply();

    protected static void Integrate(EffectModifierMode mode, float[] items, float[] modifiers)
    {
      int count = Math.Min(items.Length, modifiers.Length);
      switch (mode)
      {
        case EffectModifierMode.REPLACE:
          for (int i = 0; i < count; i++)
          {
            items[i] = modifiers[i];
          }
          break;

        case EffectModifierMode.MULTIPLY:
          for (int i = 0; i < count; i++)
          {
            items[i] *= modifiers[i];
          }
          break;

        case EffectModifierMode.ADD:
          for (int i = 0; i < count; i++)
          {
            items[i] += modifiers[i];
          }
          break;

        case EffectModifierMode.SUBTRACT:
          for (int i = 0; i < count; i++)
          {
            items[i] -= modifiers[i];
          }
          break;
      }
    }
    public void Integrate(EffectModifierMode mode, Vector2[] items, Vector2[] modifiers)
    {
      int count = Math.Min(items.Length, modifiers.Length);
      switch (mode)
      {
        case EffectModifierMode.REPLACE:
          for (int i = 0; i < count; i++)
          {
            items[i] = modifiers[i];
          }
          break;

        case EffectModifierMode.MULTIPLY:
          for (int i = 0; i < count; i++)
          {
            items[i] = Vector2.Scale(items[i], modifiers[i]);
          }
          break;

        case EffectModifierMode.ADD:
          for (int i = 0; i < count; i++)
          {
            items[i] += modifiers[i];
          }
          break;

        case EffectModifierMode.SUBTRACT:
          for (int i = 0; i < count; i++)
          {
            items[i] -= modifiers[i];
          }
          break;
      }
    }

    protected static void Integrate(EffectModifierMode mode, Vector3[] items, Vector3[] modifiers)
    {
      int count = Math.Min(items.Length, modifiers.Length);
      switch (mode)
      {
        case EffectModifierMode.REPLACE:
          for (int i = 0; i < count; i++)
          {
            items[i] = modifiers[i];
          }
          break;

        case EffectModifierMode.MULTIPLY:
          for (int i = 0; i < count; i++)
          {
            items[i] = Vector3.Scale(items[i], modifiers[i]);
          }
          break;

        case EffectModifierMode.ADD:
          for (int i = 0; i < count; i++)
          {
            items[i] += modifiers[i];
          }
          break;

        case EffectModifierMode.SUBTRACT:
          for (int i = 0; i < count; i++)
          {
            items[i] -= modifiers[i];
          }
          break;
      }
    }

    protected static void Integrate(EffectModifierMode mode, Color[] items, Color[] modifiers)
    {
      int count = Math.Min(items.Length, modifiers.Length);
      switch (mode)
      {
        case EffectModifierMode.REPLACE:
          for (int i = 0; i < count; i++)
          {
            items[i] = modifiers[i];
          }
          break;

        case EffectModifierMode.MULTIPLY:
          for (int i = 0; i < count; i++)
          {
            items[i] *= modifiers[i];
          }
          break;

        case EffectModifierMode.ADD:
          for (int i = 0; i < count; i++)
          {
            items[i] += modifiers[i];
          }
          break;

        case EffectModifierMode.SUBTRACT:
          for (int i = 0; i < count; i++)
          {
            items[i] -= modifiers[i];
          }
          break;
      }
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
    public readonly bool testIntensity;

    public EffectIntegrator_Float(WaterfallEffect effect, EffectModifier_Float mod, bool testIntensity_) : base(effect, mod)
    {
      testIntensity = testIntensity_;
    }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.Integrator_Float.Update");

    public override void Update()
    {
      Update_TestIntensity();
    }
    protected override void Apply()
    {
      Apply_TestIntensity();
    }

    protected abstract bool Apply_TestIntensity();

    public bool Update_TestIntensity()
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
      bool result = Apply_TestIntensity();
      s_Apply.End();

      s_Update.End();

      return result;
    }
  }

  public abstract class EffectIntegrator_Color : EffectIntegratorTyped<Color>
  {
    public EffectIntegrator_Color(WaterfallEffect effect, EffectModifier_Color mod) : base(effect, mod) { }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.Integrator_Color.Update");

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
  public abstract class EffectIntegrator_Vector2 : EffectIntegratorTyped<Vector2>
  {
    public EffectIntegrator_Vector2(WaterfallEffect effect, EffectModifier_Vector2 mod) : base(effect, mod) { }

    protected static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.Integrator_Vector2.Update");

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
          ((EffectModifier_Vector2)mod).Get(controllerData, modifierData);
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
