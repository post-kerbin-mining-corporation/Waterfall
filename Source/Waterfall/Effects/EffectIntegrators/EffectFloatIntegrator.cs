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

    protected override void Apply()
    {
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
    }
  }
}
