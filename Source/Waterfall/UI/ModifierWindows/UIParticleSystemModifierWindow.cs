using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall.UI
{
  class UIParticleSystemModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    Vector2 curveButtonDims = new Vector2(100f, 50f);

    Texture2D miniCurve1;
    Texture2D miniCurve2;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    CurveUpdateFunction curve1Function;
    CurveUpdateFunction curve2Function;
    string paramName = "X";
    int paramIndex = 0;
    string[] paramNames;
    public EffectParticleSystemModifier paramMod;
    #endregion

    public UIParticleSystemModifierWindow(EffectParticleSystemModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      paramMod = mod;
      paramNames = WaterfallConstants.ParticleParameterMap.Keys.ToArray();
      paramIndex = paramNames.ToList().FindIndex(x => x == paramMod.paramName);
      curve1Function = new CurveUpdateFunction(UpdateFloatCurve1);
      curve2Function = new CurveUpdateFunction(UpdateFloatCurve2);
      GenerateCurveThumbs(mod);
    }


    protected void UpdateFloatCurve1(FloatCurve curve)
    {
      paramMod.curve1 = curve;
      UpdateModifierPanel();
    }
    protected void UpdateFloatCurve2(FloatCurve curve)
    {
      paramMod.curve2 = curve;
      UpdateModifierPanel();
    }
    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Particle System Parameter";
    }
    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {

      GUILayout.BeginHorizontal();
      GUILayout.Label("Particle Parameter Name");
      int selectedIndex = GUILayout.SelectionGrid(paramIndex, paramNames, 4);
      if (selectedIndex != paramIndex)
      {
        paramIndex = selectedIndex;
        paramName = paramNames[paramIndex];
        paramMod.ApplyParameterName(paramName);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Start Curve", GUILayout.Width(hdrWidth));
      Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.curve1, curve1Function);
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniCurve1);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.curve1);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.curve1, out paramMod.curve1);
      }
      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label("End Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.curve2, curve2Function);
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniCurve1);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.curve2);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.curve2, out paramMod.curve2);
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
      GenerateCurveThumbs(paramMod);
    }
    protected void GenerateCurveThumbs(EffectParticleSystemModifier floatMod)
    {
      miniCurve1 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.curve1, Color.red);
      miniCurve2 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.curve2,  Color.green);
    }


  }
}
