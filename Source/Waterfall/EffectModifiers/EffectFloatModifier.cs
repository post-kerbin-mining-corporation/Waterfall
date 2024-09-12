using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectFloatModifier : EffectModifier_Float
  {
    protected override string ConfigNodeName => WaterfallConstants.FloatModifierNodeName;

    [Persistent] public string floatName = "";
    private Material[] m;
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(floatName);

    public EffectFloatModifier() : base()
    {
      modifierTypeName = "Material Float";
    }

    public EffectFloatModifier(ConfigNode node) : base(node) { }

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

    public void ApplyFloatName(string newFloatName)
    {
      floatName = newFloatName;
      parentEffect.ModifierParameterChange(this);
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectFloatIntegrator i && i.floatName == floatName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectFloatIntegrator(parentEffect, this);

  }
}
