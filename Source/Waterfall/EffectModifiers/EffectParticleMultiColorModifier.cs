using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectParticleMultiColorModifier : EffectModifier
  {
    [Persistent] public string paramName = "_TintColor";

    public FloatCurve c1RedCurve = new();
    public FloatCurve c2RedCurve = new();
    public FloatCurve c1GreenCurve = new();
    public FloatCurve c2GreenCurve = new();
    public FloatCurve c1BlueCurve = new();
    public FloatCurve c2BlueCurve = new();
    public FloatCurve c1AlphaCurve = new();
    public FloatCurve c2AlphaCurve = new();

    public ParticleSystemGradientMode curveMode;

    private Color _c1 = new();
    private Color _c2 = new();

    protected override string ConfigNodeName => WaterfallConstants.ParticleColorModifierNodeName;

    private ParticleSystem[] sys;

    public override bool ValidForIntegrator => !string.IsNullOrEmpty(paramName);
    public EffectParticleMultiColorModifier() : base()
    {
      modifierTypeName = "Particle System Color Parameter";
    }
    public ParticleSystem GetParticleSystem() => sys[0];
    public EffectParticleMultiColorModifier(ConfigNode node) : base(node) { }
    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      sys = new ParticleSystem[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        sys[i] = xforms[i].GetComponent<ParticleSystem>();
        curveMode = ParticleUtils.GetParticleSystemColorMode(paramName, sys[i]);
      }
    }
    public void ApplyParameterName(string newName)
    {
      paramName = newName;
      if (sys != null)
      {
        // curve mode might change
        curveMode = ParticleUtils.GetParticleSystemColorMode(paramName, sys[0]);
      }
      parentEffect.ModifierParameterChange(this);
    }
    public override void Load(ConfigNode node)
    {
      base.Load(node);

      if (curveMode == ParticleSystemGradientMode.Color)
      {
        c1RedCurve.Load(node.GetNode("rCurve1"));
        c1GreenCurve.Load(node.GetNode("gCurve1"));
        c1BlueCurve.Load(node.GetNode("bCurve1"));
        c1AlphaCurve.Load(node.GetNode("aCurve1"));

      }
      if (curveMode == ParticleSystemGradientMode.TwoColors)
      {
        c1RedCurve.Load(node.GetNode("rCurve1"));
        c1GreenCurve.Load(node.GetNode("gCurve1"));
        c1BlueCurve.Load(node.GetNode("bCurve1"));
        c1AlphaCurve.Load(node.GetNode("aCurve1"));

        c2RedCurve.Load(node.GetNode("rCurve2"));
        c2GreenCurve.Load(node.GetNode("gCurve2"));
        c2BlueCurve.Load(node.GetNode("bCurve2"));
        c2AlphaCurve.Load(node.GetNode("aCurve2"));
      }
      if (curveMode == ParticleSystemGradientMode.Gradient)
      {
        /// Not supported yet
      }
      if (curveMode == ParticleSystemGradientMode.TwoGradients)
      {
        /// Not supported yet
      }
    }
    public override ConfigNode Save()
    {
      var node = base.Save();
      if (curveMode == ParticleSystemGradientMode.Color)
      {
        node.AddNode(Utils.SerializeFloatCurve("rCurve1", c1RedCurve));
        node.AddNode(Utils.SerializeFloatCurve("gCurve1", c1GreenCurve));
        node.AddNode(Utils.SerializeFloatCurve("bCurve1", c1BlueCurve));
        node.AddNode(Utils.SerializeFloatCurve("aCurve1", c1AlphaCurve));
      }
      if (curveMode == ParticleSystemGradientMode.TwoColors)
      {
        node.AddNode(Utils.SerializeFloatCurve("rCurve1", c1RedCurve));
        node.AddNode(Utils.SerializeFloatCurve("gCurve1", c1GreenCurve));
        node.AddNode(Utils.SerializeFloatCurve("bCurve1", c1BlueCurve));
        node.AddNode(Utils.SerializeFloatCurve("aCurve1", c1AlphaCurve));

        node.AddNode(Utils.SerializeFloatCurve("rCurve2", c2RedCurve));
        node.AddNode(Utils.SerializeFloatCurve("gCurve2", c2GreenCurve));
        node.AddNode(Utils.SerializeFloatCurve("bCurve2", c2BlueCurve));
        node.AddNode(Utils.SerializeFloatCurve("aCurve2", c2AlphaCurve));
      }
      if (curveMode == ParticleSystemGradientMode.TwoGradients)
      {
        /// Not supported yet
      }
      if (curveMode == ParticleSystemGradientMode.TwoGradients)
      {
        /// Not supported yet
      }
      return node;
    }

    public void Get(float[] input, MultiColorData[] output)
    {
      if (input.Length > 1)
      {
        for (int i = 0; i < output.Length; i++)
        {
          float inValue = input[i];
          switch (output[i].mode)
          {
            case ParticleSystemGradientMode.Color:
              output[i].color1 = new(c1RedCurve.Evaluate(inValue) + randomValue,
                                     c1GreenCurve.Evaluate(inValue) + randomValue,
                                     c1BlueCurve.Evaluate(inValue) + randomValue,
                                     c1AlphaCurve.Evaluate(inValue) + randomValue);
              break;
            case ParticleSystemGradientMode.TwoColors:
              output[i].color1 = new(c1RedCurve.Evaluate(inValue) + randomValue,
                                     c1GreenCurve.Evaluate(inValue) + randomValue,
                                     c1BlueCurve.Evaluate(inValue) + randomValue,
                                     c1AlphaCurve.Evaluate(inValue) + randomValue);

              output[i].color2 = new(c2RedCurve.Evaluate(inValue) + randomValue,
                                     c2GreenCurve.Evaluate(inValue) + randomValue,
                                     c2BlueCurve.Evaluate(inValue) + randomValue,
                                     c2AlphaCurve.Evaluate(inValue) + randomValue);
              break;
            case ParticleSystemGradientMode.Gradient:
              /// Not supported yet
              break;
            case ParticleSystemGradientMode.TwoGradients:
              /// Not supported yet
              break;
          }
        }
      }
      else if (input.Length == 1)
      {
        float inValue = input[0];
        switch (output[0].mode)
        {
          case ParticleSystemGradientMode.Color:
            _c1 = new(c1RedCurve.Evaluate(inValue) + randomValue,
                                   c1GreenCurve.Evaluate(inValue) + randomValue,
                                   c1BlueCurve.Evaluate(inValue) + randomValue,
                                   c1AlphaCurve.Evaluate(inValue) + randomValue);
            break;
          case ParticleSystemGradientMode.TwoColors:
            _c1 = new(c1RedCurve.Evaluate(inValue) + randomValue,
                                   c1GreenCurve.Evaluate(inValue) + randomValue,
                                   c1BlueCurve.Evaluate(inValue) + randomValue,
                                   c1AlphaCurve.Evaluate(inValue) + randomValue);

            _c2 = new(c2RedCurve.Evaluate(inValue) + randomValue,
                                   c2GreenCurve.Evaluate(inValue) + randomValue,
                                   c2BlueCurve.Evaluate(inValue) + randomValue,
                                   c2AlphaCurve.Evaluate(inValue) + randomValue);
            break;
          case ParticleSystemGradientMode.Gradient:
            /// Not supported yet
            break;
          case ParticleSystemGradientMode.TwoGradients:
            /// Not supported yet
            break;
        }
        for (int i = 0; i < output.Length; i++)
        {
          switch (output[i].mode)
          {
            case ParticleSystemGradientMode.Color:
              output[i].color1 = _c1;
              break;
            case ParticleSystemGradientMode.TwoColors:
              output[i].color1 = _c1;
              output[i].color2 = _c2;
              break;
            case ParticleSystemGradientMode.Gradient:
              /// Not supported yet
              break;
            case ParticleSystemGradientMode.TwoGradients:
              /// Not supported yet
              break;
          }
        }
      }
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectParticleMultiColorIntegrator i && i.paramName == paramName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectParticleMultiColorIntegrator(parentEffect, this);
  }
}

