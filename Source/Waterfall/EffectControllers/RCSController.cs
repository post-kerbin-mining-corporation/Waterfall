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
    public const string ControllerTypeId = "rcs";
    public const string DisplayName = "RCS";

    public List<float> currentThrottle;
    public float responseRateUp = 100f;
    public float responseRateDown = 100f;
    public string thrusterTransformName = string.Empty;
    ModuleRCSFX rcsController;

    public RCSController()
    {
      linkedTo = ControllerTypeId;
    }

    public RCSController(ConfigNode node) : this()
    {
      node.TryGetValue(nameof(name), ref name);
      node.TryGetValue(nameof(responseRateUp), ref responseRateUp);
      node.TryGetValue(nameof(responseRateDown), ref responseRateDown);
      node.TryGetValue(nameof(thrusterTransformName), ref thrusterTransformName);
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

      c.AddValue(nameof(responseRateUp), responseRateUp);
      c.AddValue(nameof(responseRateDown), responseRateDown);
      c.AddValue(nameof(thrusterTransformName), thrusterTransformName);
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