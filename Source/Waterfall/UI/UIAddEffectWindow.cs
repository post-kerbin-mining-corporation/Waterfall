using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall;

namespace Waterfall.UI
{
  
  public class UIAddEffectWindow : UIPopupWindow
  {

    protected string windowTitle = "";
    ModifierPopupMode windowMode;
    WaterfallEffect effect;
    ModuleWaterfallFX module;

    string parentName = "";
    string newEffectName = "newEffect";
    string[] shaderOptions;
    int shaderFlag = 0;

    int modelFlag = 0;
    string[] modelOptions;

    public UIAddEffectWindow( bool show) : base(show)
    {
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2f, 750, 400);

    }

    public void SetDeleteMode(ModuleWaterfallFX fxModule, WaterfallEffect fx)
    {
      showWindow = true;
      module = fxModule;
      effect = fx;
      windowMode = ModifierPopupMode.Delete;
      WindowPosition = new Rect(Screen.width / 2-100, Screen.height / 2f-50, 200, 100);
    }

    public void SetAddMode(ModuleWaterfallFX fxModule)
    {
      showWindow = true;
      module = fxModule;
      windowMode = ModifierPopupMode.Add;

      modelOptions = WaterfallAssets.Models.Keys.ToArray();
      shaderOptions = ShaderLoader.GetAllShadersNames().ToArray();
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2f, 750, 400);
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
      GUILayout.Label($"Are you sure you want to delete {effect.name}?");
      GUILayout.BeginHorizontal();
      if(GUILayout.Button("Yes"))
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

      GUILayout.BeginHorizontal();
      GUILayout.Label("Effect Model", GUILayout.Width(120f));
      modelFlag = GUILayout.SelectionGrid(modelFlag, 
        modelOptions, Mathf.Min(modelOptions.Length,2),
        GUIResources.GetStyle("radio_text_button"));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Shader", GUILayout.Width(120f));
      shaderFlag = GUILayout.SelectionGrid(shaderFlag,
        shaderOptions, Mathf.Min(shaderOptions.Length, 2),
        GUIResources.GetStyle("radio_text_button"));
      GUILayout.EndHorizontal();


      if (GUILayout.Button("Add"))
      {
        Transform[] modelXforms = module.GetComponentsInChildren<Transform>();
        
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

    WaterfallEffect CreateNewEffect()
    {
      WaterfallModel model = ScriptableObject.CreateInstance<WaterfallModel>();
      model.SetupModel(modelOptions[modelFlag], shaderOptions[shaderFlag]);     
      
      WaterfallEffect newFX = ScriptableObject.CreateInstance<WaterfallEffect>();
      newFX.SetupEffect(parentName, model);
      newFX.name = newEffectName;

      return newFX;
    }
  }
}
