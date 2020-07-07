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
    public CustomController(ConfigNode node)
    {
      name = "throttle";
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
      return value;
    }
  }
}
