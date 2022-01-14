using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class ThrottleControllerUIOptions : DefaultEffectControllerUIOptions<ThrottleController>
  {
    private readonly string[] throttleStrings;

    private readonly UIResources guiResources;
    private          float       rampRateUp   = 100f;
    private          float       rampRateDown = 100f;

    public ThrottleControllerUIOptions(UIResources guiResources)
    {
      this.guiResources = guiResources ?? throw new ArgumentNullException(nameof(guiResources));
      throttleStrings   = new[] { rampRateUp.ToString(), rampRateDown.ToString() };
    }

    public override void DrawOptions()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Up", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      throttleStrings[0] = GUILayout.TextArea(throttleStrings[0], GUILayout.MaxWidth(60f));

      if (Single.TryParse(throttleStrings[0], out float floatParsed))
      {
        rampRateUp = floatParsed;
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Down", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      throttleStrings[1] = GUILayout.TextArea(throttleStrings[1], GUILayout.MaxWidth(60f));
      if (Single.TryParse(throttleStrings[1], out floatParsed))
      {
        rampRateDown = floatParsed;
      }

      GUILayout.EndHorizontal();
    }

    protected override void LoadOptions(ThrottleController controller)
    {
      throttleStrings[0] = controller.responseRateUp.ToString();
      throttleStrings[1] = controller.responseRateDown.ToString();
    }

    protected override ThrottleController CreateControllerInternal() =>
      new()
      {
        responseRateUp   = rampRateUp,
        responseRateDown = rampRateDown
      };
  }
}