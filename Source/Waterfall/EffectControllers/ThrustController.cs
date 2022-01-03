using System.Collections.Generic;
using System.Linq;

namespace Waterfall
{

  /// <summary>
  /// A controller that pulls from the current engine's thrust. Returns a fractional thrust value
  /// normalized to [0, 1] where 1 corresponds to the max thrust possible under current conditions.
  /// </summary>
  [System.Serializable]
  public class ThrustController : WaterfallController
  {
    public const string Name = "thrust";

    public string engineID = "";
    public float currentThrustFraction;
    ModuleEngines engineController;

    public ThrustController() { }
    public ThrustController(ConfigNode node)
    {
      name = Name;
      linkedTo = Name;
      engineID = string.Empty;
      node.TryGetValue(nameof(name), ref name);
      node.TryGetValue(nameof(engineID), ref engineID);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      engineController = host.GetComponents<ModuleEngines>().ToList().Find(x => x.engineID == engineID);
      if (engineController == null)
      {
        Utils.Log($"[ThrustController] Could not find engine ID {engineID}, using first module");
        engineController = host.GetComponent<ModuleEngines>();
      }

      if (engineController == null)
        Utils.LogError("[ThrustController] Could not find engine controller on Initialize");

    }
    public override ConfigNode Save()
    {
      ConfigNode c = base.Save();

      c.AddValue(nameof(engineID), engineID);
      return c;
    }
    public override List<float> Get()
    {

      if (overridden)
        return new List<float>() { overrideValue };

      if (engineController == null)
      {
        Utils.LogWarning("[ThrustController] Engine controller not assigned");
        return new List<float>() { 0f };
      }

      if (!engineController.isOperational)
        currentThrustFraction = 0f;
      else
      {
        // Thanks to NathanKell for the formula.
        currentThrustFraction = engineController.fuelFlowGui
          / engineController.maxFuelFlow
          / (float)engineController.ratioSum
          * engineController.mixtureDensity
          * engineController.multIsp;
      }

      return new List<float>() { currentThrustFraction };
    }
  }
}
