using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall.UI
{

  public class UIEffectWidget : UIWidget
  {
    Vector3 modelRotation;
    Vector3 modelOffset;
    Vector2 modifierListPosition = Vector2.zero;
    WaterfallEffect fx;
    WaterfallUI parent;

    bool showUI = true;
    bool enabled = true;
    string[] modelOffsetString;
    string[] modelRotationString;

    public UIEffectWidget(WaterfallUI uiHost, WaterfallEffect effect) : base(uiHost)
    {
      parent = uiHost;
      fx = effect;
      modelOffsetString = new string[] { effect.FXModel.modelPositionOffset.x.ToString(), effect.FXModel.modelPositionOffset.y.ToString(), effect.FXModel.modelPositionOffset.z.ToString() };
      modelRotationString = new string[] { effect.FXModel.modelRotationOffset.x.ToString(), effect.FXModel.modelRotationOffset.y.ToString(), effect.FXModel.modelRotationOffset.z.ToString() };

    }
    /// <summary>
    /// Do localization of UI strings
    /// </summary>
    protected override void Localize()
    {
      base.Localize();

    }

    /// <summary>
    /// Draw method
    /// </summary>
    public void Draw()
    {

      GUILayout.BeginHorizontal(GUI.skin.textArea);

      
      if (!showUI)
      {
        if (GUILayout.Button("[+]", GUILayout.ExpandHeight(true)))
          showUI = true;
        GUILayout.BeginVertical(GUILayout.MaxWidth(250f));
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b>{fx.name}</b>");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Drawn");
        enabled = GUILayout.Toggle(enabled, "");
        fx.SetEnabled(enabled);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Label($"<b>{fx.FXModifiers.Count} Effect Modifiers</b>");
        GUILayout.FlexibleSpace();
      }
      else
      {
        if (GUILayout.Button("[-]", GUILayout.ExpandHeight(true)))
          showUI = false;
        GUILayout.BeginVertical(GUILayout.MaxWidth(250f));
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b>{fx.name}</b>");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Drawn");
        enabled = GUILayout.Toggle(enabled, "");
        fx.SetEnabled(enabled);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("EDIT MATERIAL"))
        {
          parent.OpenMaterialEditWindow(fx.FXModel);
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Position Offset");
        modelOffset = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(230f, 30f), modelOffset, modelOffsetString, GUI.skin.label, GUI.skin.textArea);

        GUILayout.Label("Rotation Offset");
        modelRotation = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(230f, 30f), modelRotation, modelRotationString, GUI.skin.label, GUI.skin.textArea);


        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("<b>Effect Modifiers</b>");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add New"))
        {
          parent.OpenEffectModifierAddWindow(fx);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("    <b>Modifier Name</b>");
        GUILayout.FlexibleSpace();
        GUILayout.Label("<b>Controller</b>", GUILayout.Width(120));
        GUILayout.Label("<b>Mode</b>", GUILayout.Width(80));
        GUILayout.Space(80);
        GUILayout.EndHorizontal();
        modifierListPosition = GUILayout.BeginScrollView(modifierListPosition, GUILayout.MinHeight(150f));
        for (int i = 0; i < fx.FXModifiers.Count; i++)
        {
          GUILayout.BeginHorizontal(GUI.skin.textArea);
          GUILayout.Label(fx.FXModifiers[i].fxName);
          GUILayout.FlexibleSpace();
          GUILayout.Label(fx.FXModifiers[i].controllerName, GUILayout.Width(120));
          GUILayout.Label(fx.FXModifiers[i].effectMode.ToString(), GUILayout.Width(80));

          if (GUILayout.Button("Edit"))
          {
            parent.OpenModifierEditWindow(fx.FXModifiers[i]);
          }
          if (GUILayout.Button("x"))
          {
            parent.OpenEffectModifierDeleteWindow(fx, fx.FXModifiers[i]);
          }
          GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
      }
      GUILayout.EndHorizontal();
      
    }
    public void Update()
    {
      if (fx.FXModel.modelPositionOffset != modelOffset || fx.FXModel.modelRotationOffset != modelRotation)
      {
        fx.FXModel.ApplyOffsets(modelOffset, modelRotation);
      }
    }
  }
}
