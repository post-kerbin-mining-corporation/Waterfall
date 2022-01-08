using System;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public enum ControllerPopupMode
  {
    Add,
    Delete,
    Modify
  }

  public class UIControllerPopupWindow : UIPopupWindow
  {
    private string windowTitle = string.Empty;
    private ControllerPopupMode windowMode;
    private WaterfallController control;
    private ModuleWaterfallFX fxMod;
    private string newControllerName = "controller";

    private readonly Type[] controllerTypes;
    private readonly string[] controllersGridValues;
    private readonly IEffectControllerUIOptions[] controllerOptions;

    private int selectedControllerIndex = 0;


    public UIControllerPopupWindow(bool show) : base(show)
    {
      if (!showWindow)
        WindowPosition = new Rect(Screen.width / 2f, Screen.height / 2f, 400, 400);

      var metadata = EffectControllersMetadata.EffectControllers.Values.OrderBy(v => v.DisplayName).ToArray();
      controllerTypes       = metadata.Select(c => c.ControllerType).ToArray();
      controllersGridValues = metadata.Select(c => c.DisplayName).ToArray();
      controllerOptions     = metadata.Select(c => c.CreateUIOptions(GUIResources)).ToArray();
    }

    public void SetDeleteMode(WaterfallController ctrl, ModuleWaterfallFX mod)
    {
      showWindow = true;
      control = ctrl;
      fxMod = mod;
      windowMode = ControllerPopupMode.Delete;
      GUI.BringWindowToFront(windowID);
    }

    public void SetEditMode(WaterfallController ctrl, ModuleWaterfallFX mod)
    {
      windowMode = ControllerPopupMode.Modify;
      showWindow = true;
      control = ctrl;
      fxMod = mod;
      newControllerName = ctrl.name;

      selectedControllerIndex = controllerTypes.IndexOf(ctrl.GetType());
      GUI.BringWindowToFront(windowID);

      controllerOptions[selectedControllerIndex].LoadOptions(ctrl);
    }

    public void SetAddMode(ModuleWaterfallFX mod)
    {
      showWindow = true;
      fxMod = mod;
      windowMode = ControllerPopupMode.Add;
      selectedControllerIndex = 0;

      GUI.BringWindowToFront(windowID);
    }

    protected override void InitUI()
    {
      windowTitle = windowMode switch
      {
        ControllerPopupMode.Add => "Add new Controller",
        ControllerPopupMode.Delete => "Confirm Delete",
        ControllerPopupMode.Modify => "Edit Controller",
        _ => windowTitle
      };

      base.InitUI();
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      switch (windowMode)
      {
        case ControllerPopupMode.Add:
          DrawAdd();
          break;
        case ControllerPopupMode.Delete:
          DrawDelete();
          break;
        case ControllerPopupMode.Modify:
          DrawModify();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(windowMode));
      }

      GUI.DragWindow();
    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
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

    protected void DrawDelete()
    {
      GUILayout.Label($"Are you sure you want to delete {control.name}?");
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Yes"))
      {
        fxMod.RemoveController(control);
        showWindow = false;
      }

      if (GUILayout.Button("No"))
      {
        showWindow = false;
      }

      GUILayout.EndHorizontal();
    }

    protected void DrawAdd()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller name");
      newControllerName = GUILayout.TextArea(newControllerName);
      GUILayout.EndHorizontal();
      GUILayout.Label("Controller type");

      UpdateControllerSelection();
      DrawControllerOptions();

      if (GUILayout.Button("Add"))
      {
        fxMod.AddController(CreateNewController());
        showWindow = false;
      }

      if (GUILayout.Button("Cancel"))
      {
        showWindow = false;
      }
    }

    protected void DrawModify()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller name");
      newControllerName = GUILayout.TextArea(newControllerName);
      GUILayout.EndHorizontal();
      GUILayout.Label("Controller type");

      UpdateControllerSelection();
      DrawControllerOptions();

      if (GUILayout.Button("Apply"))
      {
        fxMod.RemoveController(control);
        fxMod.AddController(CreateNewController());
        showWindow = false;
      }

      if (GUILayout.Button("Cancel"))
      {
        showWindow = false;
      }
    }

    private void UpdateControllerSelection()
    {
      selectedControllerIndex = GUILayout.SelectionGrid(selectedControllerIndex, controllersGridValues, Mathf.Min(controllersGridValues.Length, 4), GUIResources.GetStyle("radio_text_button"));
    }

    private WaterfallController CreateNewController()
    {
      var options = controllerOptions[selectedControllerIndex];
      var controller = options.CreateController();
      controller.name = newControllerName;

      return controller;
    }

    private void DrawControllerOptions()
    {
      var options = controllerOptions[selectedControllerIndex];
      options.DrawOptions();
    }
  }
}