using System.Collections.Generic;


namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from atmosphere density
  /// </summary>
  public class MachController : WaterfallController
  {
    public const string ControllerTypeId = "mach";
    public const string DisplayName = "Mach";

    public float mach = 0;

    public MachController()
    {
      name = ControllerTypeId;
      linkedTo = ControllerTypeId;
    }

    public MachController(ConfigNode node) : this()
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
      return new List<float>() { (float)parentModule.vessel.mach };
    }
  }
}