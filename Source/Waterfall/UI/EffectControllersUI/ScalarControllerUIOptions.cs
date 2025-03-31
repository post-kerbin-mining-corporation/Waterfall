using System.Collections.Generic;
using UnityEngine;
using UniLinq;

namespace Waterfall.UI.EffectControllersUI
{
  public class ScalarControllerUIOptions : DefaultEffectControllerUIOptions<ScalarModuleController> 
  {
    private string[] scalarIDOptions;
    private int scalarIndex;

    public ScalarControllerUIOptions() 
    {
    }

    public override void DrawOptions()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Scalar Module ID", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      if (scalarIDOptions != null && scalarIDOptions.Length != 0)
      {
        scalarIndex = GUILayout.SelectionGrid(scalarIndex, scalarIDOptions, 2);
      }
      else
      {
        GUILayout.Label("0");
      }
      GUILayout.EndHorizontal();
    }

    protected override void LoadOptions(ScalarModuleController controller)
    {
      List<IScalarModule> scalarOptions = controller.ParentModule.part.FindModulesImplementing<IScalarModule>();
      scalarIDOptions = scalarOptions.Select(x => x.ScalarModuleID).ToArray();
      scalarIndex = scalarIDOptions.ToList().IndexOf(controller.moduleID);
      scalarIndex = scalarIndex == -1 ? 0 : scalarIndex;
    }

    public override void DefaultOptions(ModuleWaterfallFX parentModule)
    {
      List<IScalarModule> scalarOptions = parentModule.part.FindModulesImplementing<IScalarModule>();
      scalarIDOptions = scalarOptions.Select(x => x.ScalarModuleID).ToArray();
      scalarIndex = 0;
    }

    protected override ScalarModuleController CreateControllerInternal() =>
      new()
      {
        moduleID = scalarIDOptions[scalarIndex]
      };
  }
}