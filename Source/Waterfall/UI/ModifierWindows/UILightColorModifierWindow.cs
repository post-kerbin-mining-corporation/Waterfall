
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UILightColorModifierWindow : UIModifierWindow
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
    int colorIndex = 0;
    string[] colorNames;
    public EffectLightColorModifier colorMod;
    CurveUpdateFunction rCurveFunction;
    CurveUpdateFunction gCurveFunction;
    CurveUpdateFunction bCurveFunction;
    CurveUpdateFunction aCurveFunction;
    #endregion

    public UILightColorModifierWindow(EffectLightColorModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      colorMod = mod;
      rCurveFunction = new CurveUpdateFunction(UpdateRedCurve);
      gCurveFunction = new CurveUpdateFunction(UpdateGreenCurve);
      bCurveFunction = new CurveUpdateFunction(UpdateBlueCurve);
      aCurveFunction = new CurveUpdateFunction(UpdateAlphaCurve);
      GenerateCurveThumbs(mod);
    }

    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Light Color";
    }
    protected void UpdateRedCurve(FloatCurve curve)
    {
      colorMod.rCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateBlueCurve(FloatCurve curve)
    {
      colorMod.bCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateGreenCurve(FloatCurve curve)
    {
      colorMod.gCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateAlphaCurve(FloatCurve curve)
    {
      colorMod.aCurve = curve;
      UpdateModifierPanel();
    }
    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {

     

      GUILayout.BeginHorizontal();
      GUILayout.Label("Red Curve", GUILayout.Width(hdrWidth));
      Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(colorMod.rCurve, rCurveFunction);
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
        EditCurve(colorMod.gCurve, gCurveFunction);
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
        EditCurve(colorMod.bCurve, bCurveFunction);
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
        EditCurve(colorMod.aCurve, aCurveFunction);
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
    }

    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(colorMod);
    }
    protected void GenerateCurveThumbs(EffectLightColorModifier colorMod)
    {
      miniRedCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.rCurve, Color.red);
      miniBlueCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.bCurve, Color.blue);
      miniGreenCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.gCurve, Color.green);
      miniAlphaCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.aCurve, Color.white);
    }


  }

}
