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
    ModuleEnginesFX engineController;

    public ThrottleController(ConfigNode node)
    {
      name = "throttle";
      node.TryGetValue("name", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      if (host.engineID != "")
        engineController = host.GetComponents<ModuleEnginesFX>().ToList().Find(x => x.engineID == host.engineID);
      else
        engineController = host.GetComponent<ModuleEnginesFX>();
    }
    public override float Get()
    {

      if (overridden)
        return overrideValue;
      return engineController.requestedThrottle;
    }
  }

}
