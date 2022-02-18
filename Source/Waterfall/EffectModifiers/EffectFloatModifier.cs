using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectFloatModifier : EffectModifier
  {
    [Persistent] public string floatName = "";
    public FloatCurve curve = new();
    private Material[] m;
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(floatName);

    public EffectFloatModifier() : base()
    {
      modifierTypeName = "Material Float";
    }

    public EffectFloatModifier(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      curve.Load(node.GetNode("floatCurve"));
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.name = WaterfallConstants.FloatModifierNodeName;
      node.AddNode(Utils.SerializeFloatCurve("floatCurve", curve));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      m = new Material[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
      }
    }

    public List<float> Get(List<float> input, List<float> output)
    {
      output.Clear();
      if (input.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
          output.Add(curve.Evaluate(input[i]) + randomValue);
      }
      else
      {
        float data = curve.Evaluate(input[0]);
        for (int i = 0; i < m.Length; i++)
          output.Add(data + randomValue);
      }

      return output;
    }

    public Material GetMaterial() => m[0];

    public void ApplyFloatName(string newFloatName)
    {
      floatName = newFloatName;
      parentEffect.ModifierParameterChange(this);
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectFloatIntegrator i && i.floatName == floatName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectFloatIntegrator(parentEffect, this);

  }
}
