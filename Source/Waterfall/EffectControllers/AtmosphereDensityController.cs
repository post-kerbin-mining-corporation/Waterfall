using System;
using System.ComponentModel;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls from atmosphere density
  /// </summary>
  [DisplayName("Atmosphere Density")]
  public class AtmosphereDensityController : WaterfallController
  {
    public AtmosphereDensityController() : base() { }
    public AtmosphereDensityController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      values = new float[1];
    }

    protected override float UpdateSingleValue()
    {
      //(float)parentModule.vessel.mainBody.GetPressureAtm(parentModule.vessel.altitude) 
      return (float)Math.Pow(parentModule.part.atmDensity, Settings.AtmosphereDensityExponent);
    }
  }
}
