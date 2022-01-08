using System.Collections.Generic;
using System.Linq;

namespace Waterfall
{
  public class GimbalController : WaterfallController
  {
    public const string ControllerTypeId = "gimbal";
    public const string DisplayName = "Gimbal";

    public float atmosphereDepth = 1;
    public string axis = "x";
    ModuleGimbal gimbalController;

    public GimbalController()
    {
      linkedTo = ControllerTypeId;
    }

    public GimbalController(ConfigNode node) : this()
    {
      node.TryGetValue(nameof(axis), ref axis);
      node.TryGetValue(nameof(name), ref name);
    }

    public override ConfigNode Save()
    {
      ConfigNode c = base.Save();
      c.AddValue(nameof(axis), axis);
      return c;
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      gimbalController = host.GetComponents<ModuleGimbal>().ToList().First();

      if (gimbalController == null)
        Utils.LogError("[GimbalController] Could not find gimbal controller on Initialize");
    }

    public override List<float> Get()
    {
      if (overridden)
        return new List<float>() { overrideValue };

      if (gimbalController == null)
      {
        Utils.LogWarning("[GimbalController] Gimbal controller not assigned");
        return new List<float>() { 0f };
      }

      if (axis == "x")
        return new List<float>() { gimbalController.actuationLocal.x / gimbalController.gimbalRangeXP };
      if (axis == "y")
        return new List<float>() { gimbalController.actuationLocal.y / gimbalController.gimbalRangeYP };
      if (axis == "z")
        return new List<float>() { gimbalController.actuationLocal.z };

      return new List<float>() { 0f };
    }
  }
}