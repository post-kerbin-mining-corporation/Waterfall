using System.Collections.Generic;
using UnityEngine;
using Waterfall.EffectControllers;

namespace Waterfall
{
  /// <summary>
  ///   A generic effect controller
  /// </summary>
  public abstract class WaterfallController
  {
    /// <summary>
    ///   Name of the config node which will store controller type name.
    /// </summary>
    public const string LegacyControllerTypeNodeName = "linkedTo";

    [Persistent] public string name = "unnamedController";

    public ModuleWaterfallFX ParentModule => parentModule;

    public bool awake;

    public bool overridden
    {
      get { return _overridden; }
      set
      {
        _overridden = value;
        if (_overridden)
        {
          Set(overrideValue);
        }
      }
    }
    public float overrideValue
    {
      get {  return _overrideValue; }
      set
      {
        _overrideValue = value;
        if (overridden)
        {
          Set(_overrideValue);
        }
      }
    }
    
    protected bool _overridden;
    protected float _overrideValue;

    public int referencingModifierCount = 0; // NOTE: this is only used for the upgrade pipeline and set on load, it does not get updated as effects are added or removed
    protected float[] values;
    protected ModuleWaterfallFX parentModule;

    public WaterfallController() { }
    public WaterfallController(ConfigNode node) : this()
    {
      ConfigNode.LoadObjectFromConfig(this, node);
    }

    public bool Update()
    {
      awake = overridden;
      
      if (!overridden)
      {
        awake = UpdateInternal();
      }

      return awake;
    }

    protected virtual float UpdateSingleValue() { return values[0]; }

    /// <summary>
    /// Get and store the value of the controller.  Consumers should call Get() to retrieve the data.
    /// </summary>
    protected virtual bool UpdateInternal()
    {
      float newValue = UpdateSingleValue();
      if (Utils.ApproximatelyEqual(newValue, values[0]))
      {
        return false;
      }
      values[0] = newValue;
      return true;
    }

    /// <summary>
    ///   Get the value of the controller.
    /// </summary>
    /// <returns></returns>
    public float[] Get()
    {
      return values;
    }

    /// <summary>
    ///   Saves the controller
    /// </summary>
    /// <param name="host"></param>
    public virtual ConfigNode Save()
    {
      var node = ConfigNode.CreateConfigFromObject(this);
      node.name = EffectControllersMetadata.GetConfigNodeName(GetType());
      return node;
    }

    /// <summary>
    ///   Initializes the controller
    /// </summary>
    /// <param name="host"></param>
    public virtual void Initialize(ModuleWaterfallFX host)
    {
      parentModule = host;
    }

    /// <summary>
    ///   Sets the value of the controller
    /// </summary>
    /// <param name="mode"></param>
    public void Set(float newValue)
    {
      for (int i = 0; i < values.Length; i++)
      {
        values[i] = newValue;
      }
    }

    /// <summary>
    ///   Sets whether this controller is overridden, likely controlled by the UI
    /// </summary>
    /// <param name="mode"></param>
    public void SetOverride(bool mode)
    {
      overridden = mode;
    }

    public virtual void UpgradeToCurrentVersion(Version loadedVersion)
    {

    }
  }
}
