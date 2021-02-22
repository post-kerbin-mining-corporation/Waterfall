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
  public class EffectScaleModifier : EffectModifier
  {
    public FloatCurve xCurve;
    public FloatCurve yCurve;
    public FloatCurve zCurve;

    Vector3 baseScale;
    public EffectScaleModifier()
    {
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();
      modifierTypeName = "Scale";
    }
    public EffectScaleModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();
      xCurve.Load(node.GetNode("xCurve"));
      yCurve.Load(node.GetNode("yCurve"));
      zCurve.Load(node.GetNode("zCurve"));
      modifierTypeName = "Scale";
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

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
    protected override void ApplyReplace(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        //xforms[i].localScale = new Vector3(xCurve.Evaluate(strength)+ randomValue, yCurve.Evaluate(strength)+ randomValue, zCurve.Evaluate(strength)+ randomValue);
      }
    }
    
  }

}
