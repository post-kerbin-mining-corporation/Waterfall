using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectLightFloatModifier : EffectModifier
  {
    public string floatName = "";

    public FloatCurve curve;

    private Light[] l;

    public EffectLightFloatModifier()
    {
      curve = new();

      modifierTypeName = "Light Float";
    }

    public EffectLightFloatModifier(ConfigNode node)
    {
      Load(node);
    }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("floatName", ref floatName);
      curve = new();
      curve.Load(node.GetNode("floatCurve"));

      modifierTypeName = "Light Float";
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.name = WaterfallConstants.LightFloatModifierNodeName;
      node.AddValue("floatName", floatName);

      node.AddNode(Utils.SerializeFloatCurve("floatCurve", curve));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      l = new Light[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();
      }
    }

    public List<float> Get(List<float> strengthList)
    {
      var floatList = new List<float>();
      if (strengthList.Count > 1)
      {
        for (int i = 0; i < l.Length; i++)
        {
          floatList.Add(curve.Evaluate(strengthList[i]) + randomValue);
        }
      }
      else
      {
        for (int i = 0; i < l.Length; i++)
        {
          floatList.Add(curve.Evaluate(strengthList[0]) + randomValue);
        }
      }

      return floatList;
    }

    public Light GetLight() => l[0];

    public void ApplyFloatName(string newFloatName)
    {
      floatName = newFloatName;
      parentEffect.ModifierParameterChange(this);
    }
  }
}