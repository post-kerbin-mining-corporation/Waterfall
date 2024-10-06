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
    int[] activeTransformIndices;

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
        values = new float[0];
        return;
      }

      List<int> transformIndices = new List<int>();

      for (int i = 0; i < rcsController.thrusterTransforms.Count; ++i)
      {
        Transform t = rcsController.thrusterTransforms[i];
        if (t.gameObject.activeInHierarchy)
        {
          transformIndices.Add(i);
        }
      }

      activeTransformIndices = transformIndices.ToArray();
      values = new float[activeTransformIndices.Length];
    }

    protected override bool UpdateInternal()
    {
      bool awake = false;
      for (int valueIndex = values.Length; valueIndex-- > 0;)
      {
        int transformIndex = activeTransformIndices[valueIndex];
        float newThrottle = rcsController.thrustForces[transformIndex] / rcsController.thrusterPower;
        float oldValue = values[valueIndex];
          
        if (!Utils.ApproximatelyEqual(oldValue, newThrottle))
        {
          float responseRate = newThrottle > oldValue ? responseRateUp : responseRateDown;
          values[valueIndex] = Mathf.MoveTowards(oldValue, newThrottle, responseRate * TimeWarp.deltaTime);
          awake = true;
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
