using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls from RCS throttle
  /// </summary>
  [Serializable]
  [DisplayName("RCS")]
  public class RCSController : WaterfallController
  {
    public  List<float> currentThrottle;
    public  float       responseRateUp        = 100f;
    public  float       responseRateDown      = 100f;
    public  string      thrusterTransformName = String.Empty;
    private ModuleRCSFX rcsController;

    public RCSController() : base() { }
    public RCSController(ConfigNode node) : base(node)
    {
      node.TryGetValue(nameof(responseRateUp),        ref responseRateUp);
      node.TryGetValue(nameof(responseRateDown),      ref responseRateDown);
      node.TryGetValue(nameof(thrusterTransformName), ref thrusterTransformName);
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      rcsController = host.GetComponents<ModuleRCSFX>().FirstOrDefault(x => x.thrusterTransformName == thrusterTransformName);
      if (rcsController == null)
        rcsController = host.part.FindModuleImplementing<ModuleRCSFX>();

      if (rcsController == null)
      {
        Utils.LogError("[RCSController] Could not find ModuleRCSFX on Initialize");
        return;
      }

      currentThrottle = new(rcsController.thrusterTransforms.Count);
      for (int i = 0; i < rcsController.thrusterTransforms.Count; i++)
      {
        currentThrottle.Add(0f);
      }
    }

    public override ConfigNode Save()
    {
      var c = base.Save();

      c.AddValue(nameof(responseRateUp),        responseRateUp);
      c.AddValue(nameof(responseRateDown),      responseRateDown);
      c.AddValue(nameof(thrusterTransformName), thrusterTransformName);
      return c;
    }

    public override void Update()
    {
      if (rcsController == null)
      {
        Utils.LogWarning("[RCSController] RCS controller not assigned");
        value = 0;
        return;
      }
      if (!overridden)
        for (int i = 0; i < rcsController.thrusterTransforms.Count; i++)
        {
          float newThrottle = rcsController.thrustForces[i] / rcsController.thrusterPower;
          float responseRate = newThrottle > currentThrottle[i] ? responseRateUp : responseRateDown;
          currentThrottle[i] = Mathf.MoveTowards(currentThrottle[i], newThrottle, responseRate * TimeWarp.deltaTime);
        }
    }

    public override void Get(List<float> output)
    {
      if (rcsController == null)
      {
        base.Get(output);
        return;
      }

      output.Clear();
      for (int i = 0; i < currentThrottle.Count; i++)
        output.Add(overridden ? overrideValue : currentThrottle[i]);
    }
  }
}
