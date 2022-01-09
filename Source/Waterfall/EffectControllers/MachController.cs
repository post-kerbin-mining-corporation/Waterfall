using System.Collections.Generic;
using System.ComponentModel;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls from atmosphere density
  /// </summary>
  [DisplayName("Mach")]
  public class MachController : WaterfallController
  {
    public float mach = 0;

    public MachController() { }

    public MachController(ConfigNode node)
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
        return new() { overrideValue };
      return new() { (float)parentModule.vessel.mach };
    }
  }
}