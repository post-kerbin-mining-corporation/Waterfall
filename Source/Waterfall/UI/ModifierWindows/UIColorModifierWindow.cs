using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIColorModifierWindow : UIModifierWindow
  {
    public UIColorModifierWindow(EffectColorModifier mod, bool show) : base(mod, show)
    {
      colorMod       = mod;
      colorName      = colorMod.colorName;
      colorNames     = MaterialUtils.FindValidShaderProperties(colorMod.GetMaterial(), WaterfallMaterialPropertyType.Color).ToArray();
      colorIndex     = colorNames.ToList().FindIndex(x => x == colorMod.colorName);
      rCurveFunction = UpdateRedCurve;
      gCurveFunction = UpdateGreenCurve;
      bCurveFunction = UpdateBlueCurve;
      aCurveFunction = UpdateAlphaCurve;
      GenerateCurveThumbs(mod);
    }

    public override void UpdateCurves(FloatCurve newCurve, string tag)
    {
      base.UpdateCurves(newCurve, tag);
    }

    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Material Color";
    }

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Shader Color Name");
      int selectedIndex = GUILayout.SelectionGrid(colorIndex, colorNames, 4);
      if (selectedIndex != colorIndex)
      {
        colorIndex = selectedIndex;
        colorName  = colorNames[colorIndex];
        colorMod.ApplyMaterialName(colorName);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Red Curve", GUILayout.Width(hdrWidth));
      var buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      var imageRect  = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
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
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
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
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
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
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
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

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(colorMod);
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

    protected void GenerateCurveThumbs(EffectColorModifier colorMod)
    {
      miniRedCurve   = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.rCurve, Color.red);
      miniBlueCurve  = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.bCurve, Color.blue);
      miniGreenCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.gCurve, Color.green);
      miniAlphaCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, colorMod.aCurve, Color.white);
    }

    #region GUI Variables

    private readonly Vector2 curveButtonDims = new(100f, 50f);

    private Texture2D miniRedCurve;
    private Texture2D miniGreenCurve;
    private Texture2D miniBlueCurve;
    private Texture2D miniAlphaCurve;

    #endregion

    #region GUI Widgets

    #endregion

    #region Data

    private          string              colorName = "_TintColor";
    private          int                 colorIndex;
    private readonly string[]            colorNames;
    public           EffectColorModifier colorMod;
    private readonly CurveUpdateFunction rCurveFunction;
    private readonly CurveUpdateFunction gCurveFunction;
    private readonly CurveUpdateFunction bCurveFunction;
    private readonly CurveUpdateFunction aCurveFunction;

    #endregion
  }
}