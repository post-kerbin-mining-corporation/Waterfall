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

      values = new float[1];

      lightController = host.GetComponentsInChildren<Light>().FirstOrDefault(x => x.transform.name == lightName);
      if (lightController == null)
        lightController = host.GetComponentsInChildren<Light>().FirstOrDefault();

      if (lightController == null)
        Utils.LogError("[LightController] Could not find any lights on Initialize");
    }

    protected override bool UpdateInternal()
    {
      bool awake = false;
      if (lightController != null)
      {
        values[0] = lightController.intensity;
        awake = values[0] > 0;
      }

      return awake;
    }
  }
}
