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
  [DisplayName("Light")]
  public class LightController : WaterfallController
  {
    public  float  currentThrottle = 1;
    public  string lightName       = "";
    private Light  lightController;


    public LightController() { }

    public LightController(ConfigNode node)
    {
      node.TryGetValue(nameof(lightName), ref lightName);
      node.TryGetValue(nameof(name),      ref name);
    }

    public override ConfigNode Save()
    {
      var c = base.Save();
      c.AddValue(nameof(lightName), lightName);
      return c;
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
        return new() { overrideValue };

      if (lightController == null)
      {
        Utils.LogWarning("[lightController] Light controller not assigned");
        return new() { 0f };
      }

      return new() { lightController.intensity };
    }
  }
}