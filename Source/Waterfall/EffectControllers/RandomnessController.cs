using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Waterfall
{
  /// <summary>
  ///   A controller that generates randomness
  /// </summary>
  [Serializable]
  [DisplayName("Randomness")]
  public class RandomnessController : WaterfallController
  {
    public delegate float NoiseFunction();

    public const string PerlinNoiseName = "perlin";
    public const string RandomNoiseName = "random";

    public Vector2 range;
    public string  noiseType = RandomNoiseName;

    public  int           seed;
    public  float         scale = 1f;
    public  float         minimum;
    public  float         speed = 1f;
    private NoiseFunction noiseFunc;

    public RandomnessController() : base() { }
    public RandomnessController(ConfigNode node) : base(node)
    {
      node.TryGetValue(nameof(noiseType), ref noiseType);
      node.TryGetValue(nameof(range),     ref range);
      node.TryGetValue(nameof(scale),     ref scale);
      node.TryGetValue(nameof(minimum),   ref minimum);
      node.TryGetValue(nameof(speed),     ref speed);
      // Randomize seed if not specified
      if (!node.TryGetValue(nameof(seed), ref seed))
      {
        seed = Random.Range(0, 10000);
      }
    }

    public override ConfigNode Save()
    {
      var c = base.Save();
      c.AddValue(nameof(noiseType), noiseType);

      if (noiseType == RandomNoiseName)
        c.AddValue(nameof(range), range);

      if (noiseType == PerlinNoiseName)
      {
        c.AddValue(nameof(scale),   scale);
        c.AddValue(nameof(minimum), minimum);
        c.AddValue(nameof(speed),   speed);
        c.AddValue(nameof(seed),    seed);
      }

      return c;
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      if (noiseType == PerlinNoiseName)
      {
        noiseFunc = PerlinNoise;
      }
      else if (noiseType == RandomNoiseName)
      {
        noiseFunc = RandomNoise;
      }
      else
      {
        noiseFunc = RandomNoise;
      }
    }

    public float RandomNoise() => Random.Range(range.x, range.y);

    public float PerlinNoise() => Mathf.PerlinNoise(seed + Time.time * speed, seed + Time.time * speed) * (scale - minimum) + minimum;

    public override void Update()
    {
      value = noiseFunc();
    }
  }
}
