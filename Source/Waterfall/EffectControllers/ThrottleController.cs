using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{

  /// <summary>
  /// A controller that pulls from throttle settings
  /// </summary>
  [System.Serializable]
  public class ThrottleController : WaterfallController
  {
    public float currentThrottle = 1f;
    public float responseRateUp = 100f;
    public float responseRateDown = 100f;
    public string engineID = "";
    ModuleEngines engineController;

    public ThrottleController() { }
    public ThrottleController(ConfigNode node)
    {
      name = "throttle";
      linkedTo = "throttle";
      engineID = "";
      node.TryGetValue("name", ref name);
      node.TryGetValue("responseRateUp", ref responseRateUp);
      node.TryGetValue("responseRateDown", ref responseRateDown);
      node.TryGetValue("engineID", ref engineID);

    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      
      engineController = host.GetComponents<ModuleEngines>().ToList().Find(x => x.engineID == engineID);
      if (engineController == null)
      {
        Utils.Log($"[ThrottleController] Could not find engine ID {engineID}, using first module");
        engineController = host.GetComponent<ModuleEngines>();
      }

      if (engineController == null)
        Utils.LogError("[ThrottleController] Could not find engine controller on Initialize");

    }
    public override ConfigNode Save()
    {
      ConfigNode c = base.Save();

      c.AddValue("engineID", engineID);
      c.AddValue("responseRateUp", responseRateUp);
      c.AddValue("responseRateDown", responseRateDown);
      return c;
    }
    public override List<float> Get()
    {

      if (overridden)
        return new List<float>() { overrideValue };

      if (engineController == null)
      {
        Utils.LogWarning("[ThrottleController] Engine controller not assigned");
        return new List<float>() { 0f };
      }


      if (!engineController.isOperational)
        currentThrottle = 0f;
      else
      {
        if (engineController.currentThrottle > currentThrottle)
        {
          currentThrottle = Mathf.MoveTowards(currentThrottle, engineController.currentThrottle, responseRateUp * TimeWarp.deltaTime );
        }
        else
        {
          currentThrottle = Mathf.MoveTowards(currentThrottle, engineController.currentThrottle, responseRateDown * TimeWarp.deltaTime);
        }
        
      }

      return new List<float>() { currentThrottle };
    }
  }

}
