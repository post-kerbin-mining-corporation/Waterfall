
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIUVScrollModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    Vector2 effectsScrollListPosition = Vector2.zero;
    Vector2 partsScrollListPosition = Vector2.zero;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    public EffectUVScrollModifier scrollMod;
    #endregion


    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {

      base.InitUI();
      windowTitle = "UV Scroll Modifier Editor";
    }


    public UIUVScrollModifierWindow(EffectUVScrollModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      scrollMod = mod;
      GenerateCurveThumbs(mod);
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
      if (GUILayout.Button("Apply"))
      {

      }
      GUILayout.EndHorizontal();

      float hdrWidth = 125f;
      GUILayout.BeginHorizontal();
      GUILayout.Label("X Scroll Rate", GUILayout.Width(hdrWidth));

      Rect buttonRect = GUILayoutUtility.GetRect(150, 50);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 5, buttonRect.width - 20, buttonRect.height - 10);
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
      imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
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
    public override void UpdateCurves(FloatCurve newCurve, string tag)
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
    Texture2D miniXCurve;
    Texture2D miniYCurve;
    

    protected void GenerateCurveThumbs(EffectUVScrollModifier scaleMod)
    {
      miniXCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, scrollMod.scrollCurveX, Color.red);
      miniYCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, scrollMod.scrollCurveY, Color.blue);
    }


  }

}