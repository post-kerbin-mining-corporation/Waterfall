using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIParticleMultiNumericModifierWindow : UIModifierWindow
  {
    public UIParticleMultiNumericModifierWindow(EffectParticleMultiNumericModifier mod, bool show) : base(mod, show)
    {
      particleMod = mod;
      paramName = particleMod.paramName;
      paramNames = ParticleUtils.FindValidParticlePropeties(particleMod.GetParticleSystem(), WaterfallParticlePropertyType.Numeric).ToArray();
      paramIndex = paramNames.ToList().FindIndex(x => x == particleMod.paramName);
      curve1Function = UpdateCurve1;
      curve2Function = UpdateCurve2;
      curve3Function = UpdateCurve3;
      curve4Function = UpdateCurve4;
      GenerateCurveThumbs(mod);
    }

    public override void UpdateCurves(FloatCurve newCurve, string tag)
    {
      base.UpdateCurves(newCurve, tag);
    }

    /// <summary>
    ///   Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      base.InitUI();
      windowTitle = "Modifier Editor - Particle Parameter";
    }

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void DrawModifierPanel()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Particle Parameter Name");
      int selectedIndex = GUILayout.SelectionGrid(paramIndex, paramNames, 4);
      if (selectedIndex != paramIndex)
      {
        paramIndex = selectedIndex;
        paramName = paramNames[paramIndex];
        //particleMod.ApplyParameterName(paramName);
      }
      GUILayout.EndHorizontal();

      GUILayout.Label($"<b>Selected Parameter Mode</b>: {particleMod.curveMode}");

      if (particleMod.curveMode == ParticleSystemCurveMode.Constant)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Parameter Curve", GUILayout.Width(hdrWidth));
        //var buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
        //var imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
        //if (GUI.Button(buttonRect, ""))
        //{
        //  EditCurve(particleMod.curve1, curve1Function);
        //  selectionFlag = 1;
        //}

        //GUI.DrawTexture(imageRect, miniCurve1);
        //if (GUILayout.Button("Copy", GUILayout.Width(copyWidth)))
        //{
        //  CopyCurve(particleMod.curve1);
        //}

        //if (GUILayout.Button("Paste", GUILayout.Width(copyWidth)))
        //{
        //  PasteCurve(particleMod.curve1, out particleMod.curve1);
        //}

        GUILayout.EndHorizontal();
      }

    }

    /// <summary>
    ///   Draws the modifier content
    /// </summary>
    protected override void UpdateModifierPanel()
    {
      GenerateCurveThumbs(particleMod);
    }

    protected void UpdateCurve1(FloatCurve curve)
    {
      particleMod.curve1 = curve;
      UpdateModifierPanel();
    }

    protected void UpdateCurve2(FloatCurve curve)
    {
      particleMod.curve2 = curve;
      UpdateModifierPanel();
    }

    protected void UpdateCurve3(FloatCurve curve)
    {
      UpdateModifierPanel();
    }

    protected void UpdateCurve4(FloatCurve curve)
    {
      UpdateModifierPanel();
    }

    protected void GenerateCurveThumbs(EffectParticleMultiNumericModifier pMod)
    {
      miniCurve1 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, pMod.curve1, Color.red);
      miniCurve2 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, pMod.curve2, Color.blue);
      miniCurve3 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, pMod.curve1, Color.green);
      miniCurve4 = GraphUtils.GenerateCurveTexture(texWidth, texHeight, pMod.curve2, Color.white);
    }

    #region GUI Variables

    private readonly Vector2 curveButtonDims = new(100f, 50f);

    private Texture2D miniCurve1;
    private Texture2D miniCurve2;
    private Texture2D miniCurve3;
    private Texture2D miniCurve4;

    #endregion

    #region GUI Widgets

    #endregion

    #region Data

    private string paramName = "_TintColor";
    private int paramIndex;
    private  string[] paramNames;
    public EffectParticleMultiNumericModifier particleMod;
    private readonly CurveUpdateFunction curve1Function;
    private readonly CurveUpdateFunction curve2Function;
    private readonly CurveUpdateFunction curve3Function;
    private readonly CurveUpdateFunction curve4Function;

    #endregion
  }
}