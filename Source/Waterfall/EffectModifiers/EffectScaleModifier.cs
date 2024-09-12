using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Transform scale modifier
  /// </summary>
  public class EffectScaleModifier : EffectModifier_Vector3
  {
    protected override string ConfigNodeName => WaterfallConstants.ScaleModifierNodeName;

    private Vector3 baseScale;

    public EffectScaleModifier() : base()
    {
      modifierTypeName = "Scale";
    }

    public EffectScaleModifier(ConfigNode node) : base(node) { }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      baseScale = xforms[0].localScale;
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectScaleIntegrator && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectScaleIntegrator(parentEffect, this);

  }
}
