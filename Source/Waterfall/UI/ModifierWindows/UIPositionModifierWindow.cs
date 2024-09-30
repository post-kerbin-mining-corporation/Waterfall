using UnityEngine;

namespace Waterfall.UI
{
  public class UIPositionModifierWindow : UIModifierWindow
  {
    private Texture2D miniXCurve;
    private Texture2D miniYCurve;
    private Texture2D miniZCurve;


    public UIPositionModifierWindow(EffectPositionModifier mod, bool show) : base(mod, show)
    {
      posMod         = mod;
      xCurveFunction = UpdateXCurve;
      yCurveFunction = UpdateYCurve;
      zCurveFunction = UpdateZCurve;
      GenerateCurveThumbs(mod);
    }

    public override void UpdateCurves(FastFloatCurve newCurve, string tag)
    {
      base.UpdateCurves(newCurve, tag);
    }


    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Transform Position";
    }


    protected override void DrawHeader()
    {
      base.DrawHeader();
    }

    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Positioned Transform Name");
      posMod.transformName = GUILayout.TextArea(posMod.transformName);
      if (GUILayout.Button("Apply")) { }

      GUILayout.EndHorizontal();

      float hdrWidth = 125f;
      GUILayout.BeginHorizontal();
      GUILayout.Label("X Curve", GUILayout.Width(hdrWidth));

      var buttonRect = GUILayoutUtility.GetRect(150, 50);
      var imageRect  = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 5, buttonRect.width - 20, buttonRect.height - 10);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(posMod.xCurve, xCurveFunction);
        selectionFlag = 1;
      }

      GUI.DrawTexture(imageRect, miniXCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(posMod.xCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(posMod.xCurve, out posMod.xCurve);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Y Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(posMod.yCurve, yCurveFunction);
        selectionFlag = 2;
      }

      GUI.DrawTexture(imageRect, miniYCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(posMod.yCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(posMod.yCurve, out posMod.yCurve);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Z Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(posMod.zCurve, zCurveFunction);
        selectionFlag = 3;
      }

      GUI.DrawTexture(imageRect, miniZCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(posMod.zCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(posMod.zCurve, out posMod.zCurve);
      }

      GUILayout.EndHorizontal();
    }

    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(posMod);
    }

    protected void UpdateXCurve(FastFloatCurve curve)
    {
      posMod.xCurve = curve;
      UpdateModifierPanel();
    }

    protected void UpdateYCurve(FastFloatCurve curve)
    {
      posMod.yCurve = curve;
      UpdateModifierPanel();
    }

    protected void UpdateZCurve(FastFloatCurve curve)
    {
      posMod.zCurve = curve;
      UpdateModifierPanel();
    }


    protected void GenerateCurveThumbs(EffectPositionModifier posMod)
    {
      miniXCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, posMod.xCurve, Color.red);
      miniYCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, posMod.yCurve, Color.blue);
      miniZCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, posMod.zCurve, Color.green);
    }

    #region GUI Variables

    #endregion

    #region GUI Widgets

    #endregion

    #region Data

    public           EffectPositionModifier posMod;
    private readonly CurveUpdateFunction    xCurveFunction;
    private readonly CurveUpdateFunction    yCurveFunction;
    private readonly CurveUpdateFunction    zCurveFunction;

    #endregion
  }
}