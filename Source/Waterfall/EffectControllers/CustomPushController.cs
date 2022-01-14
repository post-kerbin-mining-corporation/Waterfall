using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Waterfall
{
  /// <summary>
  ///    Custom push-based controller.
  ///    Other mods can access it and use <see cref="WaterfallController.SetOverride"/> to provide value to this controller.
  /// </summary>
  [Serializable]
  [DisplayName("Custom")]
  public class CustomPushController : WaterfallController
  {
    public CustomPushController() { }

    public CustomPushController(ConfigNode node)
    {
      node.TryGetValue(nameof(name), ref name);
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
    }

    public override List<float> Get()
    {
      if (overridden)
        return new() { overrideValue };
      return new() { value };
    }
  }
}