using System.Collections.Generic;

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
    public bool overridden;
    public float overrideValue;
    protected float value;
    protected ModuleWaterfallFX parentModule;

    public WaterfallController() { }
    public WaterfallController(ConfigNode node) : this()
    {
      ConfigNode.LoadObjectFromConfig(this, node);
    }

    /// <summary>
    /// Get and store the value of the controller.  Consumers should call Get() to retrieve the data.
    /// </summary>
    public abstract void Update();

    /// <summary>
    ///   Get the value of the controller.
    /// </summary>
    /// <returns></returns>
    public virtual void Get(List<float> output)
    {
      output.Clear();
      output.Add(overridden ? overrideValue : value);
    }

    /// <summary>
    ///   Saves the controller
    /// </summary>
    /// <param name="host"></param>
    public virtual ConfigNode Save()
    {
      return ConfigNode.CreateConfigFromObject(this);
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
    public virtual void Set(float newValue)
    {
      value = newValue;
    }

    /// <summary>
    ///   Sets whether this controller is overridden, likely controlled by the UI
    /// </summary>
    /// <param name="mode"></param>
    public virtual void SetOverride(bool mode)
    {
      overridden = mode;
    }

    /// <summary>
    ///   Sets the override value, not controlled by the game, likely an editor UI
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetOverrideValue(float value)
    {
      overrideValue = value;
    }
  }
}
