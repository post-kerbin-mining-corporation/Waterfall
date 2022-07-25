using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectLightColorModifier : EffectModifier_Color
  {
    protected override string ConfigNodeName => WaterfallConstants.LightColorModifierNodeName;

    [Persistent] public string colorName = "_Main";

    private Light[] l;
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(colorName);

    public EffectLightColorModifier() : base()
    {
      modifierTypeName = "Light Color";
    }

    public EffectLightColorModifier(ConfigNode node) : base(node) { }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      l = new Light[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();
      }
    }

    public Light GetLight() => l[0];

    public void ApplyMaterialName(string newColorName)
    {
      colorName = newColorName;
      parentEffect.ModifierParameterChange(this);
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectLightColorIntegrator i && i.colorName == colorName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectLightColorIntegrator(parentEffect, this);
  }
}
