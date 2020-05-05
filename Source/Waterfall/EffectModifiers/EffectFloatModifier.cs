using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{

  /// <summary>
  /// Material color modifier
  /// </summary>
  public class EffectFloatModifier : EffectModifier
  {
    public string floatName;

    public FloatCurve curve;

    Material m;

    public EffectFloatModifier() { }
    public EffectFloatModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("floatName", ref floatName);
      curve = new FloatCurve();
      curve.Load(node.GetNode("floatCurve"));

      modifierTypeName = this.GetType().Name;
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.FloatModifierNodeName;
      node.AddValue("floatName", floatName);

      node.AddNode(Utils.SerializeFloatCurve("floatCurve", curve));
      return node;
    }
    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      m = xform.GetComponent<Renderer>().material;
    }
    protected override void ApplyReplace(float strength)
    {
      float toSet = curve.Evaluate(strength);
      m.SetFloat(floatName, toSet);
    }
    protected override void ApplyAdd(float strength)
    {
      float original = m.GetFloat(floatName);
      float toSet = curve.Evaluate(strength);
      m.SetFloat(floatName, original + toSet);
    }
    protected override void ApplySubtract(float strength)
    {
      float original = m.GetFloat(floatName);
      float toSet = curve.Evaluate(strength);
      m.SetFloat(floatName, original - toSet);
    }
    protected override void ApplyMultiply(float strength)
    {
      float original = m.GetFloat(floatName);
      float toSet = curve.Evaluate(strength);
      m.SetFloat(floatName, original * toSet);
    }

    public void ApplyFloatName(string newFloatName)
    {
      floatName = newFloatName;
    }
  }

}
