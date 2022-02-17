using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Transform rotation modifier
  /// </summary>
  public class EffectRotationModifier : EffectModifier
  {
    public FloatCurve xCurve = new();
    public FloatCurve yCurve = new();
    public FloatCurve zCurve = new();

    private Vector3 baseRotation;

    public EffectRotationModifier() : base()
    {
      modifierTypeName = "Rotation";
    }

    public EffectRotationModifier(ConfigNode node) : this()
    {
      Load(node);
    }

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

      node.name = WaterfallConstants.RotationModifierNodeName;

      node.AddNode(Utils.SerializeFloatCurve("xCurve", xCurve));
      node.AddNode(Utils.SerializeFloatCurve("yCurve", yCurve));
      node.AddNode(Utils.SerializeFloatCurve("zCurve", zCurve));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      baseRotation = xforms[0].localEulerAngles;
    }

    public List<Vector3> Get(List<float> strengthList)
    {
      var vectorList = new List<Vector3>();

      if (strengthList.Count > 1)
      {
        for (int i = 0; i < xforms.Count; i++)
        {
          vectorList.Add(new(xCurve.Evaluate(strengthList[i]) + randomValue,
                             yCurve.Evaluate(strengthList[i]) + randomValue,
                             zCurve.Evaluate(strengthList[i]) + randomValue));
        }
      }
      else
      {
        for (int i = 0; i < xforms.Count; i++)
        {
          vectorList.Add(new(xCurve.Evaluate(strengthList[0]) + randomValue,
                             yCurve.Evaluate(strengthList[0]) + randomValue,
                             zCurve.Evaluate(strengthList[0]) + randomValue));
        }
      }

      return vectorList;
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectRotationIntegrator && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectRotationIntegrator(parentEffect, this);

  }
}