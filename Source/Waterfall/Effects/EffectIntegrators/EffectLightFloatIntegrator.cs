using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  public class EffectLightFloatIntegrator : EffectIntegrator_Float
  {
    public string                         floatName;

    private readonly Light[]     l;

    private readonly bool testIntensity;

    public EffectLightFloatIntegrator(WaterfallEffect effect, EffectLightFloatModifier floatMod) : base(effect, floatMod)
    {
      // light-float specific
      floatName = floatMod.floatName;
      testIntensity = WaterfallConstants.ShaderPropertyHideFloatNames.Contains(floatName);

      l = new Light[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();

        if (floatName == "Intensity") initialValues[i] = l[i].intensity;
        else if (floatName == "Range") initialValues[i] = l[i].range;
        else if (floatName == "SpotAngle") initialValues[i] = l[i].spotAngle;
      }
    }

    protected override void Apply()
    {
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
