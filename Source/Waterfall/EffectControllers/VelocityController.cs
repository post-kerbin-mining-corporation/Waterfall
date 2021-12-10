using System.Collections.Generic;


namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from vessel velocity magnitude. has surface and orbit modes
  /// </summary>
  public class VelocityController : WaterfallController
  {
    public int mode = 0;
    public VelocityController() { }
    public VelocityController(ConfigNode node)
    {
      name = "velocity";
      linkedTo = "velocity";
      node.TryGetValue("name", ref name);
      node.TryGetValue("mode", ref mode);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      overrideMin = 0f;
      overrideMax = 10000f;
    }

    public override ConfigNode Save()
    {
      ConfigNode c = base.Save();

      c.AddValue("mode", mode);
      return c;
    }
    public override List<float> Get()
    {
      if (overridden)
        return new List<float>() { overrideValue };
      if (mode == 0)
        return new List<float>() { (float)parentModule.vessel.srf_velocity.magnitude };
      else
        return new List<float>() { (float)parentModule.vessel.obt_velocity.magnitude };
    }
  }
}
