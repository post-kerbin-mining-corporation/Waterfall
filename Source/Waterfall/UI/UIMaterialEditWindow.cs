using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall;

namespace Waterfall.UI
{
  public class UIMaterialEditWindow : UIPopupWindow
  {

    string windowTitle = "Material Editor";
    WaterfallMaterial matl;
    public UIMaterialEditWindow(WaterfallMaterial materialToEdit, bool show) : base(show)
    {
      matl = materialToEdit;

      InitializeShaderProperties(matl.material);
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      DrawMaterialEdit();
      GUI.DragWindow();
      
    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f), GUILayout.MinWidth(350f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected void DrawMaterialEdit()
    {
      
    }

    protected void InitializeShaderProperties(Material m)
    {
      
    }
  }

  
}
