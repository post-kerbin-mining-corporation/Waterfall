using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

namespace Waterfall
{
  public abstract class EffectIntegrator
  {
    public string transformName;
    protected WaterfallEffect parentEffect;
    protected List<Transform> xforms = new();
    protected List<float> controllerData = new();
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

    public abstract void Update();

    public void Integrate(EffectModifierMode mode, List<float> items, List<float> modifiers)
    {
      int count = Math.Min(items.Count, modifiers.Count);
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
    public void Integrate(EffectModifierMode mode, List<Vector3> items, List<Vector3> modifiers)
    {
      int count = Math.Min(items.Count, modifiers.Count);
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
    public void Integrate(EffectModifierMode mode, List<Color> items, List<Color> modifiers)
    {
      int count = Math.Min(items.Count, modifiers.Count);
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
  }

  public class EffectFloatIntegrator : EffectIntegrator
  {
    public string floatName;
    protected readonly List<float> modifierData = new();
    protected readonly List<float> initialValues = new();
    protected readonly List<float> workingValues = new();

    private readonly Renderer[] r;

    private readonly bool testIntensity;

    public EffectFloatIntegrator(WaterfallEffect effect, EffectFloatModifier floatMod) : base(effect, floatMod)
    {
      // float specific
      floatName        = floatMod.floatName;
      testIntensity = WaterfallConstants.ShaderPropertyHideFloatNames.Contains(floatName);

      r                  = new Renderer[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        r[i] = xforms[i].GetComponent<Renderer>();
        try
        {
          initialValues.Add(r[i].material.GetFloat(floatName));
        }
        catch (Exception e)
        {
          Utils.LogError($"Material {r[i].material.name} failed to get float {floatName} for modifier {floatMod.fxName} in module {effect.parentModule.moduleID};\n{e}");
        }
      }
    }

    private static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.FloatIntegrator.Update");
    private static readonly ProfilerMarker s_ListPrep = new ProfilerMarker("Waterfall.FloatIntegrator.ListPrep");
    private static readonly ProfilerMarker s_Modifiers = new ProfilerMarker("Waterfall.FloatIntegrator.Modifiers");
    private static readonly ProfilerMarker s_Integrate = new ProfilerMarker("Waterfall.FloatIntegrator.Integrate");
    private static readonly ProfilerMarker s_Apply = new ProfilerMarker("Waterfall.FloatIntegrator.ApplyResult");

    public override void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      s_Update.Begin();

      s_ListPrep.Begin();
      workingValues.Clear();
      workingValues.AddRange(initialValues);
      s_ListPrep.End();

      s_Modifiers.Begin();
      foreach (var mod in handledModifiers)
      {
        mod.Controller?.Get(controllerData);

        List<float> modResult;
        modResult = (mod as EffectFloatModifier).Get(controllerData, modifierData);

        s_Integrate.Begin();
        Integrate(mod.effectMode, workingValues, modResult);
        s_Integrate.End();
      }
      s_Modifiers.End();

      s_Apply.Begin();
      for (int i = 0; i < r.Length; i++)
      {
        var rend = r[i];
        float val = workingValues[i];
        if (testIntensity)
        {
          if (rend.enabled && val < Settings.MinimumEffectIntensity)
            rend.enabled = false;
          else if (!rend.enabled && val >= Settings.MinimumEffectIntensity)
            rend.enabled = true;
        }
        if (rend.enabled)
          rend.material.SetFloat(floatName, val);
      }
      s_Apply.End();
      s_Update.End();
    }
  }
}
