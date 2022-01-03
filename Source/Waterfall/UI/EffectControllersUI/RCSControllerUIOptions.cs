using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class RCSControllerUIOptions : DefaultEffectControllerUIOptions<RCSController>
  {
    private readonly UIResources guiResources;
    private readonly string[] throttleStrings;

    private float rampRateUp = 100f;
    private float rampRateDown = 100f;

    public RCSControllerUIOptions(UIResources guiResources)
    {
      this.guiResources = guiResources ?? throw new ArgumentNullException(nameof(guiResources));
      throttleStrings = new string[] { rampRateUp.ToString(), rampRateDown.ToString() };
    }

    public override void DrawOptions()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Up", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      throttleStrings[0] = GUILayout.TextArea(throttleStrings[0], GUILayout.MaxWidth(60f));
      float floatParsed;
      if (float.TryParse(throttleStrings[0], out floatParsed))
      {
        rampRateUp = floatParsed;
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Down", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      throttleStrings[1] = GUILayout.TextArea(throttleStrings[1], GUILayout.MaxWidth(60f));
      if (float.TryParse(throttleStrings[1], out floatParsed))
      {
        rampRateDown = floatParsed;
      }

      GUILayout.EndHorizontal();
    }

    protected override void LoadOptions(RCSController controller)
    {
      throttleStrings[0] = controller.responseRateUp.ToString();
      throttleStrings[1] = controller.responseRateDown.ToString();
    }

    protected override RCSController CreateControllerInternal()
    {
      return new RCSController
      {
        responseRateUp = rampRateUp,
        responseRateDown = rampRateDown
      };
    }
  }
}