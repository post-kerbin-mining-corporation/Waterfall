using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Waterfall
{
  [Serializable]
  [DisplayName("Custom")]
  public class CustomController : WaterfallController
  {
    public CustomController() { }

    public CustomController(ConfigNode node)
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