using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from atmosphere density
  /// </summary>
  [System.Serializable]
  public class AtmosphereDensityController : WaterfallController
  {
    public float atmosphereDepth = 1;

    public AtmosphereDensityController(ConfigNode node)
    {
      name = "atmosphereDensity";
      linkedTo = "atmosphere_density";
      node.TryGetValue("name", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

    }
    public override List<float> Get()
    {
      if (overridden)
        return new List<float>() { overrideValue };
      return new List<float>() { (float)parentModule.vessel.mainBody.GetPressureAtm(parentModule.vessel.altitude) };
    }
  }
}
