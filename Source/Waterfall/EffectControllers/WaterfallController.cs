using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// A generic effect controller
  /// </summary>
  public abstract class WaterfallController
  {
    public string name = "unnamedController";
    public bool overridden = false;
    public float overrideValue = 0.0f;
    protected float value = 0.0f;
    protected ModuleWaterfallFX parentModule;

    /// <summary>
    ///    Name of the config node which will store value of <see cref="TypeId"/>.
    /// </summary>
    public const string ControllerTypeNodeName = "linkedTo";

    /// <summary>
    ///   Identifies which kind of effect controller this is.
    /// </summary>
    /// <remarks>
    ///   Every controller type is also expected to have constant member similar to <see cref="ThrottleController.ControllerTypeId"/> which is used to automatically bind controller types to UI and serialization logic.
    /// </remarks>
    public abstract string TypeId { get; }

    /// <summary>
    /// Get the value of the controller. 
    /// </summary>
    /// <returns></returns>
    public virtual List<float> Get()
    {
      if (overridden)
        return new List<float>() {overrideValue};
      else
        return new List<float>() {0f};
    }

    /// <summary>
    ///  Saves the controller
    /// </summary>
    /// <param name="host"></param>
    public virtual ConfigNode Save()
    {
      ConfigNode c = new ConfigNode(WaterfallConstants.ControllerNodeName);
      c.AddValue(nameof(name), name);
      c.AddValue(ControllerTypeNodeName, TypeId);
      return c;
    }

    /// <summary>
    ///  Initializes the controller
    /// </summary>
    /// <param name="host"></param>
    public virtual void Initialize(ModuleWaterfallFX host)
    {
      parentModule = host;
    }

    /// <summary>
    /// Sets the value of the controller
    /// </summary>
    /// <param name="mode"></param>
    public virtual void Set(float newValue)
    {
      value = newValue;
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
}