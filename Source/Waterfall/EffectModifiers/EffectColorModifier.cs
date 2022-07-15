using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectColorModifier : EffectModifier
  {
    public Gradient colorCurve;
    [Persistent] public string colorName;

    public FloatCurve rCurve = new();
    public FloatCurve gCurve = new();
    public FloatCurve bCurve = new();
    public FloatCurve aCurve = new();

    private Material[] m;
    public override bool ValidForIntegrator => !string.IsNullOrEmpty(colorName);

    public EffectColorModifier() : base()
    {
      modifierTypeName = "Material Color";
    }

    public EffectColorModifier(ConfigNode node) : base(node) { }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      rCurve.Load(node.GetNode("rCurve"));
      gCurve.Load(node.GetNode("gCurve"));
      bCurve.Load(node.GetNode("bCurve"));
      aCurve.Load(node.GetNode("aCurve"));
    }

    public override ConfigNode Save()
    {
      var node = base.Save();

      node.name = WaterfallConstants.ColorModifierNodeName;
      node.AddNode(Utils.SerializeFloatCurve("rCurve", rCurve));
      node.AddNode(Utils.SerializeFloatCurve("gCurve", gCurve));
      node.AddNode(Utils.SerializeFloatCurve("bCurve", bCurve));
      node.AddNode(Utils.SerializeFloatCurve("aCurve", aCurve));
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


    public List<Color> Get(List<float> input, List<Color> output)
    {
      output.Clear();
      if (input.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
        {
          output.Add(new(rCurve.Evaluate(input[i]) + randomValue,
                         gCurve.Evaluate(input[i]) + randomValue,
                         bCurve.Evaluate(input[i]) + randomValue,
                         aCurve.Evaluate(input[i]) + randomValue));
        }
      }
      else if (input.Count == 1)
      {
        float rVal = rCurve.Evaluate(input[0]);
        float gVal = gCurve.Evaluate(input[0]);
        float bVal = bCurve.Evaluate(input[0]);
        float aVal = aCurve.Evaluate(input[0]);
        for (int i = 0; i < m.Length; i++)
        {
          output.Add(new(rVal + randomValue,gVal + randomValue, bVal + randomValue, aVal + randomValue));
        }
      }

      return output;
    }

    public Material GetMaterial() => m[0];

    public void ApplyMaterialName(string newColorName)
    {
      colorName = newColorName;
      parentEffect.ModifierParameterChange(this);
    }

    public override bool IntegratorSuitable(EffectIntegrator integrator) => integrator is EffectColorIntegrator i && i.colorName == colorName && integrator.transformName == transformName;

    public override EffectIntegrator CreateIntegrator() => new EffectColorIntegrator(parentEffect, this);
  }
}
