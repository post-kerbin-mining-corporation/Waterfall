using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// Transform scale modifier
  /// </summary>
  public class EffectPositionModifier : EffectModifier
  {
    public FloatCurve xCurve;
    public FloatCurve yCurve;
    public FloatCurve zCurve;

    Vector3 basePosition;
    public EffectPositionModifier()
    {
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();
      modifierTypeName = "Position";
    }
    public EffectPositionModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();
      xCurve.Load(node.GetNode("xCurve"));
      yCurve.Load(node.GetNode("yCurve"));
      zCurve.Load(node.GetNode("zCurve"));
      modifierTypeName = "Position";
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

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
    public List<Vector3> Get(List<float> strengthList)
    {
      List<Vector3> vectorList = new List<Vector3>();

      if (strengthList.Count > 1)
      {
        for (int i = 0; i < xforms.Count; i++)
        {
          vectorList.Add(new Vector3(xCurve.Evaluate(strengthList[i]) + randomValue, 
            yCurve.Evaluate(strengthList[i]) + randomValue, 
            zCurve.Evaluate(strengthList[i]) + randomValue)

            );
        }
      }
      else
      {
        for (int i = 0; i < xforms.Count; i++)
        {
          vectorList.Add(new Vector3(xCurve.Evaluate(strengthList[0]) + randomValue,
            yCurve.Evaluate(strengthList[0]) + randomValue,
            zCurve.Evaluate(strengthList[0]) + randomValue)

            );
        }
      }
      return vectorList;
    }
    
  }

}
