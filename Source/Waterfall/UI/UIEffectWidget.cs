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

      GUILayout.BeginHorizontal();
      GUILayout.BeginVertical();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Effect Name");
      GUILayout.Label(fx.name);
      GUILayout.EndHorizontal();

      GUILayout.Label("Model Properties");

      GUILayout.BeginHorizontal();
      if (GUILayout.Button("MODEL"))
      {
        // Opens model edit window
      }
      if (GUILayout.Button("MATERIAL"))
      {
        parent.OpenMaterialEditWindow(fx.FXModel);
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Position Offset");
      modelOffset = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(230f, 30f), modelOffset, modelOffsetString, GUI.skin.label, GUI.skin.textArea);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Rotation Offset");
      modelRotation = UIUtils.Vector3InputField(GUILayoutUtility.GetRect(230f, 30f), modelRotation, modelRotationString, GUI.skin.label, GUI.skin.textArea);
      
      GUILayout.EndHorizontal();
      
      GUILayout.EndVertical();

      GUILayout.BeginVertical();
      GUILayout.Label("Effect Modifiers");
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Add New"))
      {
        parent.OpenEffectModifierAddWindow(fx);
      }
      modifierListPosition = GUILayout.BeginScrollView(modifierListPosition, GUILayout.MinHeight(150f));
      for (int i=0; i< fx.FXModifiers.Count; i++)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label(fx.FXModifiers[i].fxName);
        GUILayout.FlexibleSpace();
        GUILayout.Label(fx.FXModifiers[i].controllerName);
        GUILayout.Label(fx.FXModifiers[i].effectMode.ToString());
        
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
