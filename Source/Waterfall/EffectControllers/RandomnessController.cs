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
    public string noiseType = "random";
    public delegate float NoiseFunction();
    public int seed = 0;
    public float scale = 1f;
    NoiseFunction noiseFunc;

    public RandomnessController() { }
    public RandomnessController(ConfigNode node)
    {
      name = "random";
      linkedTo = "random";
      node.TryGetValue("name", ref name);
      node.TryGetValue("noiseType", ref noiseType);
      node.TryGetValue("range", ref range);
      node.TryGetValue("scale", ref scale);
      // Randomize seed if not specified
      if (!node.TryGetValue("seed", ref seed))
      {
        seed = UnityEngine.Random.Range(0, 10000);
      }
    }
    public override ConfigNode Save()
    {

      ConfigNode c = base.Save();
      c.AddValue("noiseType", noiseType);
      if (noiseType == "random")
        c.AddValue("range", range);
      if (noiseType == "perlin")
      {
        c.AddValue("scale", scale);
        c.AddValue("seed", seed);
      }
      
      return c;
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      if (noiseType == "perlin")
      {
        noiseFunc = new NoiseFunction(PerlinNoise);
      }
      else if (noiseType == "random")
      {
        noiseFunc = new NoiseFunction(RandomNoise);
      }
      else
      {
        noiseFunc = new NoiseFunction(RandomNoise);
      }

    }

    public float RandomNoise()
    {
      return UnityEngine.Random.Range(range.x, range.y);
    }
    public float PerlinNoise()
    {
      return Mathf.PerlinNoise(seed+Time.time, seed + Time.time)*scale;
    }
    public override List<float> Get()
    {


      return new List<float>() { noiseFunc() };
    }
  }

}
