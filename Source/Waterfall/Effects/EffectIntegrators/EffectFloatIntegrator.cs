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
      for (int i = 0; i < roots.Count; i++)
      {
        var t = roots[i];
        if (t.FindDeepChild(transformName) is Transform t1 && t1 != null)
          xforms.Add(t1);
        else
          Utils.LogError($"[EffectIntegrator]: Unable to find transform {mod.transformName} on modifier {mod.fxName}");
      }

      AddModifier(mod);
    }

    public abstract void Update();

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
  }

  public class EffectFloatIntegrator : EffectIntegrator
  {
    private string _floatName;
    private int floatPropertyID;
    public string floatName
    {
      get { return _floatName; }
      set
      {
        _floatName = value;
        floatPropertyID = Shader.PropertyToID(_floatName);
      }
    }
    protected readonly float[] modifierData;
    protected readonly float[] initialValues;
    protected readonly float[] workingValues;

    private readonly Renderer[] r;

    private readonly bool testIntensity;

    public EffectFloatIntegrator(WaterfallEffect effect, EffectFloatModifier floatMod) : base(effect, floatMod)
    {
      // float specific
      floatName        = floatMod.floatName;
      testIntensity = WaterfallConstants.ShaderPropertyHideFloatNames.Contains(floatName);

      r                  = new Renderer[xforms.Count];
      modifierData = new float[xforms.Count];
      initialValues = new float[xforms.Count];
      workingValues = new float[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        r[i] = xforms[i].GetComponent<Renderer>();

        if (r[i] == null)
        {
          // TODO: it would be really nice to print the path to the transform that failed, but I don't see an easy way offhand
          Utils.LogError($"Integrator for {floatName} for modifier {floatMod.fxName} in module {effect.parentModule.moduleID} failed to find a renderer on transform {transformName}");
        }
        else if (r[i].material.HasProperty(floatPropertyID))
        {
          initialValues[i] = r[i].material.GetFloat(floatPropertyID);
        }
        else
        {
          Utils.LogError($"Material {r[i].material.name} does not have float property {floatName} for modifier {floatMod.fxName} in module {effect.parentModule.moduleID}");
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
      Array.Copy(initialValues, workingValues, workingValues.Length);
      s_ListPrep.End();

      s_Modifiers.Begin();
      for (int i = 0; i < handledModifiers.Count; i++)
      {
        var mod = handledModifiers[i];
        if (mod.Controller != null)
        {
          float[] controllerData = mod.Controller.Get();

          ((EffectFloatModifier) mod).Get(controllerData, modifierData);

          s_Integrate.Begin();
          Integrate(mod.effectMode, workingValues, modifierData);
          s_Integrate.End();
        }
      }

      s_Modifiers.End();

      s_Apply.Begin();
      if (testIntensity)
      {
        for (int i = 0; i < r.Length; i++)
        {
          var rend = r[i];
          float val = workingValues[i];
          
          if (rend.enabled && val < Settings.MinimumEffectIntensity)
            rend.enabled = false;
          else if (!rend.enabled && val >= Settings.MinimumEffectIntensity)
            rend.enabled = true;

          if (rend.enabled)
            rend.material.SetFloat(floatPropertyID, val);
        }
      }
      else
      {
        for (int i = 0; i < r.Length; i++)
        {
          var rend = r[i];
          float val = workingValues[i];

          if (rend.enabled)
            rend.material.SetFloat(floatPropertyID, val);
        }
      }
      s_Apply.End();

      s_Update.End();
    }
  }
}
