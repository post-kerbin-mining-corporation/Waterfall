using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIAddEffectWindow : UIPopupWindow
  {
    protected string            windowTitle = "";
    private   ModifierPopupMode windowMode;
    private   WaterfallEffect   effect;
    private   ModuleWaterfallFX module;

    private string parentName    = "";
    private string newEffectName = "newEffect";

    private int      workflowFlag;
    private string[] workflowOptions;

    private int      shaderFlag;
    private string[] shaderOptions;

    private int                  modelFlag;
    private string[]             modelOptions;
    private List<WaterfallAsset> models;
    private List<WaterfallAsset> shaders;

    private bool randomizeSeed = true;

    public UIAddEffectWindow(bool show) : base(show)
    {
      WindowPosition = new(Screen.width / 2, Screen.height / 2f, 750, 400);
    }

    public void SetDeleteMode(ModuleWaterfallFX fxModule, WaterfallEffect fx)
    {
      showWindow     = true;
      module         = fxModule;
      effect         = fx;
      windowMode     = ModifierPopupMode.Delete;
      WindowPosition = new(Screen.width / 2 - 100, Screen.height / 2f - 50, 200, 100);
    }

    public void SetAddMode(ModuleWaterfallFX fxModule)
    {
      showWindow = true;
      module     = fxModule;
      windowMode = ModifierPopupMode.Add;

      workflowOptions = Enum.GetNames(typeof(AssetWorkflow)).ToArray();
      workflowFlag    = 0;
      var modelOpts = new List<string>();
      models = WaterfallAssets.GetModels((AssetWorkflow)Enum.Parse(typeof(AssetWorkflow), workflowOptions[workflowFlag]));
      foreach (var w in models)
      {
        modelOpts.Add($"<b>{w.Name}</b>\n{w.Description}\n{w.Path}");
      }

      modelOptions = modelOpts.ToArray();

      var shaderOpts = new List<string>();
      shaders = WaterfallAssets.GetShaders((AssetWorkflow)Enum.Parse(typeof(AssetWorkflow), workflowOptions[workflowFlag]));
      foreach (var w in shaders)
      {
        shaderOpts.Add($"<b>{w.Name}</b>\n{w.Description}");
      }

      shaderOptions  = shaderOpts.ToArray();
      WindowPosition = new(Screen.width / 2, Screen.height / 2f, 750, 400);
    }

    protected override void InitUI()
    {
      if (windowMode == ModifierPopupMode.Add)
        windowTitle = "Add new Effect";
      if (windowMode == ModifierPopupMode.Delete)
        windowTitle = "Confirm Delete";
      base.InitUI();
    }


    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      if (windowMode == ModifierPopupMode.Add)
        DrawAdd();
      if (windowMode == ModifierPopupMode.Delete)
        DrawDelete();
      GUI.DragWindow();
    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, UIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f), GUILayout.MinWidth(350f));

      GUILayout.FlexibleSpace();

      var buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = UIResources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", UIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, UIResources.GetIcon("cancel").iconAtlas, UIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected void DrawDelete()
    {
      GUILayout.Label($"Are you sure you want to delete {effect.name}?");
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Yes"))
      {
        module.RemoveEffect(effect);
        WaterfallUI.Instance.RefreshEffectList();
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
      GUILayout.Label("Effect Name", GUILayout.Width(120f));
      newEffectName = GUILayout.TextArea(newEffectName);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Effect Parent", GUILayout.Width(120f));
      parentName = GUILayout.TextArea(parentName);
      GUILayout.EndHorizontal();


      GUILayout.Label("<b>SELECT WORKFLOW</b>", GUILayout.Width(120f));
      int newFlag = GUILayout.SelectionGrid(workflowFlag,
                                            workflowOptions,
                                            Mathf.Min(workflowOptions.Length, 4),
                                            UIResources.GetStyle("radio_text_button"));


      if (newFlag != workflowFlag)
      {
        workflowFlag = newFlag;

        var modelOpts = new List<string>();
        models = WaterfallAssets.GetModels((AssetWorkflow)Enum.Parse(typeof(AssetWorkflow), workflowOptions[workflowFlag]));
        foreach (var w in models)
        {
          modelOpts.Add($"<b>{w.Name}</b>\n{w.Description}\n{w.Path}");
        }

        modelOptions = modelOpts.ToArray();

        var shaderOpts = new List<string>();
        shaders = WaterfallAssets.GetShaders((AssetWorkflow)Enum.Parse(typeof(AssetWorkflow), workflowOptions[workflowFlag]));
        foreach (var w in shaders)
        {
          shaderOpts.Add($"<b>{w.Name}</b>\n{w.Description}");
        }

        shaderOptions = shaderOpts.ToArray();

        modelFlag  = 0;
        shaderFlag = 0;
      }

      GUILayout.BeginVertical(GUI.skin.textArea);

      GUILayout.BeginHorizontal();
      GUILayout.Label("<b>Model</b>", GUILayout.Width(120f));
      modelFlag = GUILayout.SelectionGrid(modelFlag,
                                          modelOptions,
                                          Mathf.Min(modelOptions.Length, 2),
                                          UIResources.GetStyle("radio_text_button"));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("<b>Shader</b>", GUILayout.Width(120f));
      shaderFlag = GUILayout.SelectionGrid(shaderFlag,
                                           shaderOptions,
                                           Mathf.Min(shaderOptions.Length, 2),
                                           UIResources.GetStyle("radio_text_button"));
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
      GUILayout.BeginHorizontal();

      randomizeSeed = GUILayout.Toggle(randomizeSeed, "Randomize Effect Seed");

      GUILayout.EndHorizontal();

      if (GUILayout.Button("Add"))
      {
        var modelXforms = module.GetComponentsInChildren<Transform>();

        if (modelXforms.ToList().FindAll(x => x.name == parentName).Any())
        {
          module.AddEffect(CreateNewEffect());
          WaterfallUI.Instance.RefreshEffectList();
          showWindow = false;
        }
      }

      if (GUILayout.Button("Cancel"))
      {
        showWindow = false;
      }
    }

    private WaterfallEffect CreateNewEffect()
    {
      WaterfallModel model;
      if (shaders.Count == 0)
      {
        model = new(models[modelFlag],
                    null,
                    randomizeSeed);
      }
      else
      {
        model = new(models[modelFlag],
                    shaders[shaderFlag],
                    randomizeSeed);
      }

      WaterfallEffect newFX;
      if (WaterfallUI.Instance.selectedTemplate == null)
      {
        Utils.Log("[UIAddEffectWindow]: Creating effect", LogType.UI);
        newFX = new(parentName, model);
      }
      else
      {
        Utils.Log($"[UIAddEffectWindow]: Creating effect as part of template {WaterfallUI.Instance.selectedTemplate.templateName}", LogType.UI);
        newFX = new(parentName, model, WaterfallUI.Instance.selectedTemplate);
      }

      newFX.name = newEffectName;

      return newFX;
    }
  }
}