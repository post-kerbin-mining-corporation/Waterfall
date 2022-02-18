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
    public override void Update()
    {
      value = (float) parentModule.vessel.mach;
    }
  }
}
