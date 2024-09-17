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
  public class EffectParticleFloatModifier : EffectModifier_Float
  {
    protected override string ConfigNodeName => WaterfallConstants.ParticleFloatModifierNodeName;
    public string paramName = "";

    private WaterfallParticleSystem[] p;

    public override bool ValidForIntegrator => !string.IsNullOrEmpty(paramName);

    public EffectParticleFloatModifier() : base()
    {
      modifierTypeName = "Particle System Float";
    }

    public EffectParticleFloatModifier(ConfigNode node) : base(node) { }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      p = new WaterfallParticleSystem[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        p[i] = xforms[i].GetComponent<WaterfallParticleSystem>();
      }
    }
    public WaterfallParticleSystem GetEmitter()
    {
      return p[0];
    }
    public void ApplyParameterName(string newParamName)
    {
      paramName = newParamName;
      parentEffect.ModifierParameterChange(this);
    }
    
    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleFloatIntegrator i && i.paramName == paramName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectParticleFloatIntegrator(parentEffect, this);

    
  }

}
