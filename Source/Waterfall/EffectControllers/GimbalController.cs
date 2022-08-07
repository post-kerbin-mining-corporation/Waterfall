using System.ComponentModel;

namespace Waterfall
{
  [DisplayName("Gimbal")]
  public class GimbalController : WaterfallController
  {
    [Persistent] public string axis = "x";
    private ModuleGimbal gimbalController;

    public GimbalController() : base() { }
    public GimbalController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      values = new float[1];

      gimbalController = host.part.FindModuleImplementing<ModuleGimbal>();

      if (gimbalController == null)
        Utils.LogError("[GimbalController] Could not find gimbal controller on Initialize");
    }

    protected override void UpdateInternal()
    {
      if (gimbalController == null)
      {
        Utils.LogWarning("[GimbalController] Gimbal controller not assigned");
        return;
      }

      if (axis == "x") values[0] = gimbalController.actuationLocal.x / gimbalController.gimbalRangeXP;
      else if (axis == "y") values[0] = gimbalController.actuationLocal.y / gimbalController.gimbalRangeYP;
      else if (axis == "z") values[0] = gimbalController.actuationLocal.z;
    }
  }
}
