using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waterfall
{

  /// <summary>
  /// A generic controller
  /// </summary>
  public class WaterfallController
  {
    public string name = "unnamedController";
    protected bool overridden = false;
    protected float overrideValue = 0.0f;
    protected ModuleWaterfallFX parentModule;
    public virtual float Get() {
      if (overridden)
        return overrideValue;
      else
        return 0f;
    }
    public virtual void Initialize(ModuleWaterfallFX host) {
      parentModule = host;
    }

    public virtual void SetOverride(bool mode)
    {
      overridden = mode;
    }
    public virtual void SetOverrideValue(float value)
    {
      overrideValue = value;
    }
  }

  /// <summary>
  /// A controller that pulls from a throttle number
  /// </summary>
  [System.Serializable]
  public class ThrottleController : WaterfallController
  {
    public float currentThrottle = 1;
    ModuleEngines engineController;

    public ThrottleController(ConfigNode node)
    {
      name = "throttle";
      node.TryGetValue("name", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      engineController = host.GetComponent<ModuleEngines>();
    }
    public override float Get()
    {
      if (overridden)
        return overrideValue;
      return engineController.requestedThrottle;
    }
  }

  /// <summary>
  /// A controller that pulls from atmosphere density
  /// </summary>
  [System.Serializable]
  public class AtmosphereDensityController : WaterfallController
  {
    public float atmosphereDepth = 1;
    public AtmosphereDensityController(ConfigNode node)
    {
      name = "atmosphereDensity";
      node.TryGetValue("name", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

    }
    public override float Get()
    {
      if (overridden)
        return overrideValue;
      return (float)parentModule.vessel.mainBody.GetPressureAtm(parentModule.vessel.altitude);
    }
  }
}
