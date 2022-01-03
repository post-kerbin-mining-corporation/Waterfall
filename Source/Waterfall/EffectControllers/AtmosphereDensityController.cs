using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from atmosphere density
  /// </summary>
  public class AtmosphereDensityController : WaterfallController
  {
    public const string Name = "atmosphere_density";

    public float atmosphereDepth = 1;

    public AtmosphereDensityController() { }
    public AtmosphereDensityController(ConfigNode node)
    {
      name = Name;
      linkedTo = Name;
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
      return new List<float>() {
        Mathf.Pow((float)parentModule.part.atmDensity, Settings.AtmosphereDensityExponent)
        
      //(float)parentModule.vessel.mainBody.GetPressureAtm(parentModule.vessel.altitude) 
      };
    }
  }
}
