using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{

  /// <summary>
  /// A controller that generates randomness
  /// </summary>
  [System.Serializable]
  public class RandomnessController : WaterfallController
  {
    public Vector2 range = new Vector2();


    public override void SetupController(ConfigNode node)
    {
      name = "random";
      linkedTo = "random";
      node.TryGetValue("name", ref name);
      node.TryGetValue("range", ref range);
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
    }

    public override List<float> Get()
    {
      
      
      return new List<float>() { UnityEngine.Random.Range(range.x, range.y) };
    }
  }

}
