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
          float inValue = input[i];
          output[i] = new(rCurve.Evaluate(inValue) + randomValue,
                          gCurve.Evaluate(inValue) + randomValue,
                          bCurve.Evaluate(inValue) + randomValue,
                          aCurve.Evaluate(inValue) + randomValue);
        }
      }
      else if (input.Count == 1)
      {
        float inValue = input[0];
        Color color = new Color(
          rCurve.Evaluate(inValue) + randomValue,
          gCurve.Evaluate(inValue) + randomValue,
          bCurve.Evaluate(inValue) + randomValue,
          aCurve.Evaluate(inValue) + randomValue);
        for (int i = 0; i < m.Length; i++)
        {
          output.Add(color);
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
