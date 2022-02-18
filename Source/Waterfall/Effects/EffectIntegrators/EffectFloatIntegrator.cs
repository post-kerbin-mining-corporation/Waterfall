using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public abstract class EffectIntegrator
  {
    public string transformName;
    protected WaterfallEffect parentEffect;
    protected List<Transform> xforms = new();
    public List<EffectModifier> handledModifiers = new();
    public virtual void AddModifier(EffectModifier mod) => handledModifiers.Add(mod);
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

      handledModifiers.Add(mod);
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
    public string                    floatName;
    protected List<float> initialValues = new();
    protected List<float> workingValues = new();

    private readonly Material[] m;
    private readonly Renderer[] r;

    private readonly bool testIntensity;

    public EffectFloatIntegrator(WaterfallEffect effect, EffectFloatModifier floatMod) : base(effect, floatMod)
    {
      // float specific
      floatName        = floatMod.floatName;
      testIntensity = WaterfallConstants.ShaderPropertyHideFloatNames.Contains(floatName);

      m                  = new Material[xforms.Count];
      r                  = new Renderer[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        r[i] = xforms[i].GetComponent<Renderer>();
        m[i] = r[i].material;
        initialValues.Add(m[i].GetFloat(floatName));
      }
    }

    public override void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      workingValues.Clear();
      workingValues.AddRange(initialValues);

      foreach (var mod in handledModifiers)
      {
        var modResult = (mod as EffectFloatModifier).Get(parentEffect.parentModule.GetControllerValue(mod.controllerName));
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < m.Length; i++)
      {
        if (testIntensity)
        {
          if (r[i].enabled && workingValues[i] < Settings.MinimumEffectIntensity)
            r[i].enabled = false;
          else if (!r[i].enabled && workingValues[i] >= Settings.MinimumEffectIntensity)
            r[i].enabled = true;
        }
        if (r[i].enabled)
          m[i].SetFloat(floatName, workingValues[i]);
      }
    }
  }
}
