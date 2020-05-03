
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UICurveEditWindow : UIPopupWindow
  {
    #region GUI Variables
    protected string windowTitle = "";
    private int texWidth = 512;
    private int texHeight = 128;
    private Texture2D graphTexture;
    private Vector2 scrollPos = new Vector2();
    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    private  FloatCurve curve;
    protected UIModifierWindow modifier;
    protected string modifierTag;
    #endregion


    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      windowTitle = "CurveEditor";
      base.InitUI();
    }


    public UICurveEditWindow(FloatCurve curveToEdit, bool show) : base(show)
    {
      Utils.Log($"Started editing curve {curveToEdit.ToString()}");
      
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2, 678, 600);
      curve = curveToEdit;
      
      points = GraphUtils.FloatCurveToPoints(curveToEdit);
      UpdateCurve(out curve);
    }

    public UICurveEditWindow(FloatCurve curveToEdit, UIModifierWindow modWin, string tag, bool show) : base(show)
    {
      modifier = modWin;
      modifierTag = tag;
      Utils.Log($"Started editing curve {curveToEdit.ToString()}");

      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2, 678, 600);
      curve = curveToEdit;

      points = GraphUtils.FloatCurveToPoints(curveToEdit);
      UpdateCurve(out curve);
    }

    public void ChangeCurve(FloatCurve curveToEdit)
    {

      Utils.Log($"Started editing curve {curveToEdit.ToString()}");
      curve = curveToEdit;
      points = GraphUtils.FloatCurveToPoints(curveToEdit);
      UpdateCurve(out curve);
      showWindow = true;
    }
    public void ChangeCurve(FloatCurve curveToEdit, UIModifierWindow modWin, string tag)
    {
      modifier = modWin;
      modifierTag = tag;
      Utils.Log($"Started editing curve {curveToEdit.ToString()}");
      curve = curveToEdit;
      points = GraphUtils.FloatCurveToPoints(curveToEdit);
      UpdateCurve(out curve);
      showWindow = true;
    }


    /// <summary>
    /// Draw the window
    /// </summary>
    /// <param name="windowId">window ID</param>
    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawHeader();
      DrawCurveEditor();
      GUI.DragWindow();
    }

    protected virtual void DrawHeader()
    {
      GUILayout.BeginHorizontal();

      GUILayout.FlexibleSpace();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f), GUILayout.MinWidth(350f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
      
    }
    const int GraphLabels = 4;
    const float labelSpace = 20f * (GraphLabels + 1) / GraphLabels;
    private float minY = -5f;
    private float maxY = 5f;
    private string textVersion;
    private List<FloatString4> points = new List<FloatString4>();

   

    protected void DrawCurveEditor()
    {
      GUILayout.BeginHorizontal(GUILayout.Height(texHeight));

      Vector2 sizeMax = GUI.skin.label.CalcSize(new GUIContent(maxY.ToString("F3")));
      Vector2 sizeMin = GUI.skin.label.CalcSize(new GUIContent(minY.ToString("F3")));

      GUILayout.BeginVertical(GUILayout.MinWidth(Mathf.Max(sizeMin.x, sizeMax.x)));
      const int GraphLabels = 4;
        const float labelSpace = 20f * (GraphLabels + 1) / GraphLabels;
      for (int i = 0; i <= GraphLabels; i++)
      {
        GUILayout.Label((maxY - (maxY - minY) * i / GraphLabels).ToString("F3"), new GUIStyle(GUI.skin.label) { wordWrap = false });
        if (i != GraphLabels) //only do it if it's not the last one
          GUILayout.Space(texHeight / GraphLabels - labelSpace);
      }
      GUILayout.EndVertical();

      GUILayout.Box(graphTexture);

      GUILayout.EndHorizontal();

      FloatString4 excludePoint = null;

      scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(200));
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      GUILayout.BeginVertical();
      GUILayout.Label("X");

      foreach (FloatString4 p in points)
      {
        string ns = GUILayout.TextField(p.strings[0]);
        if (ns != p.strings[0])
        {
          p.strings[0] = ns;
          p.UpdateFloats();
          UpdateCurve(out curve);
        }
      }
      GUILayout.EndVertical();
      GUILayout.BeginVertical();
      GUILayout.Label("Y");
      foreach (FloatString4 p in points)
      {
        string ns = GUILayout.TextField(p.strings[1]);
        if (ns != p.strings[1])
        {
          p.strings[1] = ns;
          p.UpdateFloats();
          UpdateCurve(out curve);
        }
      }
      GUILayout.EndVertical();
      GUILayout.BeginVertical();
      GUILayout.Label("In Tangent");
      foreach (FloatString4 p in points)
      {
        string ns = GUILayout.TextField(p.strings[2]);
        if (ns != p.strings[2])
        {
          p.strings[2] = ns;
          p.UpdateFloats();
          UpdateCurve(out curve);
        }
      }
      GUILayout.EndVertical();
      GUILayout.BeginVertical();
      GUILayout.Label("Out Tangent");
      foreach (FloatString4 p in points)
      {
        string ns = GUILayout.TextField(p.strings[3]);
        if (ns != p.strings[3])
        {
          p.strings[3] = ns;
          p.UpdateFloats();
          UpdateCurve(out curve);
        }
      }
      GUILayout.EndVertical();
      GUILayout.BeginVertical();
      GUILayout.Label("Remove");
      foreach (FloatString4 p in points)
      {
        if (GUILayout.Button("X"))
        {
          excludePoint = p;
        }
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
      GUILayout.EndScrollView();

      if (excludePoint != null)
      {
        points.Remove(excludePoint);
        UpdateCurve(out curve);
      }

      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
     // sort = GUILayout.Toggle(sort, "Sort");
      if (GUILayout.Button("New Curve"))
      {
        points.Clear();
        textVersion = "";
        graphTexture = GraphUtils.GenerateCurveTexture(texWidth, texHeight, curve, Color.green);
      }
      if (GUILayout.Button("Smooth Tangents"))
      {
        //SmoothTangents();
      }
      if (GUILayout.Button("Copy out"))
      {
        GUIUtility.systemCopyBuffer = textVersion;
      }
      if (GUILayout.Button("Paste in"))
      {
        textVersion = GUIUtility.systemCopyBuffer;
        points = GraphUtils.StringToPoints(textVersion);
        UpdateCurve(out curve);
      }
      if (GUILayout.Button("Add Node"))
      {
        if (points.Count > 0)
        {
          points.Add(new FloatString4(points.Last().floats.x + 1, points.Last().floats.y, points.Last().floats.z, points.Last().floats.w));
        }
        else
        {
          points.Add(new FloatString4(0, 0, 0, 0));
        }
        UpdateCurve(out curve);
      }
      GUILayout.EndHorizontal();
    }
    
    public void UpdateCurve(out FloatCurve theCurve)
    {
      
      theCurve = new FloatCurve();
      foreach (FloatString4 v in points)
      {
        theCurve.Add(v.floats.x, v.floats.y, v.floats.z, v.floats.w);
      }

      theCurve.FindMinMaxValue(out minY, out maxY);
      curve = theCurve;
      WaterfallUI.Instance.UpdateCurve(curve);
      textVersion = GraphUtils.PointsToString(points);
      graphTexture = GraphUtils.GenerateCurveTexture(texWidth, texHeight, curve, Color.green);

    }


    //
    public bool Compare(FloatCurve toCompare)
    {

      if (GraphUtils.FloatCurveToPoints(toCompare) != points)
      {
        return false;
      }

      return true;
    }
    public FloatCurve GetCurve()
    {
      return curve;
    }

  }

}