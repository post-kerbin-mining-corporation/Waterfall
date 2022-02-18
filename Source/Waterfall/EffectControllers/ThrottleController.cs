using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls from throttle settings
  /// </summary>
  [Serializable]
  [DisplayName("Throttle")]
  public class ThrottleController : WaterfallController
  {
    public float  currentThrottle  = 1f;
    public float  responseRateUp   = 100f;
    public float  responseRateDown = 100f;
    public string engineID         = String.Empty;

    private ModuleEngines engineController;

    public ThrottleController() : base() { }
    public ThrottleController(ConfigNode node) : base(node)
    {
      node.TryGetValue(nameof(responseRateUp),   ref responseRateUp);
      node.TryGetValue(nameof(responseRateDown), ref responseRateDown);
      node.TryGetValue(nameof(engineID),         ref engineID);
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      engineController = host.GetComponents<ModuleEngines>().FirstOrDefault(x => x.engineID == engineID);
      if (engineController == null)
      {
        Utils.Log($"[ThrottleController] Could not find engine ID {engineID}, using first module");
        engineController = host.part.FindModuleImplementing<ModuleEngines>();
      }

      if (engineController == null)
        Utils.LogError("[ThrottleController] Could not find engine controller on Initialize");
    }

    public override ConfigNode Save()
    {
      var c = base.Save();

      c.AddValue(nameof(engineID),         engineID);
      c.AddValue(nameof(responseRateUp),   responseRateUp);
      c.AddValue(nameof(responseRateDown), responseRateDown);
      return c;
    }

    public override void Update()
    {
      if (engineController == null)
      {
        Utils.LogWarning("[ThrottleController] Engine controller not assigned");
        currentThrottle = 0;
      } 
      else if (!engineController.isOperational)
        currentThrottle = 0f;
      else if (engineController.currentThrottle > currentThrottle)
        currentThrottle = Mathf.MoveTowards(currentThrottle, engineController.currentThrottle, responseRateUp * TimeWarp.deltaTime);
      else
        currentThrottle = Mathf.MoveTowards(currentThrottle, engineController.currentThrottle, responseRateDown * TimeWarp.deltaTime);
      value = currentThrottle;
    }
  }
}
