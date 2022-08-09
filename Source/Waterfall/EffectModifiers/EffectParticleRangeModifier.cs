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
  public class EffectParticleRangeModifier : EffectModifier
  {
    public string paramName = "";
    public FloatCurve curve1 = new();
    public FloatCurve curve2 = new();

    private WaterfallParticleEmitter[] p;

    public EffectParticleRangeModifier() : base()
    {
      modifierTypeName = "Particle System Range";
    }

    public EffectParticleRangeModifier(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("paramName", ref paramName);
      curve1.Load(node.GetNode("curve1"));
      curve2.Load(node.GetNode("curve2"));

    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.ParticleRangeModifierNodeName;
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

    public void Get(float[] input, Vector2[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < p.Length; i++)
        {
          float inValue = input[i];
          output[i] = new(curve1.Evaluate(inValue) + randomValue,
                          curve2.Evaluate(inValue) + randomValue);
        }
      }
      else if (input.Length == 1)
      {
        float inValue = input[0];
        Vector2 vec = new Vector2(
          curve1.Evaluate(inValue) + randomValue,
          curve2.Evaluate(inValue) + randomValue);
        for (int i = 0; i < p.Length; i++)
          output[i] = vec;
      }
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
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(paramName);
    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleRangeIntegrator i && i.paramName == paramName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectParticleRangeIntegrator(parentEffect, this);

    
  }

}
