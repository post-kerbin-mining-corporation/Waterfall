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
    public string floatName;

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
    protected override void ApplyReplace(float strength)
    {
      float toSet = curve.Evaluate(strength) + randomValue;
      for (int i = 0; i < m.Length; i++)
      {
        m[i].SetFloat(floatName, toSet);
      }
    }
    protected override void ApplyAdd(float strength)
    {
      for (int i = 0; i < m.Length; i++)
      {
        float original = m[i].GetFloat(floatName);
        float toSet = curve.Evaluate(strength) + randomValue;
        m[i].SetFloat(floatName, original - toSet);
      }
    }
    protected override void ApplySubtract(float strength)
    {
    
      for (int i = 0; i < m.Length; i++)
      {
        float original = m[i].GetFloat(floatName);
        float toSet = curve.Evaluate(strength) + randomValue;
        m[i].SetFloat(floatName, original - toSet);
      }
    }
    protected override void ApplyMultiply(float strength)
    {
      for (int i = 0; i < m.Length; i++)
      {
        float original = m[i].GetFloat(floatName);
        float toSet = curve.Evaluate(strength) + randomValue;
        m[i].SetFloat(floatName, original * toSet);
      }
    }

    public void ApplyFloatName(string newFloatName)
    {
      floatName = newFloatName;
    }
  }

}
