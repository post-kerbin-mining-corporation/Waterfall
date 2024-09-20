using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class VelocityControllerUIOptions : DefaultEffectControllerUIOptions<VelocityController>
  {
    private readonly string[] modes = { "Surface", "Orbit" };

    private int modeFlag;

    public VelocityControllerUIOptions() { }

    public override void DrawOptions()
    {
      GUILayout.Label("Velocity Mode");
      int flagChanged = GUILayout.SelectionGrid(
        modeFlag, 
        modes, 
        Mathf.Min(modes.Length, 2), 
        UIResources.GetStyle("radio_text_button"));

      modeFlag = flagChanged;
    }

    protected override void LoadOptions(VelocityController controller)
    {
      modeFlag = controller.mode;
      if (modeFlag < 0 || modeFlag >= modes.Length)
      {
        modeFlag = 0;
      }
    }

    protected override VelocityController CreateControllerInternal() =>
      new()
    {
      mode = modeFlag
    };
  }
}
