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
  public class EffectColorModifier : EffectModifier
  {
    public Gradient colorCurve;
    public string colorName;

    public FloatCurve rCurve;
    public FloatCurve gCurve;
    public FloatCurve bCurve;
    public FloatCurve aCurve;

    Material[] m;

    public EffectColorModifier()
    {
      rCurve = new FloatCurve();
      gCurve = new FloatCurve();
      bCurve = new FloatCurve();
      aCurve = new FloatCurve();
    }
    public EffectColorModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("colorName", ref colorName);
      rCurve = new FloatCurve();
      gCurve = new FloatCurve();
      bCurve = new FloatCurve();
      aCurve = new FloatCurve();
      rCurve.Load(node.GetNode("rCurve"));
      gCurve.Load(node.GetNode("gCurve"));
      bCurve.Load(node.GetNode("bCurve"));
      aCurve.Load(node.GetNode("aCurve"));

      modifierTypeName = this.GetType().Name;
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.ColorModifierNodeName;
      node.AddValue("colorName", colorName);
      
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
    protected override void ApplyReplace(float strength)
    {
      Color toSet = new Color(rCurve.Evaluate(strength), gCurve.Evaluate(strength), bCurve.Evaluate(strength), aCurve.Evaluate(strength));
      for (int i = 0; i < m.Length; i++)
      {
        m[i].SetColor(colorName, toSet);
      }
    }
    protected override void ApplyAdd(float strength)
    {
      
      for (int i = 0; i < m.Length; i++)
      {
        Color original = m[i].GetColor(colorName);
        Color toSet = new Color(rCurve.Evaluate(strength), gCurve.Evaluate(strength), bCurve.Evaluate(strength), aCurve.Evaluate(strength));
        m[i].SetColor(colorName, original + toSet);
      }
    }
    protected override void ApplySubtract(float strength)
    {
      for (int i = 0; i < m.Length; i++)
      {
        Color original = m[i].GetColor(colorName);
        Color toSet = new Color(rCurve.Evaluate(strength), gCurve.Evaluate(strength), bCurve.Evaluate(strength), aCurve.Evaluate(strength));
        m[i].SetColor(colorName, original - toSet);
      }
    }
    protected override void ApplyMultiply(float strength)
    {
      for (int i = 0; i < m.Length; i++)
      {
        Color original = m[i].GetColor(colorName);
        Color toSet = new Color(rCurve.Evaluate(strength), gCurve.Evaluate(strength), bCurve.Evaluate(strength), aCurve.Evaluate(strength));
        m[i].SetColor(colorName, original * toSet);
      }
    }

    public void ApplyMaterialName(string newColorName)
    {
      colorName = newColorName;
    }
  }

}
