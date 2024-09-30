using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Transform rotation modifier
  /// </summary>
  public class EffectRotationModifier : EffectModifier_Vector3
  {
    protected override string ConfigNodeName => WaterfallConstants.RotationModifierNodeName;

    private Vector3 baseRotation;

    public EffectRotationModifier() : base()
    {
      modifierTypeName = "Rotation";
    }

    public EffectRotationModifier(ConfigNode node) : base(node) { }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      baseRotation = xforms.Count == 0 ? Vector3.zero : xforms[0].localEulerAngles;
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectRotationIntegrator && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectRotationIntegrator(parentEffect, this);

  }
}
