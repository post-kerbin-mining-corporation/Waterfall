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

    public override void Update()
    {
      //(float)parentModule.vessel.mainBody.GetPressureAtm(parentModule.vessel.altitude) 
      value = Mathf.Pow((float)parentModule.part.atmDensity, Settings.AtmosphereDensityExponent);
    }
  }
}
