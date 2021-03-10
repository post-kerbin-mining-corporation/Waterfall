
using System;
using System.Collections;
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


    public Vector3 modelRotation;
    public Vector3 modelOffset;
    public Vector3 modelScale = Vector3.one;
    string templateName = "";
    string[] modelOffsetString;
    string[] modelRotationString;
    string[] modelScaleString;

    int selectedModuleIndex = 0;
    int selectedTemplateIndex = 0;

    public WaterfallEffectTemplate selectedTemplate;
    #endregion

    #region GUI Widgets
    UICurveEditWindow curveEditWindow;
    UIModifierPopupWindow modifierPopupWindow;
    UIAddEffectWindow effectAddWindow;
    UIControllerPopupWindow controlAddWindow;
    UIModifierWindow currentModWinForCurve;
    UIMaterialEditWindow materialEditWindow;
    UILightEditWindow lightEditWindow;
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
      StartCoroutine(DelayedStart());

    }

    IEnumerator DelayedStart()
    {
      yield return 5;
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
      if (lightEditWindow != null)
      {
        lightEditWindow.Draw();
      }
      if (modifierPopupWindow != null)
      {
        modifierPopupWindow.Draw();
      }
      if (effectAddWindow != null)
      {
        effectAddWindow.Draw();
      }
      if (controlAddWindow != null)
      {
        controlAddWindow.Draw();
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

      if (selectedModule != null)
      {
        DrawHeader();
        GUILayout.BeginHorizontal();
        DrawPartsList();
        DrawExporters();
        GUILayout.EndHorizontal();
        //
        GUILayout.BeginHorizontal();
        DrawControllers();
        DrawTemplateControl();

        GUILayout.EndHorizontal();
        // Draw the parts list


        // Draw the effects list


        DrawEffectsList();
      }
      else
      {
        GUILayout.Label("Couldn't find any effects modules on this craft, add modules via config before using this editor..");
      }
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


    }

    void DrawController(WaterfallController ctrl)
    {
      GUILayout.BeginHorizontal();


      ctrl.overridden = GUILayout.Toggle(ctrl.overridden, "", GUILayout.Width(60));

      GUILayout.Label(ctrl.name, GUILayout.MaxWidth(120f));
      //GUILayout.Label(ctrl.linkedTo, GUILayout.MaxWidth(130f));


      float sliderMax = 1f;
      float sliderMin = 0f;
      if (ctrl.linkedTo == "mach")
        sliderMax = 15f;
      if (ctrl.linkedTo == "gimbal")
      {
        sliderMin = -1f;
      }

      if (ctrl.overridden)
      {
        ctrl.overrideValue = GUILayout.HorizontalSlider(ctrl.overrideValue, sliderMin, sliderMax, GUILayout.MaxWidth(100f));
      }
      else
      {
        ctrl.overrideValue = GUILayout.HorizontalSlider(ctrl.Get()[0], 0f, sliderMax, GUILayout.MaxWidth(100f));
      }
      GUILayout.Label(ctrl.overrideValue.ToString("F2"), GUILayout.MinWidth(40f));

      if (GUILayout.Button("Edit", GUILayout.Width(30)))
      {
        OpenControllerEditWindow(ctrl);
      }
      if (GUILayout.Button("x", GUILayout.Width(20)))
      {
        OpenControllerDeleteWindow(ctrl);
      }

      GUILayout.EndHorizontal();
    }
    protected void DrawControllers()
    {

      GUILayout.BeginVertical();
      GUILayout.Label("<b>CONTROLLERS</b>");
      GUILayout.BeginVertical(GUI.skin.textArea);
      // Title bar
      if (selectedModule != null)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("<b>Override</b>", GUILayout.Width(60));
        GUILayout.Label("  <b>Name</b>", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        GUILayout.Space(140);
        if (GUILayout.Button("Add New"))
        {
          OpenControllerAddWindow();
        }
        GUILayout.EndHorizontal();

        foreach (WaterfallController ctrl in selectedModule.Controllers)
        {
          DrawController(ctrl);
        }
        GUILayout.EndVertical();
        GUILayout.EndVertical();
      }
    }

    protected bool templatesOpen = false;
    protected bool exportsOpen = false;
    protected void DrawTemplateControl()
    {
      GUILayout.BeginVertical();
      if (selectedModule.Templates != null && selectedModule.Templates.Count > 0)
      {
        GUILayout.Label("<b>TEMPLATES</b>");
        GUILayout.BeginVertical(GUI.skin.textArea);

        GUILayout.BeginHorizontal();

        int selectedTemplateChanges = GUILayout.SelectionGrid(selectedTemplateIndex, templatesString, 4, GUIResources.GetStyle("radio_text_button"));

        if (selectedTemplateChanges != selectedTemplateIndex)
        {
          selectedTemplateIndex = selectedTemplateChanges;
          SelectTemplate(selectedModule.Templates[selectedTemplateIndex]);
        }

        if (GUILayout.Button("Copy offsets", GUILayout.Width(160f), GUILayout.Height(40)))
        {
          string copiedString = "";
          copiedString += $"position = {selectedTemplate.position.x},{selectedTemplate.position.y},{selectedTemplate.position.z}\n";
          copiedString += $"rotation = {selectedTemplate.rotation.x}, {selectedTemplate.rotation.y}, {selectedTemplate.rotation.z}\n";
          copiedString += $"scale = {selectedTemplate.scale.x}, {selectedTemplate.scale.y}, {selectedTemplate.scale.z}";

          GUIUtility.systemCopyBuffer = copiedString;
        }
        GUILayout.EndHorizontal();

        GUILayout.Label(selectedModule.Templates[selectedTemplateIndex].templateName);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Offset");
        modelOffset = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(200f, 30f), modelOffset, modelOffsetString, GUI.skin.label, GUI.skin.textArea);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotation ");
        modelRotation = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(200f, 30f), modelRotation, modelRotationString, GUI.skin.label, GUI.skin.textArea);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scale");
        modelScale = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(200f, 30f), modelScale, modelScaleString, GUI.skin.label, GUI.skin.textArea);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
      }
      GUILayout.EndVertical();
    }
    protected void DrawExporters()
    {
      GUILayout.BeginVertical();
      GUILayout.Label("<b>EXPORT</b>");
      GUILayout.BeginVertical(GUI.skin.textArea);
      //if (exportsOpen)
      //{
      //  if (GUILayout.Button("-", GUILayout.Width(30)))
      //  {
      //    exportsOpen = false;
      //  }
      //}
      //else
      //{
      //  if (GUILayout.Button("+", GUILayout.Width(30)))
      //  {
      //    exportsOpen = true;
      //  }
      //}
      exportsOpen = true;


      if (exportsOpen)
      {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Module and \ncopy to clipboard", GUILayout.Width(170f), GUILayout.Height(40)))
        {
          GUIUtility.systemCopyBuffer = (selectedModule.ExportModule().ToString());

        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();


        if (GUILayout.Button("Generate template from\n and copy to clipboard", GUILayout.Width(170f), GUILayout.Height(60)))
        {
          ConfigNode node = new ConfigNode(WaterfallConstants.TemplateLibraryNodeName);
          if (templatesString != null && templatesString.Length > 0)
          {

            node.AddValue("templateName", templateName);
            foreach (WaterfallEffect fx in selectedTemplate.allFX)
            {
              node.AddNode(fx.Save());
            }
          }
          else
          {
            node.AddValue("templateName", templateName);
            foreach (WaterfallEffect fx in selectedModule.FX)
            {
              node.AddNode(fx.Save());
            }
          }

          GUIUtility.systemCopyBuffer = (node.ToString());

        }
        GUILayout.BeginVertical();
        GUILayout.Label("Exported template name");
        templateName = GUILayout.TextArea(templateName);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
      GUILayout.EndVertical();
    }

    protected void DrawPartsList()
    {
      GUILayout.BeginVertical();
      GUILayout.Label("<b>FX MODULES ON VESSEL</b>");

      partsScrollListPosition = GUILayout.BeginScrollView(partsScrollListPosition, GUILayout.Width(340f));


      int selectedModuleChanges = GUILayout.SelectionGrid(selectedModuleIndex, modulesString, Mathf.Min(modulesString.Length, 2), GUIResources.GetStyle("radio_text_button"));

      if (selectedModuleChanges != selectedModuleIndex)
      {
        selectedModuleIndex = selectedModuleChanges;
        SelectFXModule(effectsModules[selectedModuleIndex]);
      }

      GUILayout.EndScrollView();
      GUILayout.EndVertical();
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

    public void SelectTemplate(WaterfallEffectTemplate template)
    {
      selectedTemplate = template;
      effectUIWidgets.Clear();
      foreach (WaterfallEffect fx in selectedTemplate.allFX)
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

    public void RefreshEffectList()
    {
      effectUIWidgets.Clear();
      if (selectedModule.Templates.Count > 0)
      {
        selectedTemplateIndex = 0;

        templatesString = new string[selectedModule.Templates.Count];
        if (selectedModule.Templates.Count > 0)
        {
          for (int i = 0; i < selectedModule.Templates.Count; i++)
          {
            templatesString[i] = $"{i}";
          }
          SelectTemplate(selectedModule.Templates[0]);

        }
      }
      else
      {
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
    }

    string[] modulesString;
    string[] templatesString;
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
        modulesString = new string[effectsModules.Count];
        if (effectsModules.Count > 0)
        {
          for (int i = 0; i < effectsModules.Count; i++)
          {
            modulesString[i] = $"{effectsModules[i].moduleID} ({effectsModules[i].FX.Count} Effects)";
          }
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

      try
      {
        EffectColorFromLightModifier colMod = (EffectColorFromLightModifier)fxMod;
        if (colMod != null)
        {
          editWindows.Add(new UIColorFromLightModifierWindow(colMod, true));
        }

      }
      catch (InvalidCastException e) { }

      try
      {
        EffectLightFloatModifier colMod = (EffectLightFloatModifier)fxMod;
        if (colMod != null)
        {
          editWindows.Add(new UILightFloatModifierWindow(colMod, true));
        }

      }
      catch (InvalidCastException e) { }
      try
      {
        EffectLightColorModifier colMod = (EffectLightColorModifier)fxMod;
        if (colMod != null)
        {
          editWindows.Add(new UILightColorModifierWindow(colMod, true));
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

    public UICurveEditWindow OpenCurveEditor(FloatCurve toEdit, CurveUpdateFunction curveFun)
    {
      if (curveEditWindow != null)
      {
        curveEditWindow.ChangeCurve(toEdit, curveFun);
      }
      else
      {
        curveEditWindow = new UICurveEditWindow(toEdit, curveFun, true);
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
    public UILightEditWindow OpenLightEditWindow(WaterfallModel mdl)
    {

      if (lightEditWindow != null)
      {
        lightEditWindow.ChangeLight(mdl);
      }
      else
      {
        lightEditWindow = new UILightEditWindow(mdl, true);
      }
      return lightEditWindow;
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
      selectedModule.CopyEffect(toCopy, selectedTemplate);
      RefreshEffectList();
    }

    public void OpenControllerAddWindow()
    {
      if (controlAddWindow == null)
      {
        controlAddWindow = new UIControllerPopupWindow(true);

      }
      controlAddWindow.SetAddMode(selectedModule);
    }
    public void OpenControllerDeleteWindow(WaterfallController toDelete)
    {
      if (controlAddWindow == null)
      {
        controlAddWindow = new UIControllerPopupWindow(true);

      }
      controlAddWindow.SetDeleteMode(toDelete, selectedModule);
    }

    public void OpenControllerEditWindow(WaterfallController toEdit)
    {
      if (controlAddWindow == null)
      {
        controlAddWindow = new UIControllerPopupWindow(true);

      }
      controlAddWindow.SetEditMode(toEdit, selectedModule);
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
      // update the curve
      currentModWinForCurve.UpdateCurves(curve, currentCurveTag);
    }

    public void Update()
    {

      //}
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

      if (selectedTemplate != null)
      {
        if (modelOffset != selectedTemplate.position || modelRotation != selectedTemplate.rotation || modelScale != selectedTemplate.scale)
        {
          selectedTemplate.position = modelOffset;
          selectedTemplate.rotation = modelRotation;
          selectedTemplate.scale = modelScale;

        }
      }
    }
  }

}