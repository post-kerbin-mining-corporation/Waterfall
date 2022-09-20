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
  public class EffectParticleColorModifier : EffectModifier
  {
    public string paramName = "";
    public FloatCurve rCurve = new();
    public FloatCurve gCurve = new();
    public FloatCurve bCurve = new();
    public FloatCurve aCurve = new();

    private WaterfallParticleEmitter[] p;

    public EffectParticleColorModifier() : base()
    {
      modifierTypeName = "Particle System Color";
    }

    public EffectParticleColorModifier(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("paramName", ref paramName);
      rCurve.Load(node.GetNode("rCurve"));
      gCurve.Load(node.GetNode("gCurve"));
      bCurve.Load(node.GetNode("bCurve"));
      aCurve.Load(node.GetNode("aCurve"));

    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.ParticleColorModifierNodeName;
      node.AddValue("paramName", paramName);

      node.AddNode(Utils.SerializeFloatCurve("rCurve", rCurve));
      node.AddNode(Utils.SerializeFloatCurve("gCurve", gCurve));
      node.AddNode(Utils.SerializeFloatCurve("bCurve", bCurve));
      node.AddNode(Utils.SerializeFloatCurve("aCurve", aCurve));

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

    public void Get(float[] input, Color[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < p.Length; i++)
        {
          float inValue = input[i];
          output[i] = new(rCurve.Evaluate(inValue) + randomValue,
                          gCurve.Evaluate(inValue) + randomValue,
                          bCurve.Evaluate(inValue) + randomValue,
                          aCurve.Evaluate(inValue) + randomValue
                          );
        }
      }
      else if (input.Length == 1)
      {
        float inValue = input[0];
        Color vec = new Color(
          rCurve.Evaluate(inValue) + randomValue,
                          gCurve.Evaluate(inValue) + randomValue,
                          bCurve.Evaluate(inValue) + randomValue,
                          aCurve.Evaluate(inValue) + randomValue);
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


    public override EffectIntegrator CreateIntegrator() => new EffectParticleColorIntegrator(parentEffect, this);


  }
}
