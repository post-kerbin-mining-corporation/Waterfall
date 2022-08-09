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
  public class EffectParticleFloatModifier : EffectModifier
  {
    public string paramName = "";
    public FloatCurve curve = new();

    private WaterfallParticleEmitter[] p;

    public EffectParticleFloatModifier() : base()
    {
      modifierTypeName = "Particle System Float";
    }

    public EffectParticleFloatModifier(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("paramName", ref paramName);
      curve.Load(node.GetNode("curve1"));

    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.ParticleFloatModifierNodeName;
      node.AddValue("paramName", paramName);

      node.AddNode(Utils.SerializeFloatCurve("curve1", curve));
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

    public void Get(float[] input, float[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < p.Length; i++)
        {
          float inValue = input[i];
          output[i] = curve.Evaluate(inValue) + randomValue;
        }
      }
      else if (input.Length == 1)
      {
        float inValue = input[0];
        float f = curve.Evaluate(inValue) + randomValue;
        for (int i = 0; i < p.Length; i++)
          output[i] = f;
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
    
    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleFloatIntegrator i && i.paramName == paramName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectParticleFloatIntegrator(parentEffect, this);

    
  }

}
