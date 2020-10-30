
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Waterfall.Modules;

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

    bool useControllers = false;
    float densityControllerValue = 0f;
    float throttleControllerValue = 0f;
    Vector2 randomControllerValue;
    float rcsControllerValue;

    public Vector3 modelRotation;
    public Vector3 modelOffset;
    public Vector3 modelScale = Vector3.one;
    string templateName = "";
    string[] modelOffsetString;
    string[] modelRotationString;
    string[] modelScaleString;

    #endregion

    #region GUI Widgets
    UICurveEditWindow curveEditWindow;
    UIModifierPopupWindow modifierPopupWindow;
    UIAddEffectWindow effectAddWindow;
    UIModifierWindow currentModWinForCurve;
    UIMaterialEditWindow materialEditWindow;
    UIColorPickerWindow colorPickWindow;
    UITexturePickerWindow texturePickWindow;
    UISmokeEditWindow smokeEditWindow;
    string currentCurveTag;
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
      windowPos = new Rect(200f, 200f, 800f, 600f);
      modelOffsetString = new string[] { modelOffset.x.ToString(), modelOffset.y.ToString(), modelOffset.z.ToString() };
      modelRotationString = new string[] { modelRotation.x.ToString(), modelRotation.y.ToString(), modelRotation.z.ToString() };
      modelScaleString = new string[] { modelScale.x.ToString(), modelScale.y.ToString(), modelScale.z.ToString() };
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
      if (smokeEditWindow != null)
      {
        smokeEditWindow.Draw();
      }
      if (curveEditWindow != null)
      {
        curveEditWindow.Draw();
      }
      if (materialEditWindow != null)
      {
        materialEditWindow.Draw();
      }
      if (modifierPopupWindow != null)
      {
        modifierPopupWindow.Draw();
      }
      if (effectAddWindow != null)
      {
        effectAddWindow.Draw();
      }
      if (colorPickWindow != null)
      {
        colorPickWindow.Draw();
      }
      if (texturePickWindow != null)
      {
        texturePickWindow.Draw();
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

      // Draw the parts list
      DrawPartsList();

      // Draw the effects list


      DrawEffectsList();
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
      DrawTemplateControl();
      DrawExporters();
      GUILayout.EndHorizontal();
    }


    protected void DrawControllers()
    {
      GUILayout.BeginHorizontal();
      GUILayout.BeginVertical();
      useControllers = GUILayout.Toggle(useControllers, "Link to Editor", GUILayout.Width(150));
      
      GUILayout.Label("<b>CONTROLLERS</b>");

      // 
      GUILayout.BeginHorizontal();
      GUILayout.Label("Throttle", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(120f));
      throttleControllerValue = GUILayout.HorizontalSlider(throttleControllerValue, 0f, 1f, GUILayout.MaxWidth(100f));
      GUILayout.Label(throttleControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(40f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Atmosphere Depth", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(120f));
      densityControllerValue = GUILayout.HorizontalSlider(densityControllerValue, 0f, 1f, GUILayout.MaxWidth(100f));
      GUILayout.Label(densityControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(40f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Randomness Min/Max", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));

      string xValue = GUILayout.TextArea(randomControllerValue.x.ToString(), GUILayout.MaxWidth(60f));
      string yValue = GUILayout.TextArea(randomControllerValue.y.ToString(), GUILayout.MaxWidth(60f));

      float xParsed;
      float yParsed;
      Vector2 newRand = new Vector2(randomControllerValue.x, randomControllerValue.y);
      if (float.TryParse(xValue, out xParsed))
      {
        if (xParsed != randomControllerValue.x)
        {
          newRand.x = xParsed;
        }
      }
      if (float.TryParse(yValue, out yParsed))
      {
        if (yParsed != randomControllerValue.y)
        {
          newRand.y = yParsed;
        }
      }
      if (newRand.x != randomControllerValue.x || newRand.y != randomControllerValue.y)
      {
        randomControllerValue = new Vector2(xParsed, yParsed);
      }
      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label("RCS Throttle", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(120f));
      rcsControllerValue = GUILayout.HorizontalSlider(rcsControllerValue, 0f, 1f, GUILayout.MaxWidth(100f));
      GUILayout.Label(rcsControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(40f));
      GUILayout.EndHorizontal();

      
      GUILayout.BeginHorizontal();
      GUILayout.Label("Smoke Control", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(120f));
      if (GUILayout.Button("Open", GUILayout.MaxWidth(100f)))
      {
        if (selectedModule)
        {
          ModuleWaterfallSmoke smoke = selectedModule.part.GetComponent<ModuleWaterfallSmoke>();
          if (smoke)
            OpenSmokeEditor(smoke);
        }
        
      }

     
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
    }

    protected bool templatesOpen = false;
    protected bool exportsOpen = false;
    protected void DrawTemplateControl()
    {
      GUILayout.BeginVertical();

      GUILayout.BeginHorizontal();
      if (templatesOpen)
      {
        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
          templatesOpen = false;
        }
      }
      else
      {
        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
          templatesOpen = true;
        }
      }
      
      GUILayout.Label("<b>TEMPLATES</b>");
      GUILayout.EndHorizontal();
      if (templatesOpen)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Template Name");
        templateName = GUILayout.TextArea(templateName, GUILayout.MaxWidth(100f));
        GUILayout.EndHorizontal();
        GUILayout.Label("Template Offset");
        modelOffset = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(200f, 30f), modelOffset, modelOffsetString, GUI.skin.label, GUI.skin.textArea);

        GUILayout.Label("Template Rotation ");
        modelRotation = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(200f, 30f), modelRotation, modelRotationString, GUI.skin.label, GUI.skin.textArea);

        GUILayout.Label("Template Scale");
        modelScale = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(200f, 30f), modelScale, modelScaleString, GUI.skin.label, GUI.skin.textArea);

      }
      GUILayout.EndVertical();
    }
     protected void DrawExporters()
    {
      GUILayout.BeginVertical();
      GUILayout.BeginHorizontal();
      if (exportsOpen)
      {
        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
          exportsOpen = false;
        }
      }
      else
      {
        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
          exportsOpen = true;
        }
      }

      GUILayout.Label("<b>EXPORT</b>");
      GUILayout.EndHorizontal();

      if (exportsOpen)
      {
        if (GUILayout.Button("Dump selected Effects\n to log", GUILayout.Width(150f), GUILayout.Height(60)))
        {
          Utils.Log(selectedModule.Export().ToString());

        }
        if (GUILayout.Button("Copy selected Effects\n to clipboard", GUILayout.Width(150f), GUILayout.Height(60)))
        {
          GUIUtility.systemCopyBuffer = (selectedModule.Export().ToString());

        }
        if (GUILayout.Button("Copy selected Effects\n as template to \nclipboard", GUILayout.Width(150f), GUILayout.Height(60)))
        {
          ConfigNode node = new ConfigNode(WaterfallConstants.TemplateLibraryNodeName);
          node.AddValue("templateName", templateName);
          foreach (WaterfallEffect fx in selectedModule.FX)
          {
            node.AddNode(fx.Save());
          }

          GUIUtility.systemCopyBuffer = (node.ToString());

        }
        if (GUILayout.Button("Copy template offset\nvalues to clipboard", GUILayout.Width(150f), GUILayout.Height(60)))
        {
          string copiedString = "";
          copiedString += $"position = {modelOffset.x},{modelOffset.y},{modelOffset.z}\n";
          copiedString += $"rotation = {modelRotation.x}, {modelRotation.y}, {modelRotation.z}\n";
          copiedString += $"scale = {modelScale.x}, {modelScale.y}, {modelScale.z}";

          GUIUtility.systemCopyBuffer = copiedString;

        }
      }
      GUILayout.EndVertical();
    }

    protected void DrawPartsList()
    {
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      GUILayout.BeginVertical();
      GUILayout.Label("<b>FX MODULES</b>");
      partsScrollListPosition = GUILayout.BeginScrollView(partsScrollListPosition, GUILayout.ExpandWidth(true), GUILayout.Height(40f));

      GUILayout.BeginHorizontal();
      for (int i = 0; i < effectsModules.Count; i++)
      {

        if (GUILayout.Button($"{effectsModules[i].moduleID} ({effectsModules[i].FX.Count} Effects)", GUILayout.MaxWidth(250f)))
        {
          SelectFXModule(effectsModules[i]);
        }
      }
      GUILayout.EndHorizontal();

      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
    }


    protected void DrawEffectsList()
    {
      GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
      GUILayout.BeginHorizontal();
      GUILayout.Label("<b>EFFECTS</b>");
      GUILayout.FlexibleSpace();
      if (selectedModule != null)
      {
        if (GUILayout.Button("Add"))
        {
          OpenEffectAddWindow();
        }
      }
      GUILayout.EndHorizontal();


      effectsScrollListPosition = GUILayout.BeginScrollView(effectsScrollListPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinHeight(350f));

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

    public void RefreshEffectList()
    {
      effectUIWidgets.Clear();
      foreach (WaterfallEffect fx in selectedModule.FX)
      {
        effectUIWidgets.Add(new UIEffectWidget(this, fx));

        modelRotation = fx.TemplateRotationOffset;
        modelScale = fx.TemplateScaleOffset;
        modelOffset = fx.TemplatePositionOffset;
        modelOffsetString = new string[] { modelOffset.x.ToString(), modelOffset.y.ToString(), modelOffset.z.ToString() };
        modelRotationString = new string[] { modelRotation.x.ToString(), modelRotation.y.ToString(), modelRotation.z.ToString() };
        modelScaleString = new string[] { modelScale.x.ToString(), modelScale.y.ToString(), modelScale.z.ToString() };
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
      foreach (UIModifierWindow editWin in editWindows.ToList())
      {
        editWindows.Remove(editWin);
      }
      try
      {
        EffectColorModifier colMod = (EffectColorModifier)fxMod;
        if (colMod != null)
        {
          editWindows.Add(new UIColorModifierWindow(colMod, true));
        }

      }
      catch (InvalidCastException e) { }
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
    public UISmokeEditWindow OpenSmokeEditor(ModuleWaterfallSmoke toEdit)
    {

      
      if (smokeEditWindow != null)
      {
        
      }
      else
      {
        smokeEditWindow = new UISmokeEditWindow(toEdit, true);
      }
      return smokeEditWindow;
    }

    public UIMaterialEditWindow OpenMaterialEditWindow(WaterfallModel mdl)
    {

      if (materialEditWindow != null)
      {
        materialEditWindow.ChangeMaterial(mdl);
      }
      else
      {
        materialEditWindow = new UIMaterialEditWindow(mdl, true);
      }
      return materialEditWindow;
    }
    public UIColorPickerWindow OpenColorEditWindow(Color c)
    {

      if (colorPickWindow != null)
      {
        Utils.Log("[WaterfallUI] Changing Color Picker target", LogType.UI);
        colorPickWindow.ChangeColor(c);
      }
      else
      {
        Utils.Log("[WaterfallUI] Opening Color Picker", LogType.UI);
        colorPickWindow = new UIColorPickerWindow(c, true);
      }
      return colorPickWindow;
    }

    public UITexturePickerWindow OpenTextureEditWindow(string t, string current)
    {

      if (texturePickWindow != null)
      {
        Utils.Log("[WaterfallUI] Changing Texture Picker target", LogType.UI);
        texturePickWindow.ChangeTexture(t, current);
      }
      else
      {
        Utils.Log("[WaterfallUI] Opening Texture Picker", LogType.UI);
        texturePickWindow = new UITexturePickerWindow(t, current, true);
      }
      return texturePickWindow;
    }

    public string GetTextureFromPicker()
    {
      if (texturePickWindow != null)
        return texturePickWindow.GetTexturePath();
      else
      {
        return "";
      }


    }
    public Color GetColorFromPicker()
    {
      if (colorPickWindow != null)
        return colorPickWindow.GetColor();
      else
      {
        return Color.black;
      }


    }
    public void CopyEffect(WaterfallEffect toCopy)
    {
      selectedModule.CopyEffect(toCopy);
      RefreshEffectList();
    }

    public void OpenEffectAddWindow()
    {
      if (effectAddWindow == null)
      {
        effectAddWindow = new UIAddEffectWindow(true);

      }
      effectAddWindow.SetAddMode(selectedModule);
    }
    public void OpenEffectDeleteWindow(WaterfallEffect toDelete)
    {
      if (effectAddWindow == null)
      {
        effectAddWindow = new UIAddEffectWindow(true);

      }
      effectAddWindow.SetDeleteMode(selectedModule, toDelete);
    }

    public void OpenEffectModifierAddWindow(WaterfallEffect fx)
    {
      if (modifierPopupWindow == null)
      {
        modifierPopupWindow = new UIModifierPopupWindow(true);

      }
      modifierPopupWindow.SetAddMode(fx);
    }
    public void OpenEffectModifierDeleteWindow(WaterfallEffect fx, EffectModifier toDelete)
    {
      if (modifierPopupWindow == null)
      {
        modifierPopupWindow = new UIModifierPopupWindow(true);

      }
      modifierPopupWindow.SetDeleteMode(fx, toDelete);
    }



    public void UpdateCurve(FloatCurve curve)
    {
      currentModWinForCurve.UpdateCurves(curve, currentCurveTag);
    }

    public void Update()
    {
      for (int i = 0; i < effectsModules.Count; i++)
      {

        for (int j = 0; j < effectsModules[i].Controllers.Count; j++)
        {
          effectsModules[i].SetControllerOverride(effectsModules[i].Controllers[j].name, useControllers);
          if (effectsModules[i].Controllers[j].linkedTo == "throttle")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, throttleControllerValue);
          }
          if (effectsModules[i].Controllers[j].linkedTo == "atmosphere_density")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, densityControllerValue);
          }
          if (effectsModules[i].Controllers[j].linkedTo == "rcs")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, rcsControllerValue);
          }
          if (effectsModules[i].Controllers[j].linkedTo == "random")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, randomControllerValue.x);
          }


        }
      }
      for (int i = 0; i < effectUIWidgets.Count; i++)
      {
        
        effectUIWidgets[i].Update();
      }

      if (colorPickWindow != null)
      {
        colorPickWindow.Update();
      }
      if (texturePickWindow != null)
      {
        texturePickWindow.Update();
      }

      if (smokeEditWindow != null)
      {
        smokeEditWindow.Update();
      }
    }
  }

}