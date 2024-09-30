using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectParticleMultiNumericModifier : EffectModifier
  {
    [Persistent] public string paramName = "_TintColor";

    public FastFloatCurve curve1 = new();
    public FastFloatCurve curve2 = new();

    public ParticleSystemCurveMode curveMode;

    private float _c1 = 0f;
    private float _c2 = 0f;

    protected override string ConfigNodeName => WaterfallConstants.ParticleNumericModifierNodeName;

    private ParticleSystem[] sys;

    public override bool ValidForIntegrator => !string.IsNullOrEmpty(paramName);
    public EffectParticleMultiNumericModifier() : base()
    {
      modifierTypeName = "Particle System Numeric Parameter";
    }
    public ParticleSystem GetParticleSystem() => sys[0];
    public EffectParticleMultiNumericModifier(ConfigNode node) : base(node) { }
    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      sys = new ParticleSystem[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        sys[i] = xforms[i].GetComponent<ParticleSystem>();
        curveMode = ParticleUtils.GetParticleSystemMode(paramName, sys[i]);
      }
    }
    public void ApplyParameterName(string newName)
    {
      paramName = newName;
      if (sys != null)
      {
        // curve mode might change
        curveMode = ParticleUtils.GetParticleSystemMode(paramName, sys[0]);
      }
      parentEffect.ModifierParameterChange(this);
    }
    public override void Load(ConfigNode node)
    {
      base.Load(node);

      if (curveMode == ParticleSystemCurveMode.Constant)
      {
        curve1.Load(node.GetNode("curve1"));

      }
      if (curveMode == ParticleSystemCurveMode.TwoConstants)
      {
        curve1.Load(node.GetNode("curve1"));
        curve2.Load(node.GetNode("curve2"));
      }
      if (curveMode == ParticleSystemCurveMode.Curve)
      {
        /// Not supported yet
      }
      if (curveMode == ParticleSystemCurveMode.TwoCurves)
      {
        /// Not supported yet
      }
    }
    public override ConfigNode Save()
    {
      var node = base.Save();
      if (curveMode == ParticleSystemCurveMode.Constant)
      {
        node.AddNode(Utils.SerializeFloatCurve("curve1", curve1));

      }
      if (curveMode == ParticleSystemCurveMode.TwoConstants)
      {
        node.AddNode(Utils.SerializeFloatCurve("curve1", curve1));
        node.AddNode(Utils.SerializeFloatCurve("curve2", curve2));
      }
      if (curveMode == ParticleSystemCurveMode.Curve)
      {
        /// Not supported yet
      }
      if (curveMode == ParticleSystemCurveMode.TwoCurves)
      {
        /// Not supported yet
      }
      return node;
    }

    public void Get(float[] input, MultiNumericData[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < output.Length; i++)
        {
          switch (output[i].mode)
          {
            case ParticleSystemCurveMode.Constant:
              output[i].const1 = curve1.Evaluate(input[i]) + randomValue;
              break;
            case ParticleSystemCurveMode.TwoConstants:
              output[i].const1 = curve1.Evaluate(input[i]) + randomValue;
              output[i].const2 = curve2.Evaluate(input[i]) + randomValue;
              break;
            case ParticleSystemCurveMode.Curve:
              /// Not supported yet
              break;
            case ParticleSystemCurveMode.TwoCurves:
              /// Not supported yet
              break;
          }
        }
      }
      else if (input.Length == 1)
      {
        switch (output[0].mode)
        {
          case ParticleSystemCurveMode.Constant:
            _c1 = curve1.Evaluate(input[0]) + randomValue;
            break;
          case ParticleSystemCurveMode.TwoConstants:
            _c1 = curve1.Evaluate(input[0]) + randomValue;
            _c2 = curve2.Evaluate(input[0]) + randomValue;
            break;
          case ParticleSystemCurveMode.Curve:
            /// Not supported yet
            break;
          case ParticleSystemCurveMode.TwoCurves:
            /// Not supported yet
            break;
        }
        for (int i = 0; i < output.Length; i++)
        {
          switch (output[i].mode)
          {
            case ParticleSystemCurveMode.Constant:
              output[i].const1 = _c1;
              break;
            case ParticleSystemCurveMode.TwoConstants:
              output[i].const1 = _c1;
              output[i].const2 = _c2;
              break;
            case ParticleSystemCurveMode.Curve:
              /// Not supported yet
              break;
            case ParticleSystemCurveMode.TwoCurves:
              /// Not supported yet
              break;
          }
        }
      }
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleMultiNumericIntegrator i && i.paramName == paramName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectParticleMultiNumericIntegrator(parentEffect, this);
  }
}
