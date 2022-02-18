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

    [Persistent] public Vector2 range;
    [Persistent] public string noiseType = RandomNoiseName;

    public int seed;
    [Persistent] public float scale = 1f;
    [Persistent] public float minimum;
    [Persistent] public float speed = 1f;
    private NoiseFunction noiseFunc;

    public RandomnessController() : base() { }
    public RandomnessController(ConfigNode node) : base(node)
    {
      // Randomize seed if not specified
      if (!node.TryGetValue(nameof(seed), ref seed))
      {
        seed = Random.Range(0, 10000);
      }
    }

    public override ConfigNode Save()
    {
      var c = base.Save();
      if (noiseType == PerlinNoiseName)
        c.AddValue(nameof(seed), seed);
      return c;
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      if (noiseType == PerlinNoiseName)
        noiseFunc = PerlinNoise;
      else if (noiseType == RandomNoiseName)
        noiseFunc = RandomNoise;
      else
        noiseFunc = RandomNoise;
    }

    public float RandomNoise() => Random.Range(range.x, range.y);

    public float PerlinNoise() => Mathf.PerlinNoise(seed + Time.time * speed, seed + Time.time * speed) * (scale - minimum) + minimum;

    public override void Update()
    {
      value = noiseFunc();
    }
  }
}
