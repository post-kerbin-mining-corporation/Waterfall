using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// Material UV scrolling modifier
  /// </summary>
  public class EffectUVScrollModifier : EffectModifier
  {
    public FloatCurve scrollCurveX;
    public FloatCurve scrollCurveY;
    public string textureName;
    Material m;

    public EffectUVScrollModifier() { }
    public EffectUVScrollModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("textureName", ref textureName);
      scrollCurveX = new FloatCurve();
      scrollCurveY = new FloatCurve();
      scrollCurveX.Load(node.GetNode("scrollCurveX"));
      scrollCurveY.Load(node.GetNode("scrollCurveY"));
      modifierTypeName = this.GetType().Name;
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();
      
      node.name = WaterfallConstants.UVScrollModifierNodeName;
      node.AddValue("textureName", textureName);

      node.AddNode(Utils.SerializeFloatCurve("scrollCurveX", scrollCurveX));
      node.AddNode(Utils.SerializeFloatCurve("scrollCurveY", scrollCurveY));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      m = xform.GetComponent<Renderer>().material;
    }
    public override void Apply(float strength)
    {
      Vector2 original = m.GetTextureOffset(textureName);
      float x = original.x + scrollCurveX.Evaluate(strength) * Time.deltaTime;
      if (x >= 1f || x <= -1f)
        x = 0f;
      float y = original.y + scrollCurveY.Evaluate(strength) * Time.deltaTime;
      if (y >= 1f || y <= -1f)
        y = 0f;
      m.SetTextureOffset(textureName, new Vector2(x, y));
    }
  }


}
