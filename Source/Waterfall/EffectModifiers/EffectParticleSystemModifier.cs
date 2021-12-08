using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{

  /// <summary>
  /// Particle system modifier
  /// </summary>
  public class EffectParticleSystemModifier : EffectModifier
  {
    public string paramName = "";
    public FloatCurve curve1;
    public FloatCurve curve2;

    WaterfallParticleEmitter[] p;

    public EffectParticleSystemModifier()
    {
      curve1 = new FloatCurve();
      curve2 = new FloatCurve();

      modifierTypeName = "Particle System";
    }
    public EffectParticleSystemModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("paramName", ref paramName);
      curve1 = new FloatCurve();
      curve1.Load(node.GetNode("curve1"));
      curve2 = new FloatCurve();
      curve2.Load(node.GetNode("curve2"));

      modifierTypeName = "Particle System";
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.ParticleSystemModifierNodeName;
      node.AddValue("paramName", paramName);

      node.AddNode(Utils.SerializeFloatCurve("curve1", curve1));
      node.AddNode(Utils.SerializeFloatCurve("curve2", curve2));
      return node;
    }
    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      p = new WaterfallParticleEmitter[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        p[i] = xforms[i].GetComponent<WaterfallParticleEmitter>();
      }

    }
    public List<Vector2> Get(List<float> strengthList)
    {
      List<Vector2> floatList = new List<Vector2>();
      if (strengthList.Count > 1)
      {
        for (int i = 0; i < p.Length; i++)
        {
          floatList.Add(new Vector2(
            curve1.Evaluate(strengthList[i]) + randomValue, curve2.Evaluate(strengthList[i]) + randomValue));
        }
      }
      else
      {
        for (int i = 0; i < p.Length; i++)
        {
          floatList.Add(new Vector2(
            curve1.Evaluate(strengthList[0]) + randomValue, curve2.Evaluate(strengthList[i]) + randomValue));
        }
      }
      return floatList;
    }

    public WaterfallParticleEmitter GetEmitter()
    {
      return p[0];
    }
    public void ApplyParameterName(string newParamName)
    {
      paramName = newParamName;
      parentEffect.ModifierParameterChange(this);
    }
  }

}
