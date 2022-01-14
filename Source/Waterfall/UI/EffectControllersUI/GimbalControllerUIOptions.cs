using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class GimbalControllerUIOptions : DefaultEffectControllerUIOptions<GimbalController>
  {
    private readonly string[] axisTypes = { "x", "y", "z" };

    private readonly UIResources guiResources;
    private          int         axisFlag;

    public GimbalControllerUIOptions(UIResources guiResources)
    {
      this.guiResources = guiResources ?? throw new ArgumentNullException(nameof(guiResources));
    }

    public override void DrawOptions()
    {
      GUILayout.Label("Gimbal axis");
      int axisFlagChanged = GUILayout.SelectionGrid(axisFlag, axisTypes, Mathf.Min(axisTypes.Length, 4), guiResources.GetStyle("radio_text_button"));
      axisFlag = axisFlagChanged;
    }

    protected override void LoadOptions(GimbalController controller)
    {
      axisFlag = axisTypes.IndexOf(controller.axis);
      if (axisFlag < 0 || axisFlag >= axisTypes.Length)
      {
        axisFlag = 0;
      }
    }

    protected override GimbalController CreateControllerInternal() =>
      new()
      {
        axis = axisTypes[axisFlag]
      };
  }
}