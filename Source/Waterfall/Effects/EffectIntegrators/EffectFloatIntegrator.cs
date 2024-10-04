using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

namespace Waterfall
{

  public class EffectFloatIntegrator : EffectIntegrator_Float
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

    private readonly Renderer[] renderers;
    private readonly Material[] materials;
    private float[] lastValues;

    public EffectFloatIntegrator(WaterfallEffect effect, EffectFloatModifier floatMod) : base(effect, floatMod)
    {
      // float specific
      floatName = floatMod.floatName;
      renderers = new Renderer[xforms.Count];
      materials = new Material[xforms.Count];
      lastValues = new float[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        renderers[i] = xforms[i].GetComponent<Renderer>();

        if (renderers[i] == null)
        {
          // TODO: it would be really nice to print the path to the transform that failed, but I don't see an easy way offhand
          Utils.LogError($"Integrator for {floatName} for modifier {floatMod.fxName} in module {effect.parentModule.moduleID} failed to find a renderer on transform {transformName}");
        }
        else if (renderers[i].material.HasProperty(floatPropertyID))
        {
          initialValues[i] = renderers[i].material.GetFloat(floatPropertyID);
          lastValues[i] = initialValues[i];
          materials[i] = renderers[i].material;
        }
        else
        {
          Utils.LogError($"Material {renderers[i].material.name} does not have float property {floatName} for modifier {floatMod.fxName} in module {effect.parentModule.moduleID}");
        }
      }
    }

    protected override void Apply()
    {
      bool anyActive;

      if (testIntensity)
      {
        anyActive = false;
        for (int i = renderers.Length; i-- > 0;)
        {
          float val = workingValues[i];
          float lastValue = lastValues[i];
          if (Utils.ApproximatelyEqual(val, lastValue)) continue;

          bool shouldBeVisible = val >= Settings.MinimumEffectIntensity;
          bool wasVisible = lastValue >= Settings.MinimumEffectIntensity;

          if (wasVisible != shouldBeVisible)
          {
            renderers[i].enabled = shouldBeVisible;
          }

          if (shouldBeVisible)
          {
            materials[i].SetFloat(floatPropertyID, val);
            anyActive = true;
          }

          lastValues[i] = val;
        }
      }
      else
      {
        anyActive = true;
        for (int i = renderers.Length; i-- > 0;)
        {
          float val = workingValues[i];
          float lastValue = lastValues[i];
          if (Utils.ApproximatelyEqual(val, lastValue)) continue;

          materials[i].SetFloat(floatPropertyID, val);
          lastValues[i] = val;
        }
      }

      active = anyActive;
    }
  }
}
