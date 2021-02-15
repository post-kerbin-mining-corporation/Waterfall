
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIRotationModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    public EffectRotationModifier rotMod;
    CurveUpdateFunction xCurveFunction;
    CurveUpdateFunction yCurveFunction;
    CurveUpdateFunction zCurveFunction;
    #endregion


    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      
      base.InitUI();
      windowTitle = "Modifier Editor - Transform Rotation";
    }


    public UIRotationModifierWindow(EffectRotationModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      rotMod = mod;
      xCurveFunction = new CurveUpdateFunction(UpdateXCurve);
      yCurveFunction = new CurveUpdateFunction(UpdateYCurve);
      zCurveFunction = new CurveUpdateFunction(UpdateZCurve);
      GenerateCurveThumbs(mod);
    }


    protected void UpdateXCurve(FloatCurve curve)
    {
      rotMod.xCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateYCurve(FloatCurve curve)
    {
      rotMod.yCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateZCurve(FloatCurve curve)
    {
      rotMod.zCurve = curve;
      UpdateModifierPanel();
    }
    protected override void DrawHeader()
    {
      base.DrawHeader();
    }



    protected override void DrawModifierPanel()
    {

      GUILayout.BeginHorizontal();
      GUILayout.Label("Scaled Transform Name");
      rotMod.transformName = GUILayout.TextArea(rotMod.transformName);
      if (GUILayout.Button("Apply"))
      {

      }
      GUILayout.EndHorizontal();

      float hdrWidth = 125f;
      GUILayout.BeginHorizontal();
      GUILayout.Label("X Curve", GUILayout.Width(hdrWidth));

      Rect buttonRect = GUILayoutUtility.GetRect(150, 50);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 5, buttonRect.width - 20, buttonRect.height - 10);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(rotMod.xCurve, xCurveFunction);
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniXCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(rotMod.xCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(rotMod.xCurve, out rotMod.xCurve);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Y Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(rotMod.yCurve, yCurveFunction);
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniYCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(rotMod.yCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(rotMod.yCurve, out rotMod.yCurve);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Z Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(rotMod.zCurve, zCurveFunction);
        selectionFlag = 3;
      }
      GUI.DrawTexture(imageRect, miniZCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(rotMod.zCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(rotMod.zCurve, out rotMod.zCurve);
      }
      GUILayout.EndHorizontal();

    }
    Texture2D miniXCurve;
    Texture2D miniYCurve;
    Texture2D miniZCurve;

    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(rotMod);
    }
    protected void GenerateCurveThumbs(EffectRotationModifier rotMod)
    {
      miniXCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, rotMod.xCurve, Color.red);
      miniYCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, rotMod.yCurve, Color.blue);
      miniZCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, rotMod.zCurve, Color.green);
    }


  }

}