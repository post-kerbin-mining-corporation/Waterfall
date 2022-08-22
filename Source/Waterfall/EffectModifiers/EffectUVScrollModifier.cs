﻿using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material UV scrolling modifier
  /// </summary>
  public class EffectUVScrollModifier : EffectModifier
  {
    protected override string ConfigNodeName => WaterfallConstants.UVScrollModifierNodeName;

    public FloatCurve scrollCurveX = new();
    public FloatCurve scrollCurveY = new();
    [Persistent] public string textureName;
    private Material[] m;
    public override bool ValidForIntegrator => false;

    public EffectUVScrollModifier() : base()
    {
      modifierTypeName = GetType().Name;
    }

    public EffectUVScrollModifier(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      scrollCurveX.Load(node.GetNode("scrollCurveX"));
      scrollCurveY.Load(node.GetNode("scrollCurveY"));
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

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

    public override void Apply(float[] strengthList)
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

    public override EffectIntegrator CreateIntegrator()
    {
      Utils.LogError($"EffectUVScrollModifier.CreateIntegrator() called but this has no corresponding integrator!");
      return null;
    }
  }
}
