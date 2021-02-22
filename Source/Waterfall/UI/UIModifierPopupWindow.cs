using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall;

namespace Waterfall.UI
{
  public enum ModifierPopupMode
  {
    Add,
    Delete
  }
  public class UIModifierPopupWindow : UIPopupWindow
  {

    protected string windowTitle = "";
    ModifierPopupMode windowMode;
    EffectModifier modifier;
    WaterfallEffect effect;

    string newModifierName = "newModifier";
    string[] modifierTypes = new string[] { "Position", "Rotation", "Scale", "Material Color", "Material Float", "Light Material Color"};
    int modifierFlag = 0;

    string[] controllerTypes = new string[] { "Position", "Rotation", "Scale", "Material Color", "Material Float" };
    int controllerFlag = 0;

    int transformFlag = 0;
    string[] transformOptions;

    public UIModifierPopupWindow( bool show) : base(show)
    {
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2f, 750, 400);

    }

    public void SetDeleteMode(WaterfallEffect fx, EffectModifier mod)
    {
      showWindow = true;
      modifier = mod;
      effect = fx;
      windowMode = ModifierPopupMode.Delete;
      WindowPosition = new Rect(Screen.width / 2-100, Screen.height / 2f-50, 200, 100);
    }

    public void SetAddMode(WaterfallEffect fx)
    {
      showWindow = true;
      effect = fx;
      windowMode = ModifierPopupMode.Add;
      controllerTypes = fx.parentModule.GetControllerNames().ToArray();

      List<Transform> xFormOptions = fx.GetModelTransforms()[0].GetComponentsInChildren<Transform>().ToList();
      modifierFlag = 0;
      transformOptions = new string[xFormOptions.Count];
      for (int i=0;i < xFormOptions.Count; i++)
      {
        transformOptions[i] = xFormOptions[i].name;
      }
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2f, 750, 400);
    }

    protected override void InitUI()
    {
      if (windowMode == ModifierPopupMode.Add)
        windowTitle = "Add new Modifier";
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
      GUILayout.Label($"Are you sure you want to delete {modifier.fxName}?");
      GUILayout.BeginHorizontal();
      if(GUILayout.Button("Yes"))
      {
        effect.RemoveModifier(modifier);
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
      GUILayout.Label("Modifier name");
      newModifierName = GUILayout.TextArea(newModifierName);
      GUILayout.EndHorizontal();
      GUILayout.Label("Modifier type");
      int modiferFlagChanged = GUILayout.SelectionGrid(modifierFlag, modifierTypes, Mathf.Min(modifierTypes.Length,4), GUIResources.GetStyle("radio_text_button"));

      if (modiferFlagChanged != modifierFlag)
      {
        modifierFlag = modiferFlagChanged;
        if (modifierTypes[modifierFlag].Contains("Material"))
        {
          List<Renderer> xFormOptions = effect.GetModelTransforms()[0].GetComponentsInChildren<Renderer>().ToList();

          transformOptions = new string[xFormOptions.Count];
          for (int i=0;i < xFormOptions.Count; i++)
          {
            transformOptions[i] = xFormOptions[i].gameObject.name;
          }
        }
        else
        {
          List<Transform> xFormOptions = effect.GetModelTransforms()[0].GetComponentsInChildren<Transform>().ToList();

          transformOptions = new string[xFormOptions.Count];
          for (int i=0;i < xFormOptions.Count; i++)
          {
            transformOptions[i] = xFormOptions[i].name;
          }
        }
      }
      GUILayout.Label("Target transform name");
      transformFlag = GUILayout.SelectionGrid(transformFlag, transformOptions, Mathf.Min(transformOptions.Length,3), GUIResources.GetStyle("radio_text_button"));
      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller name");
      controllerFlag = GUILayout.SelectionGrid(controllerFlag, controllerTypes, Mathf.Min(controllerTypes.Length, 4), GUIResources.GetStyle("radio_text_button"));
      GUILayout.EndHorizontal();
      if (GUILayout.Button("Add"))
      {

        effect.AddModifier(CreateNewModifier());
        showWindow = false;
      }
      if (GUILayout.Button("Cancel"))
      {
        showWindow = false;
      }
    }

    EffectModifier CreateNewModifier()
    {
      if (modifierTypes[modifierFlag] == "Position")
      {
        EffectPositionModifier newMod = new EffectPositionModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Rotation")
      {
        EffectRotationModifier newMod = new EffectRotationModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Scale")
      {
        EffectScaleModifier newMod = new EffectScaleModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "UV Scroll")
      {
        EffectUVScrollModifier newMod = new EffectUVScrollModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Material Color")
      {
        EffectColorModifier newMod = new EffectColorModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Material Float")
      {
        EffectFloatModifier newMod = new EffectFloatModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Light Material Color")
      {
        EffectLightColorModifier newMod = new EffectLightColorModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      else
      {
        return null;
      }
    }
  }
}
