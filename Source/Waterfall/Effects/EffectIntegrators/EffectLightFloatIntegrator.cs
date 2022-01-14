using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectLightFloatIntegrator : EffectIntegrator
  {
    public string                         floatName;
    public List<EffectLightFloatModifier> handledModifiers;

    private readonly Light[]     l;
    private readonly List<float> initialFloatValues;

    private readonly bool testIntensity;

    public EffectLightFloatIntegrator(WaterfallEffect effect, EffectLightFloatModifier floatMod)
    {
      Utils.Log(String.Format("[EffectLightFloatIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, floatMod.fxName), LogType.Modifiers);
      xforms        = new();
      transformName = floatMod.transformName;
      parentEffect  = effect;
      var roots = parentEffect.GetModelTransforms();
      foreach (var t in roots)
      {
        var t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectLightFloatIntegrator]: Unable to find transform {0} on modifier {1}", transformName, floatMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


      // float specific
      floatName        = floatMod.floatName;
      handledModifiers = new();
      handledModifiers.Add(floatMod);

      foreach (string nm in WaterfallConstants.ShaderPropertyHideFloatNames)
      {
        if (floatName == "Intensity")
          testIntensity = true;
      }

      initialFloatValues = new();

      l = new Light[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();

        if (floatName == "Intensity")
          initialFloatValues.Add(l[i].intensity);
        if (floatName == "Range")
          initialFloatValues.Add(l[i].range);
        if (floatName == "SpotAngle")
          initialFloatValues.Add(l[i].spotAngle);
      }
    }

    public void AddModifier(EffectLightFloatModifier newMod)
    {
      handledModifiers.Add(newMod);
    }

    public void RemoveModifier(EffectLightFloatModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }

    public void Update()
    {
      if (Settings.EnableLights)
      {
        float lightBaseScale = parentEffect.TemplateScaleOffset.x;
        if (handledModifiers.Count > 0)
        {
          var applyValues = initialFloatValues;
          foreach (var floatMod in handledModifiers)
          {
            var modResult = floatMod.Get(parentEffect.parentModule.GetControllerValue(floatMod.controllerName));

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

          for (int i = 0; i < l.Length; i++)
          {
            applyValues[i] *= lightBaseScale;
            if (testIntensity)
            {
              if (l[i].enabled && applyValues[i] < Settings.MinimumLightIntensity)
              {
                l[i].enabled = false;
              }
              else if (!l[i].enabled && applyValues[i] >= Settings.MinimumLightIntensity)
              {
                l[i].enabled = true;

                UpdateFloats(l[i], applyValues[i]);
              }
              else if (l[i].enabled && applyValues[i] >= Settings.MinimumLightIntensity)
              {
                UpdateFloats(l[i], applyValues[i]);
              }
            }
            else
            {
              UpdateFloats(l[i], applyValues[i]);
            }
          }
        }
      }
    }

    protected void UpdateFloats(Light l, float f)
    {
      if (floatName == "Intensity")
        l.intensity = f;
      if (floatName == "Range")
      {
        l.range = f;
      }

      if (floatName == "SpotAngle")
        l.spotAngle = f;
    }
  }
}