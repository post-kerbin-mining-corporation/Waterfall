using System.ComponentModel;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls from atmosphere density
  /// </summary>
  [DisplayName("Mach")]
  public class MachController : WaterfallController
  {
    public MachController() : base() { }
    public MachController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      values = new float[1];
    }

    protected override bool UpdateInternal()
    {
      values[0] = (float) parentModule.vessel.mach;
      return false;
    }
  }
}
