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
    [Persistent] public float responseRateUp         = 100f;
    [Persistent] public float responseRateDown       = 100f;
    [Persistent] public string thrusterTransformName = String.Empty;
    private ModuleRCSFX rcsController;

    public RCSController() : base() { }
    public RCSController(ConfigNode node) : base(node) { }

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

      values = new float[rcsController.thrusterTransforms.Count];
    }

    protected override bool UpdateInternal()
    {
      bool awake = false;
      if (rcsController != null)
      {
        for (int i = 0; i < values.Length; i++)
        {
          float newThrottle = rcsController.thrustForces[i] / rcsController.thrusterPower;
          float responseRate = newThrottle > values[i] ? responseRateUp : responseRateDown;
          values[i] = Mathf.MoveTowards(values[i], newThrottle, responseRate * TimeWarp.deltaTime);
          awake = awake || newThrottle != 0;
        }
      }

      return awake;
    }

    public override void UpgradeToCurrentVersion(Version loadedVersion)
    {
      base.UpgradeToCurrentVersion(loadedVersion);

      if (loadedVersion < Version.FixedRampRates)
      {
        responseRateDown *= Math.Max(1, referencingModifierCount);
        responseRateUp *= Math.Max(1, referencingModifierCount);
      }
    }
  }
}
