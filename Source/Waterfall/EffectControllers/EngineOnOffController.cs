using System;
using System.ComponentModel;
using System.Linq;

namespace Waterfall
{
  [DisplayName("Engine On State")]
  public class EngineOnOffController : WaterfallController
  {
    [Persistent] public string engineID = String.Empty;
    private ModuleEngines engineController;

    public bool zeroOnFlameout = true;

    public EngineOnOffController() : base() { }
    public EngineOnOffController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      values = new float[1];

      engineController = host.GetComponents<ModuleEngines>().FirstOrDefault(x => x.engineID == engineID);
      if (engineController == null)
      {
        Utils.Log($"[EngineOnOffController] Could not find engine ID {engineID}, using first module", LogType.Effects);
        engineController = host.part.FindModuleImplementing<ModuleEngines>();
      }

      if (engineController == null)
        Utils.LogError("[EngineOnOffController] Could not find engine controller on Initialize");
    }

    protected override float UpdateSingleValue()
    {
      if (engineController == null)
      {
        Utils.LogWarning("[EngineOnOffController] Engine controller not assigned");
        return 0f;
      }
      else if (zeroOnFlameout)
      {
        return engineController.isOperational && !engineController.flameout ? 1f : 0f;
      }
      else
      {
        return engineController.isOperational  ? 1f : 0f;
      }
      
    }
  }
}
   
