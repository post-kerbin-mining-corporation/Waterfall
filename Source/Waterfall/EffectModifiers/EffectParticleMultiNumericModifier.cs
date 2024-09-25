using UnityEngine;

namespace Waterfall
{

  public class EffectParticleMultiNumericModifier : EffectModifier
  {
    public FloatCurve curve1 = new();
    public FloatCurve curve2 = new();

    public ParticleSystemCurveMode curveMode;

    private float _c1 = 0f;
    private float _c2 = 0f;
    private FloatCurve _curve1 = new();
    private FloatCurve _curve2 = new();

    protected override string ConfigNodeName => WaterfallConstants.ParticleNumericModifierNodeName;
    public string paramName = "";

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
      Debug.Log($"Init base");
      base.Init(parentEffect);
      Debug.Log($"Init blew up");
      sys = new ParticleSystem[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        sys[i] = xforms[i].GetComponent<ParticleSystem>();
        curveMode = ParticleUtils.GetParticleSystemMode(paramName, sys[i]);
      }
      Debug.Log($"Init donezo");
    }
    public void ApplyParameterName(string newName)
    {
      paramName = newName;
      if (sys != null)
      {
        // curve mode might change
        curveMode = ParticleUtils.GetParticleSystemMode(paramName, sys[0]);
      }
      Debug.Log($"Parameter name change to {newName}");
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
      }
      if (curveMode == ParticleSystemCurveMode.TwoCurves)
      {
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
      }
      if (curveMode == ParticleSystemCurveMode.TwoCurves)
      {
      }
      return node;
    }

    public void Get(float[] input, MultiNumericData[] output)
    {
      Debug.Log($"Mod Get start");
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
              break;
            case ParticleSystemCurveMode.TwoCurves:
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
            break;
          case ParticleSystemCurveMode.TwoCurves:
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
              break;
            case ParticleSystemCurveMode.TwoCurves:
              break;
          }
        }
      }
      Debug.Log($"Mod Get end");
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleMultiNumericIntegrator i && i.paramName == paramName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectParticleMultiNumericIntegrator(parentEffect, this);
  }
}
