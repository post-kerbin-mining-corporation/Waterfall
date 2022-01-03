using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class RandomnessControllerUIOptions : DefaultEffectControllerUIOptions<RandomnessController>
  {
    string[] randTypes = { RandomnessController.RandomNoiseName, RandomnessController.PerlinNoiseName };
    int randFlag = 0;

    string[] randomStrings = new string[4];
    Vector2 randomRange;
    int perlinSeed = 0;
    float perlinMin = 0f;
    float perlinScale = 1f;
    float perlinSpeed = 1f;

    private readonly UIResources guiResources;

    public RandomnessControllerUIOptions(UIResources guiResources)
    {
      this.guiResources = guiResources ?? throw new ArgumentNullException(nameof(guiResources));
    }

    public override void DrawOptions()
    {
      GUILayout.Label("Random type");
      randFlag = GUILayout.SelectionGrid(randFlag, randTypes, Mathf.Min(randTypes.Length, 4), guiResources.GetStyle("radio_text_button"));

      if (randTypes[randFlag] == RandomnessController.RandomNoiseName)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Min/Max", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));

        randomStrings[0] = GUILayout.TextArea(randomStrings[0], GUILayout.MaxWidth(60f));
        randomStrings[1] = GUILayout.TextArea(randomStrings[1], GUILayout.MaxWidth(60f));

        Vector2 newRand = new Vector2(randomRange.x, randomRange.y);
        if (float.TryParse(randomStrings[0], out float xParsed))
        {
          newRand.x = xParsed;
        }

        if (float.TryParse(randomStrings[1], out float yParsed))
        {
          newRand.y = yParsed;
        }

        if (newRand.x != randomRange.x || newRand.y != randomRange.y)
        {
          randomRange = new Vector2(xParsed, yParsed);
        }

        GUILayout.EndHorizontal();
      }

      if (randTypes[randFlag] == RandomnessController.PerlinNoiseName)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Seed", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        randomStrings[0] = GUILayout.TextArea(randomStrings[0], GUILayout.MaxWidth(60f));
        if (int.TryParse(randomStrings[0], out int intParsed))
        {
          perlinSeed = intParsed;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Minimum", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        randomStrings[3] = GUILayout.TextArea(randomStrings[3], GUILayout.MaxWidth(60f));
        if (float.TryParse(randomStrings[3], out float floatParsed))
        {
          perlinMin = floatParsed;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Maximum", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        randomStrings[1] = GUILayout.TextArea(randomStrings[1], GUILayout.MaxWidth(60f));
        if (float.TryParse(randomStrings[1], out floatParsed))
        {
          perlinScale = floatParsed;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        randomStrings[2] = GUILayout.TextArea(randomStrings[2], GUILayout.MaxWidth(60f));

        if (float.TryParse(randomStrings[2], out floatParsed))
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
      }
    }

    protected override RandomnessController CreateControllerInternal()
    {
      return new RandomnessController
      {
        range = randomRange,
        scale = perlinScale,
        minimum = perlinMin,
        seed = perlinSeed,
        speed = perlinSpeed,
        noiseType = randTypes[randFlag],
      };
    }
  }
}