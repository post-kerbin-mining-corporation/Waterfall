using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectLightFloatIntegrator : EffectIntegrator
  {
    public string                         floatName;
    protected readonly float[] modifierData;
    protected readonly float[] initialValues;
    protected readonly float[] workingValues;

    private readonly Light[]     l;

    private readonly bool testIntensity;

    public EffectLightFloatIntegrator(WaterfallEffect effect, EffectLightFloatModifier floatMod) : base(effect, floatMod)
    {
      // light-float specific
      floatName = floatMod.floatName;
      testIntensity = WaterfallConstants.ShaderPropertyHideFloatNames.Contains(floatName);

      l = new Light[xforms.Count];
      modifierData = new float[xforms.Count];
      initialValues = new float[xforms.Count];
      workingValues = new float[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();

        if (floatName == "Intensity") initialValues[i] = l[i].intensity;
        else if (floatName == "Range") initialValues[i] = l[i].range;
        else if (floatName == "SpotAngle") initialValues[i] = l[i].spotAngle;
      }
    }

    public override void Update()
    {
      if (!Settings.EnableLights || handledModifiers.Count == 0)
        return;

      Array.Copy(initialValues, workingValues, l.Length);

      foreach (var mod in handledModifiers)
      {
        if (mod.Controller != null)
        {
          float[] controllerData = mod.Controller.Get();
          ((EffectLightFloatModifier)mod).Get(controllerData, modifierData);
          Integrate(mod.effectMode, workingValues, modifierData);
        }
      }

      float lightBaseScale = parentEffect.TemplateScaleOffset.x;
      for (int i = 0; i < l.Length; i++)
      {
        var light = l[i];
        float value = workingValues[i] * lightBaseScale;
        if (testIntensity)
        {
          if (light.enabled && value < Settings.MinimumLightIntensity)
            light.enabled = false;
          else if (!light.enabled && value >= Settings.MinimumLightIntensity)
            light.enabled = true;
        }
        if (light.enabled)
          UpdateFloats(light, value);
      }
    }

    protected void UpdateFloats(Light l, float f)
    {
      if (floatName == "Intensity") l.intensity = f;
      else if (floatName == "Range") l.range = f;
      else if (floatName == "SpotAngle") l.spotAngle = f;
    }
  }
}
