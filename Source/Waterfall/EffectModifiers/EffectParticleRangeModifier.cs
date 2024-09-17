using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{

  /// <summary>
  /// Particle system range modifier
  /// </summary>
  public class EffectParticleRangeModifier : EffectModifier_Vector2
  {
    protected override string ConfigNodeName => WaterfallConstants.ParticleRangeModifierNodeName;
    public string paramName = "";

    private WaterfallParticleSystem[] p;

    public EffectParticleRangeModifier() : base()
    {
      modifierTypeName = "Particle System Range";
    }

    public EffectParticleRangeModifier(ConfigNode node) : base(node) { }

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
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(paramName);
    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleRangeIntegrator i && i.paramName == paramName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectParticleRangeIntegrator(parentEffect, this);

    
  }

}
