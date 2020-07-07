
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIModifierWindow : UIPopupWindow
  {
    #region GUI Variables
    protected string windowTitle = "";
    protected int texWidth = 80;
    protected int texHeight = 30;
    protected float hdrWidth = 75f;
    protected float copyWidth = 40f;
    protected FloatCurve copyBuffer;
    #endregion

    #region GUI Widgets
    protected UICurveEditWindow curveEditor;
    #endregion

    #region Modifier Data
    public EffectModifier modifier;
    string[] combineModes;
    string[] controllerNames;
    int combineModeFlag = 0;
    int controllerFlag = 0;
    protected int selectionFlag = 0;
    #endregion


    public UIModifierWindow(EffectModifier mod, bool show) : base(show)
    {
      controllerNames = new string[] { "throttle", "atmosphereDepth" };
      for (int i = 0; i < controllerNames.Length; i++)
      {
        if (controllerNames[i] == mod.controllerName)
          controllerFlag = i;
      }
      combineModes = Enum.GetNames(typeof(EffectModifierMode));
      combineModeFlag = (int)mod.effectMode;
      modifier = mod;
      windowPos = new Rect(WaterfallUI.Instance.WindowPosition.xMax+5f, WaterfallUI.Instance.WindowPosition.yMin, 500f, 100f);
    }

    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor";
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    public override void Draw()
    {
      base.Draw();
    }


    /// <summary>
    /// Draw the window
    /// </summary>
    /// <param name="windowId">window ID</param>
    protected override void DrawWindow(int windowId)
    {
      DrawTitle();
      // Draw the header/tab controls
      DrawHeader();
      DrawModifierPanel();
    }
    /// <summary>
    /// Draws the title panel (title, close button)
    /// </summary>
    protected virtual void DrawTitle()
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
    /// <summary>
    ///  Draws the common header area
    /// </summary>
    protected virtual void DrawHeader()
    {
      GUILayout.BeginVertical();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Modifier Name");
      modifier.fxName = GUILayout.TextArea(modifier.fxName);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller");
      controllerFlag = GUILayout.SelectionGrid(controllerFlag, controllerNames, controllerNames.Length, GUIResources.GetStyle("radio_text_button"));
      modifier.controllerName = controllerNames[controllerFlag];
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Combine Mode");
      combineModeFlag = GUILayout.SelectionGrid(combineModeFlag, combineModes, combineModes.Length, GUIResources.GetStyle("radio_text_button"));
      modifier.effectMode = (EffectModifierMode)combineModeFlag;
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      modifier.useRandomness = GUILayout.Toggle(modifier.useRandomness, "Use Randomness");
      
      if (modifier.useRandomness)
      {
        
        GUILayout.Label("Controller");
        modifier.randomnessController = GUILayout.TextArea(modifier.randomnessController);
        GUILayout.Label("Scale");
        modifier.randomScale = float.Parse(GUILayout.TextArea(modifier.randomScale.ToString()));

      }
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the child-specific items
    /// </summary>
    protected virtual void DrawModifierPanel()
    {
    }
    /// <summary>
    /// Draws the child-specific items
    /// </summary>
    protected virtual void UpdateModifierPanel()
    {
    }

    /// <summary>
    /// Spawns the float editor panel to edit a float
    /// </summary>
    /// <param name="toEdit"></param>
    protected void EditCurve(FloatCurve toEdit)
    {
      Utils.Log($"Started editing curve {toEdit.Curve.ToString()}");
      curveEditor =  WaterfallUI.Instance.OpenCurveEditor(toEdit);
    }
    /// <summary>
    /// Spawns the float editor panel to edit a float
    /// </summary>
    /// <param name="toEdit"></param>
    protected void EditCurve(FloatCurve toEdit, string tag)
    {
      Utils.Log($"Started editing curve {toEdit.Curve.ToString()}");
      curveEditor = WaterfallUI.Instance.OpenCurveEditor(toEdit, this, tag);
    }

    protected void CopyCurve(FloatCurve toCopy)
    {
      UIUtils.CopyFloatCurve(toCopy);
    }
    public virtual void UpdateCurves(FloatCurve newCurve, string tag)
    {
     
    }
    protected void PasteCurve(FloatCurve value, out FloatCurve target)
    {
      if (UIUtils.CurveCopyBuffer != null)
        target = UIUtils.CurveCopyBuffer;
      else
        target = value;
      UpdateModifierPanel();
    }
  }

}