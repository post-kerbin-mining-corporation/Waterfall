
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UILightFloatModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    Vector2 curveButtonDims = new Vector2(100f, 50f);

    Texture2D miniCurve;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    CurveUpdateFunction floatCurveFunction;
    string floatName = "_TintColor";
    int floatIndex = 0;
    string[] floatNames;
    public EffectLightFloatModifier floatMod;
    #endregion

    public UILightFloatModifierWindow(EffectLightFloatModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      floatMod = mod;
      floatNames = new string[] { "Intensity", "Range", "SpotAngle"};
      floatIndex = floatNames.ToList().FindIndex(x => x == floatMod.floatName);
      floatCurveFunction = new CurveUpdateFunction(UpdateFloatCurve);
      GenerateCurveThumbs(mod);
    }


    protected void UpdateFloatCurve(FloatCurve curve)
    {
      floatMod.curve = curve;
      UpdateModifierPanel();
    }

    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Light Float Parameter";
    }
    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {

      GUILayout.BeginHorizontal();
      GUILayout.Label("Light Float Name");
      int selectedIndex = GUILayout.SelectionGrid(floatIndex, floatNames, 4);
      if (selectedIndex != floatIndex)
      {
        floatIndex = selectedIndex;
        floatName = floatNames[floatIndex];
        floatMod.ApplyFloatName(floatName);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Float Curve", GUILayout.Width(hdrWidth));
      Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(floatMod.curve, floatCurveFunction);
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(floatMod.curve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(floatMod.curve, out floatMod.curve);
      }
      GUILayout.EndHorizontal();
    }
    public override void UpdateCurves(FloatCurve newCurve, string tag)
    {
      //base.UpdateCurves(newCurve, tag);
      // Get the color from the UI.
      //floatMod.curve = newCurve;
      UpdateModifierPanel();
    }

    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(floatMod);
    }
    protected void GenerateCurveThumbs(EffectLightFloatModifier floatMod)
    {
      miniCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.curve, Color.green);
    }


  }

}
