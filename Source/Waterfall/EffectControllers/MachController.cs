using System.Collections.Generic;


namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from atmosphere density
  /// </summary>
  public class MachController : WaterfallController
  {
    public float mach = 0;
    public MachController() { }
    public MachController(ConfigNode node)
    {
      name = "mach";
      linkedTo = "mach";
      node.TryGetValue("name", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      overrideMin = 0f;
      overrideMax = 15f;
    }
    public override List<float> Get()
    {
      if (overridden)
        return new List<float>() { overrideValue };
      return new List<float>() { (float)parentModule.vessel.mach };
    }
  }
}
