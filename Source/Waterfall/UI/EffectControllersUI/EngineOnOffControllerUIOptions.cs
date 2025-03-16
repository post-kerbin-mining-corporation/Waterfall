using System;
using System.Collections.Generic;
using UnityEngine;
using UniLinq;

namespace Waterfall.UI.EffectControllersUI
{
  public class EngineOnOffControllerUIOptions : DefaultEffectControllerUIOptions<EngineOnOffController>
  {
    private readonly string[] rampStrings;
    private string[] engineIDOptions;
    private int engineIndex;
    private bool offOnFlameout;
    private float rampRateUp = 100f;
    private float rampRateDown = 100f;


    public EngineOnOffControllerUIOptions() 
    {
      rampStrings = new[] { rampRateUp.ToString(), rampRateDown.ToString() };
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
      offOnFlameout = GUILayout.Toggle(offOnFlameout, "Zero on Flameout");

      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Up", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      rampStrings[0] = GUILayout.TextArea(rampStrings[0], GUILayout.MaxWidth(60f));

      if (Single.TryParse(rampStrings[0], out float floatParsed))
      {
        rampRateUp = floatParsed;
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Ramp Rate Down", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      rampStrings[1] = GUILayout.TextArea(rampStrings[1], GUILayout.MaxWidth(60f));
      if (Single.TryParse(rampStrings[1], out floatParsed))
      {
        rampRateDown = floatParsed;
      }

      GUILayout.EndHorizontal();
    }

    protected override void LoadOptions(EngineOnOffController controller)
    {
      rampStrings[0] = controller.responseRateUp.ToString();
      rampStrings[1] = controller.responseRateDown.ToString();
      List<ModuleEngines> engineOptions = controller.ParentModule.part.FindModulesImplementing<ModuleEngines>();
      engineIDOptions = engineOptions.Select(x => x.engineID).ToArray();
      engineIndex = engineIDOptions.ToList().IndexOf(controller.engineID);
      engineIndex = engineIndex == -1 ? 0 : engineIndex;
      offOnFlameout = controller.zeroOnFlameout;
    }

    public override void DefaultOptions(ModuleWaterfallFX parentModule)
    {
      List<ModuleEngines> engineOptions = parentModule.part.FindModulesImplementing<ModuleEngines>();
      engineIDOptions = engineOptions.Select(x => x.engineID).ToArray();
      engineIndex = 0;
      offOnFlameout = true;
    }

    protected override EngineOnOffController CreateControllerInternal() =>
      new()
      {
        engineID = engineIDOptions[engineIndex],
        responseRateUp = rampRateUp,
        responseRateDown = rampRateDown
      };
  }
}