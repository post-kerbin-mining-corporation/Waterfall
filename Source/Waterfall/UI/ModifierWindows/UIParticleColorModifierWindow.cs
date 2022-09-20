using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall.EffectModifiers;

namespace Waterfall.UI
{
  public class UIParticleColorModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    private Vector2 curveButtonDims = new Vector2(100f, 50f);

    private Texture2D miniCurve1;
    private Texture2D miniCurve2;
    private Texture2D miniCurve3;
    private Texture2D miniCurve4;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    private CurveUpdateFunction rCurveFunction;
    private CurveUpdateFunction gCurveFunction;
    private CurveUpdateFunction bCurveFunction;
    private CurveUpdateFunction aCurveFunction;

    private string paramName = "X";
    private int paramIndex = 0;
    private string[] paramNames;
    public EffectParticleColorModifier paramMod;
    #endregion

    public UIParticleColorModifierWindow(EffectParticleColorModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      paramMod = mod;
      paramNames = WaterfallParticleLoader.FindValidParticleProperties(WaterfallParticlePropertyType.Color).ToArray();
      paramIndex = paramNames.ToList().FindIndex(x => x == paramMod.paramName);
      rCurveFunction = new CurveUpdateFunction(UpdateFloatCurveR);
      gCurveFunction = new CurveUpdateFunction(UpdateFloatCurveG);
      bCurveFunction = new CurveUpdateFunction(UpdateFloatCurveB);
      aCurveFunction = new CurveUpdateFunction(UpdateFloatCurveA);
      GenerateCurveThumbs(mod);
    }


    protected void UpdateFloatCurveR(FloatCurve curve)
    {
      paramMod.rCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateFloatCurveG(FloatCurve curve)
    {
      paramMod.gCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateFloatCurveB(FloatCurve curve)
    {
      paramMod.bCurve = curve;
      UpdateModifierPanel();
    }
    protected void UpdateFloatCurveA(FloatCurve curve)
    {
      paramMod.aCurve = curve;
      UpdateModifierPanel();
    }
    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Particle System Color";
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
      GUILayout.Label("Red Curve", GUILayout.Width(hdrWidth));
      Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.rCurve, rCurveFunction);
        selectionFlag = 1;
      }
      GUI.DrawTexture(imageRect, miniCurve1);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.rCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.rCurve, out paramMod.rCurve);
      }
      GUILayout.EndHorizontal();
      


      GUILayout.BeginHorizontal();
      GUILayout.Label("Green Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.gCurve, gCurveFunction);
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniCurve2);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.gCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.gCurve, out paramMod.gCurve);
      }
      GUILayout.EndHorizontal();



      GUILayout.BeginHorizontal();
      GUILayout.Label("Blue Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.bCurve, bCurveFunction);
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniCurve3);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.bCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.bCurve, out paramMod.bCurve);
      }
      GUILayout.EndHorizontal();



      GUILayout.BeginHorizontal();
      GUILayout.Label("Alpha Curve", GUILayout.Width(hdrWidth));
      buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(paramMod.aCurve, aCurveFunction);
        selectionFlag = 2;
      }
      GUI.DrawTexture(imageRect, miniCurve4);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(paramMod.aCurve);
      }
      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(paramMod.aCurve, out paramMod.aCurve);
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
    protected void GenerateCurveThumbs(EffectParticleColorModifier floatMod)
    {
      miniCurve1 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.rCurve, Color.red);
      miniCurve2 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.gCurve, Color.green);
      miniCurve3 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.bCurve, Color.blue);
      miniCurve4 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.aCurve, Color.white);
    }


  }
}
