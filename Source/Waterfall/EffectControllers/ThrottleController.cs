using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waterfall
{

  /// <summary>
  /// A controller that pulls from throttle settings
  /// </summary>
  [System.Serializable]
  public class ThrottleController : WaterfallController
  {
    public float currentThrottle = 1;
    ModuleEngines engineController;

    public ThrottleController(ConfigNode node)
    {
      name = "throttle";
      node.TryGetValue("name", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      
      engineController = host.GetComponents<ModuleEngines>().ToList().Find(x => x.engineID == host.engineID);
      if (engineController == null)
        engineController = host.GetComponent<ModuleEngines>();

      if (engineController == null)
        Utils.LogError("[ThrottleController] Could not find engine controller on Initialize");

    }
    public override float Get()
    {

      if (overridden)
        return overrideValue;

      if (engineController == null)
      {
        Utils.LogWarning("[ThrottleController] Engine controller not assigned");
        return 0f;
      }
      return engineController.requestedThrottle;
    }
  }

}
