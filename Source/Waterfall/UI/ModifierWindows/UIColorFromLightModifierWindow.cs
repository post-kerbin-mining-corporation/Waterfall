
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIColorFromLightModifierWindow : UIModifierWindow
  {
    #region GUI Variables
    Vector2 curveButtonDims = new Vector2(100f, 50f);

    Texture2D miniCurve;

    #endregion

    #region GUI Widgets
    #endregion

    #region  Data
    string lightName = "Spot";
    int lightIndex;
    string[] lightNames;

    string colorName = "_TintColor";
    int colorIndex = 0;
    string[] colorNames;
    public EffectColorFromLightModifier colorMod;
    #endregion

    public UIColorFromLightModifierWindow(EffectColorFromLightModifier mod, bool show) : base((EffectModifier)mod, show)
    {
      colorMod = mod;
      colorNames = MaterialUtils.FindValidShaderProperties(colorMod.GetMaterial(), WaterfallMaterialPropertyType.Color).ToArray();
      colorIndex = colorNames.ToList().FindIndex(x => x == colorMod.colorName);

      lightNames = colorMod.parentEffect.parentModule.GetComponentsInChildren<Light>().Select(x => x.transform.name).ToArray();
      lightIndex = lightNames.ToList().FindIndex(x => x == colorMod.lightTransformName);
    }

    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Light Color Match";
    }
    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {

      GUILayout.BeginHorizontal();
      GUILayout.Label("Light Transform Name");
      int selectedIndex = GUILayout.SelectionGrid(lightIndex, lightNames, 4);
      if (selectedIndex != colorIndex)
      {
        lightIndex = selectedIndex;
        lightName = lightNames[lightIndex];
        colorMod.ApplyLightName(lightName);
      }
      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label("Shader Color Name");
      selectedIndex = GUILayout.SelectionGrid(colorIndex, colorNames, 4);
      if (selectedIndex != colorIndex)
      {
        colorIndex = selectedIndex;
        colorName = colorNames[colorIndex];
        colorMod.ApplyColorName(colorName);
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Color Saturation");
      float sliderVal = GUILayout.HorizontalSlider(colorMod.colorBlend, 0f, 1f, GUILayout.Width(100f));
      GUILayout.Label(sliderVal.ToString("F3"));
      if (sliderVal != colorMod.colorBlend)
      {
        colorMod.colorBlend = sliderVal;
        
      }
        GUILayout.EndHorizontal();
    }
    public override void UpdateCurves(FloatCurve newCurve, string tag)
    {
     
    }

    /// <summary>
    /// Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel()
    {
    }
   


  }

}
