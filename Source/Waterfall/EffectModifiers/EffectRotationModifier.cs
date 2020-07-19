using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// Transform rotation modifier
  /// </summary>
  public class EffectRotationModifier : EffectModifier
  {
    public FloatCurve xCurve;
    public FloatCurve yCurve;
    public FloatCurve zCurve;

    Quaternion baseRotation;
    public EffectRotationModifier()
    {
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();

      modifierTypeName = "Rotation";
    }
    public EffectRotationModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();
      xCurve.Load(node.GetNode("xCurve"));
      yCurve.Load(node.GetNode("yCurve"));
      zCurve.Load(node.GetNode("zCurve"));
      modifierTypeName = "Rotation";
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.RotationModifierNodeName;

      node.AddNode(Utils.SerializeFloatCurve("xCurve", xCurve));
      node.AddNode(Utils.SerializeFloatCurve("yCurve", yCurve));
      node.AddNode(Utils.SerializeFloatCurve("zCurve", zCurve));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      baseRotation = xforms[0].localRotation;
    }
    protected override void ApplyReplace(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localRotation = Quaternion.LookRotation(new Vector3(xCurve.Evaluate(strength)+ randomValue, yCurve.Evaluate(strength)+ randomValue, zCurve.Evaluate(strength)+ randomValue));
      }
    }
    protected override void ApplyAdd(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localRotation = Quaternion.LookRotation(new Vector3(xCurve.Evaluate(strength)+ randomValue, yCurve.Evaluate(strength)+ randomValue, zCurve.Evaluate(strength)+ randomValue));
      }
    }
    protected override void ApplySubtract(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localRotation = Quaternion.LookRotation(new Vector3(xCurve.Evaluate(strength)+ randomValue, yCurve.Evaluate(strength)+ randomValue, zCurve.Evaluate(strength)+ randomValue));
      }
    }
    protected override void ApplyMultiply(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localRotation = Quaternion.LookRotation(new Vector3(xCurve.Evaluate(strength)+ randomValue, yCurve.Evaluate(strength)+ randomValue, zCurve.Evaluate(strength)+ randomValue));
      }
    }
    
  }

}
