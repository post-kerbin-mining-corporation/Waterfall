using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIParticleRangeModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    private Vector2 curveButtonDims = new Vector2(100f, 50f);

    private Texture2D miniCurve1;
    private Texture2D miniCurve2;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    private CurveUpdateFunction curve1Function;
    private CurveUpdateFunction curve2Function;
    private string paramName = "X";
    private int paramIndex = 0;
    private string[] paramNames;
    public EffectParticleRangeModifier paramMod;
    #endregion

    public UIParticleRangeModifierWindow(EffectParticleRangeModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      paramMod = mod;
      paramNames = WaterfallParticleLoader.FindValidParticleProperties(WaterfallParticlePropertyType.Range).ToArray();
      paramIndex = paramNames.ToList().FindIndex(x => x == paramMod.paramName);
      curve1Function = new CurveUpdateFunction(UpdateFloatCurve1);
      curve2Function = new CurveUpdateFunction(UpdateFloatCurve2);
      GenerateCurveThumbs(mod);
    }


    protected void UpdateFloatCurve1(FloatCurve curve)
    {
      paramMod.xCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateFloatCurve2(FloatCurve curve)
    {
      paramMod.yCurve = curve;
      UpdateModifierPanel();
    }
    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Particle System Range";
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
      //if (WaterfallConstants.ParticleParameterMap.ContainsKey(paramNames[paramIndex]))
      //{
      GUILayout.BeginHorizontal();
      GUILayout.Label("Start Curve", GUILayout.Width(hdrWidth));
      Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.xCurve, curve1Function);
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniCurve1);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.xCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.xCurve, out paramMod.xCurve);
      }
      GUILayout.EndHorizontal();
      //if (WaterfallConstants.ParticleParameterMap[paramName].ParamType == ParticleParameterType.Range)
      //{

      GUILayout.BeginHorizontal();
      GUILayout.Label("End Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.yCurve, curve2Function);
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniCurve2);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.yCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.yCurve, out paramMod.yCurve);
      }
      GUILayout.EndHorizontal();
      //  }
      //}
      //else
      //{

      //  GUILayout.Label("Choose a parameter");
      //}
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
    protected void GenerateCurveThumbs(EffectParticleRangeModifier floatMod)
    {
      miniCurve1 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.xCurve, Color.red);
      miniCurve2 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.yCurve, Color.green);
    }


  }
}
