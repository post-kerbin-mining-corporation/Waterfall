using UnityEngine;

namespace Waterfall.UI
{
  public class UIRotationModifierWindow : UIModifierWindow
  {
    private Texture2D miniXCurve;
    private Texture2D miniYCurve;
    private Texture2D miniZCurve;


    public UIRotationModifierWindow(EffectRotationModifier mod, bool show) : base(mod, show)
    {
      rotMod         = mod;
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
      windowTitle = "Modifier Editor - Transform Rotation";
    }

    protected override void DrawHeader()
    {
      base.DrawHeader();
    }


    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Scaled Transform Name");
      rotMod.transformName = GUILayout.TextArea(rotMod.transformName);
      if (GUILayout.Button("Apply")) { }

      GUILayout.EndHorizontal();

      float hdrWidth = 125f;
      GUILayout.BeginHorizontal();
      GUILayout.Label("X Curve", GUILayout.Width(hdrWidth));

      var buttonRect = GUILayoutUtility.GetRect(150, 50);
      var imageRect  = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 5, buttonRect.width - 20, buttonRect.height - 10);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(rotMod.xCurve, xCurveFunction);
        selectionFlag = 1;
      }

      GUI.DrawTexture(imageRect, miniXCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(rotMod.xCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(rotMod.xCurve, out rotMod.xCurve);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Y Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(rotMod.yCurve, yCurveFunction);
        selectionFlag = 2;
      }

      GUI.DrawTexture(imageRect, miniYCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(rotMod.yCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(rotMod.yCurve, out rotMod.yCurve);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Z Curve", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(rotMod.zCurve, zCurveFunction);
        selectionFlag = 3;
      }

      GUI.DrawTexture(imageRect, miniZCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(rotMod.zCurve);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(rotMod.zCurve, out rotMod.zCurve);
      }

      GUILayout.EndHorizontal();
    }

    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(rotMod);
    }


    protected void UpdateXCurve(FloatCurve curve)
    {
      rotMod.xCurve = curve;
      UpdateModifierPanel();
    }

    protected void UpdateYCurve(FloatCurve curve)
    {
      rotMod.yCurve = curve;
      UpdateModifierPanel();
    }

    protected void UpdateZCurve(FloatCurve curve)
    {
      rotMod.zCurve = curve;
      UpdateModifierPanel();
    }

    protected void GenerateCurveThumbs(EffectRotationModifier rotMod)
    {
      miniXCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, rotMod.xCurve, Color.red);
      miniYCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, rotMod.yCurve, Color.blue);
      miniZCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, rotMod.zCurve, Color.green);
    }

    #region GUI Variables

    #endregion

    #region GUI Widgets

    #endregion

    #region Data

    public           EffectRotationModifier rotMod;
    private readonly CurveUpdateFunction    xCurveFunction;
    private readonly CurveUpdateFunction    yCurveFunction;
    private readonly CurveUpdateFunction    zCurveFunction;

    #endregion
  }
}