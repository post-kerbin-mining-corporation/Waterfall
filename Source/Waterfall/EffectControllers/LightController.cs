using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Waterfall
{

  /// <summary>
  /// A controller that pulls from throttle settings
  /// </summary>
  [System.Serializable]
  public class LightController : WaterfallController
  {
    public float currentThrottle = 1;
    public string lightName = "";
    Light lightController;

    public LightController(ConfigNode node)
    {
      name = "light";
      linkedTo = "light";
      node.TryGetValue("light", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      lightController = host.GetComponentsInChildren<Light>().ToList().Find(x => x.transform.name == lightName);
      if (lightController == null)
        lightController = host.GetComponentsInChildren<Light>().ToList().First();

      if (lightController == null)
        Utils.LogError("[LightController] Could not find any lights on Initialize");

    }
    public override List<float> Get()
    {

      if (overridden)
        return new List<float>() { overrideValue };

      if (lightController == null)
      {
        Utils.LogWarning("[lightController] Light controller not assigned");
        return new List<float>() { 0f };
      }
      return new List<float>() { lightController.intensity };
    }
  }

}

