using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{

  /// <summary>
  /// Material color modifier
  /// </summary>
  public class EffectFloatModifier : EffectModifier
  {
    public string floatName = "";

    public FloatCurve curve;

    Material[] m;

    public EffectFloatModifier()
    {
      curve = new FloatCurve();

      modifierTypeName = "Material Float";
    }
    public EffectFloatModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("floatName", ref floatName);
      curve = new FloatCurve();
      curve.Load(node.GetNode("floatCurve"));

      modifierTypeName = "Material Float";
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.FloatModifierNodeName;
      node.AddValue("floatName", floatName);

      node.AddNode(Utils.SerializeFloatCurve("floatCurve", curve));
      return node;
    }
    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      m = new Material[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
      }

    }
    public List<float> Get(List<float> strengthList)
    {
      List<float> floatList = new List<float>();

      if (strengthList.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
        {
          floatList.Add(curve.Evaluate(strengthList[i]) + randomValue);
        }
      }
      else
      {
        for (int i = 0; i < m.Length; i++)
        {
          floatList.Add(curve.Evaluate(strengthList[0]) + randomValue);
        }
      }
      return floatList;
    }

    public Material GetMaterial()
    {
      return m[0];
    }
    public void ApplyFloatName(string newFloatName)
    {
      
      floatName = newFloatName;
      parentEffect.ModifierParameterChange(this);
    }
  }

}
