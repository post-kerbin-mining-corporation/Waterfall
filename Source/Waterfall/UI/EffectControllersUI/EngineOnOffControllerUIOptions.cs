using System;
using System.Collections.Generic;
using UnityEngine;
using UniLinq;

namespace Waterfall.UI.EffectControllersUI
{
  public class EngineOnOffControllerUIOptions : DefaultEffectControllerUIOptions<EngineOnOffController>
  {
    private string[] engineIDOptions;
    private int engineIndex;
    private bool offOnFlameout;

    public EngineOnOffControllerUIOptions() { }

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
    }

    protected override void LoadOptions(EngineOnOffController controller)
    {
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
        engineID = engineIDOptions[engineIndex]
      };
  }
}