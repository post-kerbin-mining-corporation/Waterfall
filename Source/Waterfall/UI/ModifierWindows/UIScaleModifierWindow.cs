using UnityEngine;

namespace Waterfall.UI
{
  public class UIScaleModifierWindow : UIModifierWindow
  {
    private Texture2D miniXCurve;
    private Texture2D miniYCurve;
    private Texture2D miniZCurve;


    public UIScaleModifierWindow(EffectScaleModifier mod, bool show) : base(mod, show)
    {
      scaleMod       = mod;
      xCurveFunction = UpdateXCurve;
      yCurveFunction = UpdateYCurve;
      zCurveFunction = UpdateZCurve;
      GenerateCurveThumbs(mod);
    }


    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Transform Scale";
    }

    protected override void DrawHeader()
    {
      base.DrawHeader();
    }


    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Scaled Transform Name");
      scaleMod.transformName = GUILayout.TextArea(scaleMod.transformName);
      if (GUILayout.Button("Apply")) { }

      GUILayout.EndHorizontal();

      float hdrWidth = 125f;
      GUILayout.BeginHorizontal();
      GUILayout.Label("X Curve", GUILayout.Width(hdrWidth));

      var buttonRect = GUILayoutUtility.GetRect(150, 50);
      var imageRect  = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 5, buttonRect.width - 20, buttonRect.height - 10);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(scaleMod.xCurve, xCurveFunction);
        selectionFlag = 1;
      }

      GUI.DrawTexture(imageRect, miniXCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(scaleMod.xCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(scaleMod.xCurve, out scaleMod.xCurve);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Y Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(scaleMod.yCurve, yCurveFunction);
        selectionFlag = 2;
      }

      GUI.DrawTexture(imageRect, miniYCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(scaleMod.yCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(scaleMod.yCurve, out scaleMod.yCurve);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Z Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(scaleMod.zCurve, zCurveFunction);
        selectionFlag = 3;
      }

      GUI.DrawTexture(imageRect, miniZCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(scaleMod.zCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(scaleMod.zCurve, out scaleMod.zCurve);
      }

      GUILayout.EndHorizontal();
    }

    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(scaleMod);
    }


    protected void UpdateXCurve(FastFloatCurve curve)
    {
      scaleMod.xCurve = curve;
      UpdateModifierPanel();
    }

    protected void UpdateYCurve(FastFloatCurve curve)
    {
      scaleMod.yCurve = curve;
      UpdateModifierPanel();
    }

    protected void UpdateZCurve(FastFloatCurve curve)
    {
      scaleMod.zCurve = curve;
      UpdateModifierPanel();
    }


    protected void GenerateCurveThumbs(EffectScaleModifier scaleMod)
    {
      miniXCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, scaleMod.xCurve, Color.red);
      miniYCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, scaleMod.yCurve, Color.blue);
      miniZCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, scaleMod.zCurve, Color.green);
    }

    #region GUI Variables

    private Vector2 effectsScrollListPosition = Vector2.zero;
    private Vector2 partsScrollListPosition   = Vector2.zero;

    #endregion

    #region GUI Widgets

    #endregion

    #region Data

    public           EffectScaleModifier scaleMod;
    private readonly CurveUpdateFunction xCurveFunction;
    private readonly CurveUpdateFunction yCurveFunction;
    private readonly CurveUpdateFunction zCurveFunction;

    #endregion
  }
}