using System.Collections.Generic;
using UnityEngine;
using UniLinq;

namespace Waterfall.UI.EffectControllersUI
{
  public class ThrustControllerUIOptions : DefaultEffectControllerUIOptions<ThrustController> 
  {
    private string[] engineIDOptions;
    private int engineIndex;

    public ThrustControllerUIOptions() { }

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
    }

    protected override void LoadOptions(ThrustController controller)
    {
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

    protected override ThrustController CreateControllerInternal() =>
      new()
      {
        engineID = engineIDOptions[engineIndex]
      };
  }
}