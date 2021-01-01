
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIPositionModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    public EffectPositionModifier posMod;
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
      windowTitle = "Modifier Editor - Transform Position";
    }


    public UIPositionModifierWindow(EffectPositionModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      posMod = mod;
      xCurveFunction = new CurveUpdateFunction(UpdateXCurve);
      yCurveFunction = new CurveUpdateFunction(UpdateYCurve);
      zCurveFunction = new CurveUpdateFunction(UpdateZCurve);
      GenerateCurveThumbs(mod);
    }


    protected override void DrawHeader()
    {
      base.DrawHeader();
    }

    protected void UpdateXCurve(FloatCurve curve)
    {
      posMod.xCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateYCurve(FloatCurve curve)
    {
      posMod.yCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateZCurve(FloatCurve curve)
    {
      posMod.zCurve = curve;
      UpdateModifierPanel();
    }

    protected override void DrawModifierPanel()
    {

      GUILayout.BeginHorizontal();
      GUILayout.Label("Positioned Transform Name");
      posMod.transformName = GUILayout.TextArea(posMod.transformName);
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
        EditCurve(posMod.xCurve, xCurveFunction);
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniXCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(posMod.xCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(posMod.xCurve, out posMod.xCurve);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Y Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(posMod.yCurve, yCurveFunction);
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniYCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(posMod.yCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(posMod.yCurve, out posMod.yCurve);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Z Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(posMod.zCurve, zCurveFunction);
        selectionFlag = 3;
      }
      GUI.DrawTexture(imageRect, miniZCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(posMod.zCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(posMod.zCurve, out posMod.zCurve);
      }
      GUILayout.EndHorizontal();

    }
    public override void UpdateCurves(FloatCurve newCurve, string tag)
    {
      base.UpdateCurves(newCurve, tag);
    }

    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(posMod);
    }
    Texture2D miniXCurve;
    Texture2D miniYCurve;
    Texture2D miniZCurve;
    

    protected void GenerateCurveThumbs(EffectPositionModifier posMod)
    {
      miniXCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, posMod.xCurve, Color.red);
      miniYCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, posMod.yCurve, Color.blue);
      miniZCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, posMod.zCurve, Color.green);
    }


  }

}