using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall.UI
{
  public enum ModifierPopupMode
  {
    Add,
    Delete
  }

  public class UIModifierPopupWindow : UIPopupWindow
  {
    protected        string            windowTitle   = "";
    private readonly string[]          modifierTypes = { 
      "Position", 
      "Rotation", 
      "Scale", 
      "Material Color", 
      "Material Float", 
      "Light Material Color", 
      "Light Float", 
      "Light Color", 
      "Particle Numeric"
    };
    private          ModifierPopupMode windowMode;
    private          EffectModifier    modifier;
    private          WaterfallEffect   effect;

    private string newModifierName = "newModifier";
    private int    modifierFlag;

    private string[] controllerTypes = { "Position", "Rotation", "Scale", "Material Color", "Material Float" };
    private int      controllerFlag;

    private int      transformFlag;
    private string[] transformOptions;

    public UIModifierPopupWindow(bool show) : base(show)
    {
      WindowPosition = new(Screen.width / 2, Screen.height / 2f, 750, 400);
    }

    public void SetDeleteMode(WaterfallEffect fx, EffectModifier mod)
    {
      showWindow = true;
      modifier   = mod;
      effect     = fx;
      windowMode = ModifierPopupMode.Delete;
      GUI.BringWindowToFront(windowID);
    }

    public void SetAddMode(WaterfallEffect fx)
    {
      showWindow      = true;
      effect          = fx;
      windowMode      = ModifierPopupMode.Add;
      controllerTypes = fx.parentModule.GetControllerNames().ToArray();

      var xFormOptions = fx.GetModelTransforms()[0].GetComponentsInChildren<Transform>().ToList();
      modifierFlag     = 0;
      transformOptions = new string[xFormOptions.Count];
      for (int i = 0; i < xFormOptions.Count; i++)
      {
        transformOptions[i] = xFormOptions[i].name;
      }

      GUI.BringWindowToFront(windowID);
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
      GUILayout.Label($"Are you sure you want to delete {modifier.fxName}?");
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Yes"))
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
      int modiferFlagChanged = GUILayout.SelectionGrid(modifierFlag, modifierTypes, Mathf.Min(modifierTypes.Length, 4), UIResources.GetStyle("radio_text_button"));

      if (modiferFlagChanged != modifierFlag)
      {
        modifierFlag = modiferFlagChanged;
        if (modifierTypes[modifierFlag].Contains("Material"))
        {
          var xFormOptions = effect.GetModelTransforms()[0].GetComponentsInChildren<Renderer>().ToList();

          transformOptions = new string[xFormOptions.Count];
          for (int i = 0; i < xFormOptions.Count; i++)
          {
            transformOptions[i] = xFormOptions[i].gameObject.name;
          }
        }
        else if (modifierTypes[modifierFlag].Contains("Light"))
        {
          var xFormOptions = effect.GetModelTransforms()[0].GetComponentsInChildren<Light>().ToList();

          transformOptions = new string[xFormOptions.Count];
          for (int i = 0; i < xFormOptions.Count; i++)
          {
            transformOptions[i] = xFormOptions[i].gameObject.name;
          }
        }
        else if (modifierTypes[modifierFlag].Contains("Particle"))
        {
          List<ParticleSystem> xFormOptions = effect.GetModelTransforms()[0].GetComponentsInChildren<ParticleSystem>().ToList();

          transformOptions = new string[xFormOptions.Count];
          for (int i = 0; i < xFormOptions.Count; i++)
          {
            transformOptions[i] = xFormOptions[i].gameObject.name;
          }
        }
        else
        {
          var xFormOptions = effect.GetModelTransforms()[0].GetComponentsInChildren<Transform>().ToList();

          transformOptions = new string[xFormOptions.Count];
          for (int i = 0; i < xFormOptions.Count; i++)
          {
            transformOptions[i] = xFormOptions[i].name;
          }
        }

        transformFlag = 0;
      }

      GUILayout.Label("Target transform name");
      transformFlag = GUILayout.SelectionGrid(transformFlag, transformOptions, Mathf.Min(transformOptions.Length, 3), UIResources.GetStyle("radio_text_button"));
      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller name");
      controllerFlag = GUILayout.SelectionGrid(controllerFlag, controllerTypes, Mathf.Min(controllerTypes.Length, 4), UIResources.GetStyle("radio_text_button"));
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

    private EffectModifier CreateNewModifier()
    {
      if (modifierTypes[modifierFlag] == "Position")
      {
        var newMod = new EffectPositionModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "Rotation")
      {
        var newMod = new EffectRotationModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "Scale")
      {
        var newMod = new EffectScaleModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "UV Scroll")
      {
        var newMod = new EffectUVScrollModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "Material Color")
      {
        var newMod = new EffectColorModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "Material Float")
      {
        var newMod = new EffectFloatModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "Light Material Color")
      {
        var newMod = new EffectLightColorModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "Light Float")
      {
        var newMod = new EffectLightFloatModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }

      if (modifierTypes[modifierFlag] == "Light Color")
      {
        var newMod = new EffectLightColorModifier();
        newMod.fxName         = newModifierName;
        newMod.transformName  = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      if (modifierTypes[modifierFlag] == "Particle Numeric")
      {
        var newMod = new EffectParticleMultiNumericModifier();
        newMod.fxName = newModifierName;
        newMod.transformName = transformOptions[transformFlag];
        newMod.controllerName = controllerTypes[controllerFlag];
        return newMod;
      }
      return null;
    }
  }
}