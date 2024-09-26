using System;
using System.Collections.Generic;
using UnityEngine;
using UniLinq;

namespace Waterfall.UI.EffectControllersUI
{
  public class ThrottleControllerUIOptions : DefaultEffectControllerUIOptions<ThrottleController>
  {
    private readonly string[] throttleStrings;
    private string[] engineIDOptions;
    private int engineIndex;

    private float rampRateUp = 100f;
    private float rampRateDown = 100f;


    public ThrottleControllerUIOptions()
    {
      throttleStrings = new[] { rampRateUp.ToString(), rampRateDown.ToString() };
    }

    public override void DrawOptions()
    {

      GUILayout.BeginHorizontal();
      GUILayout.Label("Engine ID", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      if (engineIDOptions != null && engineIDOptions.Length != 0)
      {
        engineIndex = GUILayout.SelectionGrid(engineIndex, engineIDOptions, 2);
      }
      else
      {
        GUILayout.Label("0");
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Up", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      throttleStrings[0] = GUILayout.TextArea(throttleStrings[0], GUILayout.MaxWidth(60f));

      if (Single.TryParse(throttleStrings[0], out float floatParsed))
      {
        rampRateUp = floatParsed;
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Down", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
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
      List<ModuleEngines> engineOptions = controller.ParentModule.part.FindModulesImplementing<ModuleEngines>();
      engineIDOptions = engineOptions.Select(x => x.engineID).ToArray();
      engineIndex = engineIDOptions.ToList().IndexOf(controller.engineID);
      engineIndex = engineIndex == -1 ? 0 : engineIndex;
    }

    public override void DefaultOptions(ModuleWaterfallFX parentModule)
    {
      List<ModuleEngines> engineOptions = parentModule.part.FindModulesImplementing<ModuleEngines>();
      engineIDOptions = engineOptions.Select(x => x.engineID).ToArray();
      engineIndex = 0;
    }

    protected override ThrottleController CreateControllerInternal() =>
      new()
      {
        responseRateUp = rampRateUp,
        responseRateDown = rampRateDown,
        engineID = engineIDOptions[engineIndex]
      };
  }
}