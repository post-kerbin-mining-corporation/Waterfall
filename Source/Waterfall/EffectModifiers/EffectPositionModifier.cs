using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Transform scale modifier
  /// </summary>
  public class EffectPositionModifier : EffectModifier_Vector3
  {
    protected override string ConfigNodeName => WaterfallConstants.PositionModifierNodeName;

    private Vector3 basePosition;

    public EffectPositionModifier() : base()
    {
      modifierTypeName = "Position";
    }

    public EffectPositionModifier(ConfigNode node) : base(node) { }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      basePosition = xforms.Count == 0 ? Vector3.zero : xforms[0].localPosition;
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectPositionIntegrator && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectPositionIntegrator(parentEffect, this);

  }
}
