using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Transform scale modifier
  /// </summary>
  public class EffectScaleModifier : EffectModifier
  {
    public FloatCurve xCurve = new();
    public FloatCurve yCurve = new();
    public FloatCurve zCurve = new();

    private Vector3 baseScale;

    public EffectScaleModifier() : base()
    {
      modifierTypeName = "Scale";
    }

    public EffectScaleModifier(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      xCurve.Load(node.GetNode("xCurve"));
      yCurve.Load(node.GetNode("yCurve"));
      zCurve.Load(node.GetNode("zCurve"));
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.name = WaterfallConstants.ScaleModifierNodeName;

      node.AddNode(Utils.SerializeFloatCurve("xCurve", xCurve));
      node.AddNode(Utils.SerializeFloatCurve("yCurve", yCurve));
      node.AddNode(Utils.SerializeFloatCurve("zCurve", zCurve));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      baseScale = xforms[0].localScale;
    }

    public List<Vector3> Get(List<float> input, List<Vector3> output) => Get(input, output, xCurve, yCurve, zCurve);

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectScaleIntegrator && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectScaleIntegrator(parentEffect, this);

  }
}
