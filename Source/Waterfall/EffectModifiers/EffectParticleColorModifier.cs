using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall.EffectModifiers
{
  /// <summary>
  /// Particle system modifier
  /// </summary>
  public class EffectParticleColorModifier : EffectModifier_Color
  {
    protected override string ConfigNodeName => WaterfallConstants.ParticleColorModifierNodeName;
    [Persistent] public string colorName;

    private WaterfallParticleSystem[] p;
    
    public EffectParticleColorModifier() : base()
    {
      modifierTypeName = "Particle System Color";
    }

    public EffectParticleColorModifier(ConfigNode node) : base(node) { }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      p = new WaterfallParticleSystem[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        p[i] = xforms[i].GetComponent<WaterfallParticleSystem>();
      }
    }

    public WaterfallParticleSystem GetEmitter() => p[0];

    public void ApplyParameterName(string newColorName)
    {
      colorName = newColorName;
      parentEffect.ModifierParameterChange(this);
    }
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(colorName);
    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleColorIntegrator i && i.colorName == colorName && integrator.transformName == transformName;
    
    public override EffectIntegrator CreateIntegrator() => new EffectParticleColorIntegrator(parentEffect, this);


  }
}
