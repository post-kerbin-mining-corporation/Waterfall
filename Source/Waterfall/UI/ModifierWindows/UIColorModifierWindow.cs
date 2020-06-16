
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIColorModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    Vector2 curveButtonDims = new Vector2(100f, 50f);

    Texture2D miniRedCurve;
    Texture2D miniGreenCurve;
    Texture2D miniBlueCurve;
    Texture2D miniAlphaCurve;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    string colorName = "_TintColor";
    public EffectColorModifier colorMod;
    #endregion

    public UIColorModifierWindow(EffectColorModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      colorMod = mod;
      colorName = colorMod.colorName;
      GenerateCurveThumbs(mod);
    }

    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Material Color";
    }
    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {
      
      GUILayout.BeginHorizontal();
      GUILayout.Label("Shader Color Name");
      colorName = GUILayout.TextArea(colorName);
      if (GUILayout.Button("Apply"))
      {
        colorMod.ApplyMaterialName(colorName);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Red Curve", GUILayout.Width(hdrWidth));
      Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(colorMod.rCurve, "red");
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniRedCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(colorMod.rCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(colorMod.rCurve, out colorMod.rCurve);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Green Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        //EditCurve(colorMod.gCurve);
        EditCurve(colorMod.gCurve, "green");
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniGreenCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(colorMod.gCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(colorMod.gCurve, out colorMod.gCurve);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Blue Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if(GUI.Button(buttonRect, ""))
      {
        EditCurve(colorMod.bCurve, "blue");
        selectionFlag = 3;
      }
      GUI.DrawTexture(imageRect, miniBlueCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(colorMod.bCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(colorMod.bCurve, out colorMod.bCurve);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Alpha Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(colorMod.aCurve, "alpha");
        selectionFlag = 4;
      }
      GUI.DrawTexture(imageRect, miniAlphaCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(colorMod.aCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(colorMod.aCurve, out colorMod.aCurve);
      }
      GUILayout.EndHorizontal();    
    }
    public override void UpdateCurves(FloatCurve newCurve, string tag)
    {
      base.UpdateCurves(newCurve, tag);
      // Get the color from the UI.
      switch (tag)
      {
        case "red":
          colorMod.rCurve = newCurve;
          break;
        case "green":
          colorMod.gCurve = newCurve;
          break;
        case "blue":
          colorMod.bCurve = newCurve;
          break;
        case "alpha":
          colorMod.aCurve = newCurve;
          break;
      }
      UpdateModifierPanel();
    }

    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(colorMod);
    }
    protected void GenerateCurveThumbs(EffectColorModifier colorMod)
    {
      miniRedCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.rCurve, Color.red);
      miniBlueCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.bCurve, Color.blue);
      miniGreenCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.gCurve, Color.green);
      miniAlphaCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.aCurve, Color.white);
    }


  }

}