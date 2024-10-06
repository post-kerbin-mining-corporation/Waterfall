using UnityEngine;

namespace Waterfall.UI
{
  public class UIUVScrollModifierWindow : UIModifierWindow
  {
    #region Data

    public EffectUVScrollModifier scrollMod;

    #endregion

    private Texture2D miniXCurve;
    private Texture2D miniYCurve;


    public UIUVScrollModifierWindow(EffectUVScrollModifier mod, bool show) : base(mod, show)
    {
      scrollMod = mod;
      GenerateCurveThumbs(mod);
    }

    public override void UpdateCurves(FastFloatCurve newCurve, string tag)
    {
      base.UpdateCurves(newCurve, tag);
      // Get the color from the UI.
      switch (tag)
      {
        case "x":
          scrollMod.scrollCurveX = newCurve;
          break;
        case "y":
          scrollMod.scrollCurveY = newCurve;
          break;
      }

      UpdateModifierPanel();
    }


    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - UV Scroll";
    }


    protected override void DrawHeader()
    {
      base.DrawHeader();
    }


    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Scaled Transform Name");
      scrollMod.textureName = GUILayout.TextArea(scrollMod.textureName);
      if (GUILayout.Button("Apply")) { }

      GUILayout.EndHorizontal();

      float hdrWidth = 125f;
      GUILayout.BeginHorizontal();
      GUILayout.Label("X Scroll Rate", GUILayout.Width(hdrWidth));

      var buttonRect = GUILayoutUtility.GetRect(150, 50);
      var imageRect  = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 5, buttonRect.width - 20, buttonRect.height - 10);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(scrollMod.scrollCurveX, "s");
      }

      GUI.DrawTexture(imageRect, miniXCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(scrollMod.scrollCurveX);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(scrollMod.scrollCurveX, out scrollMod.scrollCurveX);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Y Scroll Rate", GUILayout.Width(hdrWidth));

      buttonRect = GUILayoutUtility.GetRect(150, 50);
      imageRect  = new(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(scrollMod.scrollCurveY, "y");
      }

      GUI.DrawTexture(imageRect, miniYCurve);
      if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
      {
        CopyCurve(scrollMod.scrollCurveY);
      }

      if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
      {
        PasteCurve(scrollMod.scrollCurveY, out scrollMod.scrollCurveY);
      }

      GUILayout.EndHorizontal();
    }


    protected void GenerateCurveThumbs(EffectUVScrollModifier scaleMod)
    {
      miniXCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, scrollMod.scrollCurveX, Color.red);
      miniYCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, scrollMod.scrollCurveY, Color.blue);
    }

    #region GUI Variables

    private Vector2 effectsScrollListPosition = Vector2.zero;
    private Vector2 partsScrollListPosition   = Vector2.zero;

    #endregion

    #region GUI Widgets

    #endregion
  }
}