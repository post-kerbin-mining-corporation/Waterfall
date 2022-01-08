using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from atmosphere density
  /// </summary>
  [DisplayName("Atmosphere Density")]
  public class AtmosphereDensityController : WaterfallController
  {
    public float atmosphereDepth = 1;

    public AtmosphereDensityController()
    {
    }

    public AtmosphereDensityController(ConfigNode node)
    {
      node.TryGetValue(nameof(name), ref name);
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
    }

    public override List<float> Get()
    {
      if (overridden)
        return new List<float>() { overrideValue };
      return new List<float>()
      {
        Mathf.Pow((float)parentModule.part.atmDensity, Settings.AtmosphereDensityExponent)

        //(float)parentModule.vessel.mainBody.GetPressureAtm(parentModule.vessel.altitude) 
      };
    }
  }
}