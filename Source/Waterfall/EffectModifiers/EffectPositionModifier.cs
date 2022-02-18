using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Transform scale modifier
  /// </summary>
  public class EffectPositionModifier : EffectModifier
  {
    public FloatCurve xCurve = new();
    public FloatCurve yCurve = new();
    public FloatCurve zCurve = new();
    private Vector3 basePosition;

    public EffectPositionModifier() : base()
    {
      modifierTypeName = "Position";
    }

    public EffectPositionModifier(ConfigNode node) : base(node) { }

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

      node.name = WaterfallConstants.PositionModifierNodeName;

      node.AddNode(Utils.SerializeFloatCurve("xCurve", xCurve));
      node.AddNode(Utils.SerializeFloatCurve("yCurve", yCurve));
      node.AddNode(Utils.SerializeFloatCurve("zCurve", zCurve));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      basePosition = xforms[0].localPosition;
    }

    public List<Vector3> Get(List<float> input, List<Vector3> output) => Get(input, output, xCurve, yCurve, zCurve);

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectPositionIntegrator && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectPositionIntegrator(parentEffect, this);

  }
}
