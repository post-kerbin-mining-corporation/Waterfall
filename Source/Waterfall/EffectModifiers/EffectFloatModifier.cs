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
    protected override void ApplyReplace(List<float> strengthList)
    {

      float strength = strengthList[0];
      if (strengthList.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
        {
          strength = strengthList[i];
          float toSet = curve.Evaluate(strength) + randomValue;
          m[i].SetFloat(floatName, toSet);
        }
      }
      else
      {
        for (int i = 0; i < m.Length; i++)
        {
          float toSet = curve.Evaluate(strength) + randomValue;
          m[i].SetFloat(floatName, toSet);
        }
      }
    }
    protected override void ApplyAdd(List<float> strengthList)
    {

      float strength = strengthList[0];
      if (strengthList.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
        {
          strength = strengthList[i];
          float toSet = curve.Evaluate(strength) + randomValue;

          

          float original = m[i].GetFloat(floatName);
          
          m[i].SetFloat(floatName, original + toSet);
        }
      }
      else
      {
        for (int i = 0; i < m.Length; i++)
        {
          float original = m[i].GetFloat(floatName);
          float toSet = curve.Evaluate(strength) + randomValue;
          m[i].SetFloat(floatName, original + toSet);
        }
      }
    }
    protected override void ApplySubtract(List<float> strengthList)
    {
      float strength = strengthList[0];
      if (strengthList.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
        {
          float original = m[i].GetFloat(floatName);
          strength = strengthList[i];
          float toSet = curve.Evaluate(strength) + randomValue;
          m[i].SetFloat(floatName, original - toSet);
        }
      }
      else
      {
        for (int i = 0; i < m.Length; i++)
        {
          float original = m[i].GetFloat(floatName);
          float toSet = curve.Evaluate(strength) + randomValue;
          m[i].SetFloat(floatName, original - toSet);
        }
      }
    }
    protected override void ApplyMultiply(List<float> strengthList)
    {
      float strength = strengthList[0];
      if (strengthList.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
        {
          float original = m[i].GetFloat(floatName);
          strength = strengthList[i];
          float toSet = curve.Evaluate(strength) + randomValue;
          m[i].SetFloat(floatName, original * toSet);
        }
      }
      else
      {
        for (int i = 0; i < m.Length; i++)
        {
          float original = m[i].GetFloat(floatName);
          float toSet = curve.Evaluate(strength) + randomValue;
          m[i].SetFloat(floatName, original * toSet);
        }
      }
    }
    public Material GetMaterial()
    {
      return m[0];
    }
    public void ApplyFloatName(string newFloatName)
    {
      floatName = newFloatName;
    }
  }

}
