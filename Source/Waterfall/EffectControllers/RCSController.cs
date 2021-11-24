using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// A controller that pulls from RCS throttle
  /// </summary>
  [Serializable]
  public class RCSController : WaterfallController
  {
    public List<float> currentThrottle;
    public float responseRateUp = 100f;
    public float responseRateDown = 100f;
    public string thrusterTransformName = "";
    ModuleRCSFX rcsController;
    public RCSController() { }
    public RCSController(ConfigNode node)
    {
      name = "rcs";
      linkedTo = "rcs";
      node.TryGetValue("name", ref name);
      node.TryGetValue("responseRateUp", ref responseRateUp);
      node.TryGetValue("responseRateDown", ref responseRateDown);
      node.TryGetValue("thrusterTransformName", ref thrusterTransformName);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      rcsController = host.GetComponents<ModuleRCSFX>().ToList().Find(x => x.thrusterTransformName == thrusterTransformName);
      if (rcsController == null)
        rcsController = host.GetComponent<ModuleRCSFX>();

      if (rcsController == null)
      {
        Utils.LogError("[RCSController] Could not find ModuleRCSFX on Initialize");
        return;
      }

      currentThrottle = new List<float>(rcsController.thrusterTransforms.Count);
      for (int i = 0; i < rcsController.thrusterTransforms.Count; i++)
      {
        currentThrottle.Add(0f);
      }
    }

    public override ConfigNode Save()
    {
      ConfigNode c = base.Save();

      c.AddValue("responseRateUp", responseRateUp);
      c.AddValue("responseRateDown", responseRateDown);
      c.AddValue("thrusterTransformName", thrusterTransformName);
      return c;
    }
    public override List<float> Get()
    {
      if (rcsController == null)
      {
        Utils.LogWarning("[RCSController] RCS controller not assigned");
        return new List<float>() { 0f };
      }

      if (overridden)
      {
        var overrideValues = new List<float>(rcsController.thrusterTransforms.Count);
        for (int i = 0; i < rcsController.thrusterTransforms.Count; i++)
        {
          overrideValues.Add(overrideValue);
        }
        return overrideValues;
      }

      for (int i = 0; i < currentThrottle.Count; i++)
      {
        var newThrottle = rcsController.thrustForces[i] / rcsController.thrusterPower;
        var responseRate = newThrottle > currentThrottle[i] ? responseRateUp : responseRateDown;
        currentThrottle[i] = Mathf.MoveTowards(currentThrottle[i], newThrottle, responseRate * TimeWarp.deltaTime);
      }

      return new List<float>(currentThrottle);
    }
  }
}
