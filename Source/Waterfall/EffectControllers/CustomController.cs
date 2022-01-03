using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall
{
  [System.Serializable]
  public class CustomController : WaterfallController
  {
    public const string Name = "custom";

    public CustomController()
    {
      name = Name;
      linkedTo = Name;
    }

    public CustomController(ConfigNode node) : this()
    {
      node.TryGetValue(nameof(name), ref name);
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
    }

    public override string DisplayName => "Custom";

    public override List<float> Get()
    {
      if (overridden)
        return new List<float>() { overrideValue };
      return new List<float>() { value };
    }
  }
}