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
    [Persistent] public string lightName = "";
    private Light  lightController;

    public LightController() : base() { }
    public LightController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      lightController = host.GetComponentsInChildren<Light>().FirstOrDefault(x => x.transform.name == lightName);
      if (lightController == null)
        lightController = host.GetComponentsInChildren<Light>().FirstOrDefault();

      if (lightController == null)
        Utils.LogError("[LightController] Could not find any lights on Initialize");
    }

    public override void Update()
    {
      if (lightController == null)
        Utils.LogWarning("[lightController] Light controller not assigned");

      value = lightController != null ? lightController.intensity : 0;
    }
  }
}
