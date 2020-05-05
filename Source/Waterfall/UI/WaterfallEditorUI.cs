
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{
  [KSPAddon(KSPAddon.Startup.Flight, false)]
  public class WaterfallUI : UIAppToolbarWindow
  {
    public static WaterfallUI Instance { get; private set; }
    #region GUI Variables
    private string windowTitle = "";
    Vector2 effectsScrollListPosition = Vector2.zero;
    Vector2 partsScrollListPosition = Vector2.zero;

    #endregion

    #region GUI Widgets
    UICurveEditWindow curveEditWindow;
    #endregion

    #region Vessel Data
    Vessel vessel;
    List<ModuleWaterfallFX> effectsModules = new List<ModuleWaterfallFX>();
    ModuleWaterfallFX selectedModule;
    List<UIEffectWidget> effectUIWidgets = new List<UIEffectWidget>();
    #endregion


    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      windowTitle = "WaterfallFX Editor";
      base.InitUI();
    }

    protected override void Awake()
    {
      base.Awake();
      Instance = this;
    }

    protected override void Start()
    {
      base.Start();
      GetVesselData();
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    protected override void Draw()
    {
     
      base.Draw();
      foreach (UIModifierWindow modWin in editWindows)
      {
        modWin.Draw();
      }
      if (curveEditWindow != null)
      {
        curveEditWindow.Draw();
      }
    }


    /// <summary>
    /// Draw the window
    /// </summary>
    /// <param name="windowId">window ID</param>
    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawHeader();


      GUILayout.BeginHorizontal();
      // Draw the parts list
      DrawPartsList();

      // Draw the effects list

      
      DrawEffectsList();
      GUILayout.EndHorizontal();
      GUI.DragWindow();
    }

    protected void DrawHeader()
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

      GUILayout.BeginHorizontal();
      DrawControllers();
      DrawExporters();
      GUILayout.EndHorizontal();
    }

    bool useControllers = false;

    float densityControllerValue = 0f;
    float throttleControllerValue = 0f;

    protected void DrawControllers()
    {
      GUILayout.BeginHorizontal();

      useControllers = GUILayout.Toggle(useControllers, "Link to Editor", GUILayout.Width(150));
      GUILayout.BeginVertical();
      GUILayout.Label("CONTROLLERS");

      GUILayout.BeginHorizontal();
      GUILayout.Label("Throttle", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      throttleControllerValue = GUILayout.HorizontalSlider(throttleControllerValue, 0f, 1f, GUILayout.MaxWidth(120f));
      GUILayout.Label(throttleControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Atmosphere Depth", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      densityControllerValue = GUILayout.HorizontalSlider(densityControllerValue, 0f, 1f, GUILayout.MaxWidth(120f));
      GUILayout.Label(densityControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
    }

    protected void DrawExporters()
    {
      if (GUILayout.Button("Dump all to log"))
      {
        for (int i = 0; i < effectsModules.Count; i++)
        {
          Utils.Log(effectsModules[i].Export().ToString());
        }
      }
      if (GUILayout.Button("Dump selected to log"))
      {
        Utils.Log(selectedModule.Export().ToString());
        
      }
    }

    protected void DrawPartsList()
    {
      GUILayout.BeginVertical();
      GUILayout.Label("FX MODULES");
      partsScrollListPosition = GUILayout.BeginScrollView(partsScrollListPosition, GUILayout.Width(200f), GUILayout.MinHeight(100f));
      for (int i=0; i< effectsModules.Count; i++)
      {
        if (GUILayout.Button($"{effectsModules[i].moduleID}\n{effectsModules[i].FX.Count} Effects"))
        {
          SelectFXModule(effectsModules[i]);
        }
      }
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
    }


    protected void DrawEffectsList()
    {
      GUILayout.BeginVertical();
      GUILayout.Label("EFFECTS");
      effectsScrollListPosition = GUILayout.BeginScrollView(effectsScrollListPosition, GUILayout.ExpandWidth(true), GUILayout.MinHeight(200f));

      for (int i = 0; i < effectUIWidgets.Count; i++)
      {
        effectUIWidgets[i].Draw();
      }

      GUILayout.EndScrollView();
      GUILayout.EndVertical();
    }
    public void SelectFXModule(ModuleWaterfallFX fxMod)
    {
      selectedModule = fxMod;
      RefreshEffectList();
    }
    public void OpenModifierEditWindow()
    {

    }

    public void RefreshEffectList()
    {
      effectUIWidgets.Clear();
      foreach (WaterfallEffect fx in selectedModule.FX)
      {
        effectUIWidgets.Add(new UIEffectWidget(this, fx));
      }
    }
    public void GetVesselData()
    {
      vessel = FlightGlobals.ActiveVessel;
      effectsModules = new List<ModuleWaterfallFX>();
      if (vessel != null)
      {
        foreach (Part p in vessel.Parts)
        {
          ModuleWaterfallFX[] fxModules = p.GetComponents<ModuleWaterfallFX>();
          foreach (ModuleWaterfallFX fxModule in fxModules)
          {
            effectsModules.Add(fxModule);
          }
        }
        if (effectsModules.Count > 0)
        {
          SelectFXModule(effectsModules[0]);
           
         }

      }
    }

    protected List<UIModifierWindow> editWindows = new List<UIModifierWindow>();

    public void OpenModifierEditWindow(EffectModifier fxMod)
    {
      foreach (UIModifierWindow editWin in editWindows)
      {
        if (editWin.modifier == fxMod)
        {
          editWin.SetWindowState(true);
          return;
          
        }
      }
      try
      {
        EffectColorModifier colMod = (EffectColorModifier)fxMod;
        if (colMod != null)
        {
          editWindows.Add(new UIColorModifierWindow(colMod, true));
        }

      } catch (InvalidCastException e) {}
      try
      {
        EffectScaleModifier scaleMod = (EffectScaleModifier)fxMod;
        if (scaleMod != null)
        {
          editWindows.Add(new UIScaleModifierWindow(scaleMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectUVScrollModifier scrollMod = (EffectUVScrollModifier)fxMod;
        if (scrollMod != null)
        {
          editWindows.Add(new UIUVScrollModifierWindow(scrollMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectFloatModifier floatMod = (EffectFloatModifier)fxMod;
        if (floatMod != null)
        {
          editWindows.Add(new UIFloatModifierWindow(floatMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectPositionModifier posMod = (EffectPositionModifier)fxMod;
        if (posMod != null)
        {
          editWindows.Add(new UIPositionModifierWindow(posMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectRotationModifier rotMod = (EffectRotationModifier)fxMod;
        if (rotMod != null)
        {
          editWindows.Add(new UIRotationModifierWindow(rotMod, true));
        }
      }
      catch (InvalidCastException e) { }
    }

    public UICurveEditWindow OpenCurveEditor(FloatCurve toEdit)
    {
      if (curveEditWindow != null)
      {
        curveEditWindow.ChangeCurve(toEdit);
      }
      else
      {
        curveEditWindow = new UICurveEditWindow(toEdit, true);
      }
      return curveEditWindow;
    }

    UIModifierWindow currentModWinForCurve;
    string currentCurveTag;

    public UICurveEditWindow OpenCurveEditor(FloatCurve toEdit, UIModifierWindow modWin, string tag)
    {

      currentModWinForCurve = modWin;
      currentCurveTag = tag;
      if (curveEditWindow != null)
      {
        curveEditWindow.ChangeCurve(toEdit, modWin, tag);
      }
      else
      {
        curveEditWindow = new UICurveEditWindow(toEdit, modWin, tag, true);
      }
      return curveEditWindow;
    }

    public void UpdateCurve(FloatCurve curve)
    {
      currentModWinForCurve.UpdateCurves(curve, currentCurveTag);
    }

    public void Update()
    {
      for (int i = 0; i < effectsModules.Count; i++)
      {
        effectsModules[i].SetControllerOverride(useControllers);
        effectsModules[i].SetControllerOverrideValue("atmosphereDepth", densityControllerValue);
        effectsModules[i].SetControllerOverrideValue("throttle", throttleControllerValue);
      }
      for (int i = 0; i < effectUIWidgets.Count; i++)
      {
        effectUIWidgets[i].Update();
      }
    }
  }

}