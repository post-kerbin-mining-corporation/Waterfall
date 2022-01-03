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
    public const string Name = "random";

    private const string PerlinNoiseName = "perlin";
    private const string RandomNoiseName = "random";
    
    public Vector2 range = new Vector2();
    public string noiseType = RandomNoiseName;
    public delegate float NoiseFunction();
    public int seed = 0;
    public float scale = 1f;
    public float minimum = 0f;
    public float speed = 1f;
    NoiseFunction noiseFunc;

    public RandomnessController() { }
    public RandomnessController(ConfigNode node)
    {
      name = Name;
      linkedTo = Name;
      node.TryGetValue(nameof(name), ref name);
      node.TryGetValue(nameof(noiseType), ref noiseType);
      node.TryGetValue(nameof(range), ref range);
      node.TryGetValue(nameof(scale), ref scale);
      node.TryGetValue(nameof(minimum), ref minimum);
      node.TryGetValue(nameof(speed), ref speed);
      // Randomize seed if not specified
      if (!node.TryGetValue(nameof(seed), ref seed))
      {
        seed = UnityEngine.Random.Range(0, 10000);
      }
    }
    public override ConfigNode Save()
    {

      ConfigNode c = base.Save();
      c.AddValue(nameof(noiseType), noiseType);

      if (noiseType == RandomNoiseName)
        c.AddValue(nameof(range), range);

      if (noiseType == PerlinNoiseName)
      {
        c.AddValue(nameof(scale), scale);
        c.AddValue(nameof(minimum), minimum);
        c.AddValue(nameof(speed), speed);
        c.AddValue(nameof(seed), seed);
      }
      
      return c;
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      if (noiseType == PerlinNoiseName)
      {
        noiseFunc = new NoiseFunction(PerlinNoise);
      }
      else if (noiseType == RandomNoiseName)
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
      
      return Mathf.PerlinNoise(seed+Time.time*speed, seed + Time.time * speed) *(scale-minimum)+minimum;
    }

    public override string DisplayName => "Randomness";

    public override List<float> Get()
    {
      if (overridden)
        return new List<float>() { overrideValue };


      return new List<float>() { noiseFunc() };
    }
  }

}
