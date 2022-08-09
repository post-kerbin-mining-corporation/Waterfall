using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIParticleFloatModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    private Vector2 curveButtonDims = new Vector2(100f, 50f);

    private Texture2D miniCurve1;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    private CurveUpdateFunction curve1Function;
    private string paramName = "X";
    private int paramIndex = 0;
    private string[] paramNames;
    public EffectParticleFloatModifier paramMod;
    #endregion

    public UIParticleFloatModifierWindow(EffectParticleFloatModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      paramMod = mod;
      
      paramNames = WaterfallParticleLoader.FindValidParticleProperties(WaterfallParticlePropertyType.Float).ToArray();
      paramIndex = paramNames.ToList().FindIndex(x => x == paramMod.paramName);
      curve1Function = new CurveUpdateFunction(UpdateFloatCurve);
      GenerateCurveThumbs(mod);
    }


    protected void UpdateFloatCurve(FloatCurve curve)
    {
      paramMod.curve = curve;
      UpdateModifierPanel();
    }
    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Particle System Float";
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
        EditCurve(paramMod.curve, curve1Function);
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniCurve1);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.curve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.curve, out paramMod.curve);
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
    protected void GenerateCurveThumbs(EffectParticleFloatModifier floatMod)
    {
      miniCurve1 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.curve, Color.red);
    }


  }
}
