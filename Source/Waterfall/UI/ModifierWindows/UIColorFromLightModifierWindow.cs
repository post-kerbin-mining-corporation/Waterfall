using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIColorFromLightModifierWindow : UIModifierWindow
  {
    public UIColorFromLightModifierWindow(EffectColorFromLightModifier mod, bool show) : base(mod, show)
    {
      colorMod   = mod;
      colorNames = MaterialUtils.FindValidShaderProperties(colorMod.GetMaterial(), WaterfallMaterialPropertyType.Color).ToArray();
      colorIndex = colorNames.ToList().FindIndex(x => x == colorMod.colorName);

      lightNames = colorMod.parentEffect.parentModule.GetComponentsInChildren<Light>().Select(x => x.transform.name).ToArray();
      lightIndex = lightNames.ToList().FindIndex(x => x == colorMod.lightTransformName);
    }

    public override void UpdateCurves(FastFloatCurve newCurve, string tag) { }

    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Light Color Match";
    }

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Light Transform Name");
      int selectedIndex = GUILayout.SelectionGrid(lightIndex, lightNames, 4);
      if (selectedIndex != lightIndex)
      {
        lightIndex = selectedIndex;
        lightName  = lightNames[lightIndex];
        colorMod.ApplyLightName(lightName);
      }

      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label("Shader Color Name");
      selectedIndex = GUILayout.SelectionGrid(colorIndex, colorNames, 4);
      if (selectedIndex != colorIndex)
      {
        colorIndex = selectedIndex;
        colorName  = colorNames[colorIndex];
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

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel() { }

    #region GUI Widgets

    #endregion

    #region Data

    private          string   lightName = "Spot";
    private          int      lightIndex;
    private readonly string[] lightNames;

    private          string                       colorName = "_TintColor";
    private          int                          colorIndex;
    private readonly string[]                     colorNames;
    public           EffectColorFromLightModifier colorMod;

    #endregion
  }
}