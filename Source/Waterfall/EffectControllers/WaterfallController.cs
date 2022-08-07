using System.Collections.Generic;
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

    public void Update()
    {
      if (!overridden)
      {
        UpdateInternal();
      }
    }

    /// <summary>
    /// Get and store the value of the controller.  Consumers should call Get() to retrieve the data.
    /// </summary>
    protected abstract void UpdateInternal();

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

    /// <summary>
    ///   Sets the override value, not controlled by the game, likely an editor UI
    /// </summary>
    /// <param name="value"></param>
    public void SetOverrideValue(float value)
    {
      overrideValue = value;
    }

    public virtual void UpgradeToCurrentVersion(Version loadedVersion)
    {

    }
  }
}
