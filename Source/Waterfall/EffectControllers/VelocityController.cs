using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from vessel velocity magnitude. has surface and orbit modes
  /// </summary>
  [Serializable]
  [DisplayName("Velocity")]
  public class VelocityController : WaterfallController
  {
    public int mode = 0;
    public VelocityController() : base() { }
    public VelocityController(ConfigNode node) : base(node) { }
    
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      values = new float[1];
    }
    protected override float UpdateSingleValue()
    {
      if (mode == 0)
        return (float)parentModule.vessel.srf_velocity.magnitude ;
      else
        return (float)parentModule.vessel.obt_velocity.magnitude ;
    }
  }
}
