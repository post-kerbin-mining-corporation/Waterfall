using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class RandomnessControllerUIOptions : DefaultEffectControllerUIOptions<RandomnessController>
  {
    private readonly string[]    randTypes = { RandomnessController.RandomNoiseName, RandomnessController.PerlinNoiseName };

    private readonly string[] randomStrings = new string[4];
    private          int      randFlag;
    private          Vector2  randomRange;
    private          int      perlinSeed;
    private          float    perlinMin;
    private          float    perlinScale = 1f;
    private          float    perlinSpeed = 1f;
    private bool randomSeed = false;

    public RandomnessControllerUIOptions() { }

    public override void DrawOptions()
    {
      GUILayout.Label("Random type");
      randFlag = GUILayout.SelectionGrid(randFlag, randTypes, Mathf.Min(randTypes.Length, 4), UIResources.GetStyle("radio_text_button"));

      if (randTypes[randFlag] == RandomnessController.RandomNoiseName)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Min/Max", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));

        randomStrings[0] = GUILayout.TextArea(randomStrings[0], GUILayout.MaxWidth(60f));
        randomStrings[1] = GUILayout.TextArea(randomStrings[1], GUILayout.MaxWidth(60f));

        var newRand = new Vector2(randomRange.x, randomRange.y);
        if (Single.TryParse(randomStrings[0], out float xParsed))
        {
          newRand.x = xParsed;
        }

        if (Single.TryParse(randomStrings[1], out float yParsed))
        {
          newRand.y = yParsed;
        }

        if (newRand.x != randomRange.x || newRand.y != randomRange.y)
        {
          randomRange = new(xParsed, yParsed);
        }

        GUILayout.EndHorizontal();
      }

      if (randTypes[randFlag] == RandomnessController.PerlinNoiseName)
      {
        randomSeed = GUILayout.Toggle(randomSeed, "Random Seed");
        if (!randomSeed)
        {
          GUILayout.BeginHorizontal();
          GUILayout.Label("Seed", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
          randomStrings[0] = GUILayout.TextArea(randomStrings[0], GUILayout.MaxWidth(60f));
          if (Int32.TryParse(randomStrings[0], out int intParsed))
          {
            perlinSeed = intParsed;
          }
          GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Minimum", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        randomStrings[3] = GUILayout.TextArea(randomStrings[3], GUILayout.MaxWidth(60f));
        if (Single.TryParse(randomStrings[3], out float floatParsed))
        {
          perlinMin = floatParsed;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Maximum", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        randomStrings[1] = GUILayout.TextArea(randomStrings[1], GUILayout.MaxWidth(60f));
        if (Single.TryParse(randomStrings[1], out floatParsed))
        {
          perlinScale = floatParsed;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        randomStrings[2] = GUILayout.TextArea(randomStrings[2], GUILayout.MaxWidth(60f));

        if (Single.TryParse(randomStrings[2], out floatParsed))
        {
          perlinSpeed = floatParsed;
        }

        GUILayout.EndHorizontal();
      }
    }

    protected override void LoadOptions(RandomnessController controller)
    {
      if (controller.noiseType == RandomnessController.RandomNoiseName)
      {
        randomStrings[0] = controller.range.x.ToString();
        randomStrings[1] = controller.range.y.ToString();
      }

      if (controller.noiseType == RandomnessController.PerlinNoiseName)
      {
        randomStrings[0] = controller.seed.ToString();
        randomStrings[1] = controller.scale.ToString();
        randomStrings[2] = controller.speed.ToString();
        randomStrings[3] = controller.minimum.ToString();
        randomSeed = controller.randomSeed;
      }
    }

    protected override RandomnessController CreateControllerInternal() =>
      new()
      {
        range     = randomRange,
        scale     = perlinScale,
        minimum   = perlinMin,
        seed      = perlinSeed,
        randomSeed = randomSeed,
        speed     = perlinSpeed,
        noiseType = randTypes[randFlag]
      };
  }
}