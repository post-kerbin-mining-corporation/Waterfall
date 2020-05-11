using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waterfall
{

  /// <summary>
  /// A generic effect controller
  /// </summary>
  public class WaterfallController
  {
    // 
    public string name = "unnamedController";
    protected bool overridden = false;
    protected float overrideValue = 0.0f;
    protected ModuleWaterfallFX parentModule;

    /// <summary>
    /// Get the value of the controller. 
    /// </summary>
    /// <returns></returns>
    public virtual float Get() {
      if (overridden)
        return overrideValue;
      else
        return 0f;
    }

    /// <summary>
    ///  Initialzies the controller
    /// </summary>
    /// <param name="host"></param>
    public virtual void Initialize(ModuleWaterfallFX host) {
      parentModule = host;
    }

    /// <summary>
    /// Sets whether this controller is overridden, likely controlled by the UI
    /// </summary>
    /// <param name="mode"></param>
    public virtual void SetOverride(bool mode)
    {
      overridden = mode;
    }
    /// <summary>
    /// Sets the override value, not controlled by the game, likely an editor UI
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetOverrideValue(float value)
    {
      overrideValue = value;
    }
  }

  /// <summary>
  /// A controller that pulls from throttle settings
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
