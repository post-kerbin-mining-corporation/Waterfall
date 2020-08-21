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

    public override void SetupController(ConfigNode node)
    {
      name = "throttle";
      linkedTo = "throttle";
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
    public override List<float> Get()
    {

      if (overridden)
        return new List<float>() { overrideValue };

      if (engineController == null)
      {
        Utils.LogWarning("[ThrottleController] Engine controller not assigned");
        return new List<float>() { 0f };
      }
      return new List<float>() { engineController.currentThrottle };
    }
  }

}
