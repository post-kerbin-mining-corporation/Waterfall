using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UILightFloatModifierWindow : UIModifierWindow
  {
    public UILightFloatModifierWindow(EffectLightFloatModifier mod, bool show) : base(mod, show)
    {
      floatMod           = mod;
      floatNames         = new[] { "Intensity", "Range", "SpotAngle" };
      floatIndex         = floatNames.ToList().FindIndex(x => x == floatMod.floatName);
      FastFloatCurveFunction = UpdateFastFloatCurve;
      GenerateCurveThumbs(mod);
    }

    public override void UpdateCurves(FastFloatCurve newCurve, string tag)
    {
      //base.UpdateCurves(newCurve, tag);
      // Get the color from the UI.
      //floatMod.curve = newCurve;
      UpdateModifierPanel();
    }

    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Light Float Parameter";
    }

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Light Float Name");
      int selectedIndex = GUILayout.SelectionGrid(floatIndex, floatNames, 4);
      if (selectedIndex != floatIndex)
      {
        floatIndex = selectedIndex;
        floatName  = floatNames[floatIndex];
        floatMod.ApplyFloatName(floatName);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Float Curve", GUILayout.Width(hdrWidth));
      var buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      var imageRect  = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(floatMod.curve, FastFloatCurveFunction);
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

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(floatMod);
    }


    protected void UpdateFastFloatCurve(FastFloatCurve curve)
    {
      floatMod.curve = curve;
      UpdateModifierPanel();
    }

    protected void GenerateCurveThumbs(EffectLightFloatModifier floatMod)
    {
      miniCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, floatMod.curve, Color.green);
    }

    #region GUI Variables

    private readonly Vector2 curveButtonDims = new(100f, 50f);

    private Texture2D miniCurve;

    #endregion

    #region GUI Widgets

    #endregion

    #region Data

    private readonly CurveUpdateFunction      FastFloatCurveFunction;
    private          string                   floatName = "_TintColor";
    private          int                      floatIndex;
    private readonly string[]                 floatNames;
    public           EffectLightFloatModifier floatMod;

    #endregion
  }
}