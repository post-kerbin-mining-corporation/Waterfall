using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectLightFloatModifier : EffectModifier_Float
  {
    protected override string ConfigNodeName => WaterfallConstants.LightFloatModifierNodeName;

    [Persistent] public string floatName = "";
    
    private Light[] l;
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(floatName) && Settings.EnableLights;
    public override bool TestIntensity => floatName == "Intensity";

    public EffectLightFloatModifier() : base()
    {
      modifierTypeName = "Light Float";
    }

    public EffectLightFloatModifier(ConfigNode node) : base(node) { }

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

    public void ApplyFloatName(string newFloatName)
    {
      floatName = newFloatName;
      parentEffect.ModifierParameterChange(this);
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectLightFloatIntegrator i && i.floatName == floatName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectLightFloatIntegrator(parentEffect, this);
  }
}
