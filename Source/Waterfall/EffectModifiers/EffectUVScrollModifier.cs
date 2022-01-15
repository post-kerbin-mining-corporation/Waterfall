using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material UV scrolling modifier
  /// </summary>
  public class EffectUVScrollModifier : EffectModifier
  {
    public  FloatCurve scrollCurveX;
    public  FloatCurve scrollCurveY;
    public  string     textureName;
    private Material[] m;

    public EffectUVScrollModifier()
    {
      scrollCurveX = new();
      scrollCurveY = new();
    }

    public EffectUVScrollModifier(ConfigNode node)
    {
      Load(node);
    }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("textureName", ref textureName);
      scrollCurveX = new();
      scrollCurveY = new();
      scrollCurveX.Load(node.GetNode("scrollCurveX"));
      scrollCurveY.Load(node.GetNode("scrollCurveY"));
      modifierTypeName = GetType().Name;
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.name = WaterfallConstants.UVScrollModifierNodeName;
      node.AddValue("textureName", textureName);

      node.AddNode(Utils.SerializeFloatCurve("scrollCurveX", scrollCurveX));
      node.AddNode(Utils.SerializeFloatCurve("scrollCurveY", scrollCurveY));
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

    public override void Apply(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < m.Length; i++)
      {
        var   original = m[i].GetTextureOffset(textureName);
        float x        = original.x + scrollCurveX.Evaluate(strength) * Time.deltaTime;
        if (x >= 1f || x <= -1f)
          x = 0f;
        float y = original.y + scrollCurveY.Evaluate(strength) * Time.deltaTime;
        if (y >= 1f || y <= -1f)
          y = 0f;
        m[i].SetTextureOffset(textureName, new(x, y));
      }
    }
  }
}