using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectColorModifier : EffectModifier_Color
  {
    protected override string ConfigNodeName => WaterfallConstants.ColorModifierNodeName;

    public Gradient colorCurve;
    [Persistent] public string colorName;

    private Material[] m;
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(colorName);

    public EffectColorModifier() : base()
    {
      modifierTypeName = "Material Color";
    }

    public EffectColorModifier(ConfigNode node) : base(node) { }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      m = new Material[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
      }
    }

    public Material GetMaterial() => m[0];

    public void ApplyMaterialName(string newColorName)
    {
      colorName = newColorName;
      parentEffect.ModifierParameterChange(this);
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectColorIntegrator i && i.colorName == colorName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectColorIntegrator(parentEffect, this);
  }
}
