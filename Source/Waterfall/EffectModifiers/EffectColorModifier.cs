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

      modifierTypeName = "Material Color";
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

      modifierTypeName = "Material Color";
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


    public List<Color> Get(List<float> strengthList)
    {
      List<Color> colorList = new List<Color>();

      if (strengthList.Count > 1)
      {
        for (int i = 0; i < m.Length; i++)
        {

          colorList.Add(
            new Color(
              rCurve.Evaluate(strengthList[i]) + randomValue,
            gCurve.Evaluate(strengthList[i]) + randomValue,
            bCurve.Evaluate(strengthList[i]) + randomValue,
            aCurve.Evaluate(strengthList[i]) + randomValue));
        }
      }
      else
      {
        for (int i = 0; i < m.Length; i++)
        {
          colorList.Add(
            new Color(
              rCurve.Evaluate(strengthList[0]) + randomValue,
            gCurve.Evaluate(strengthList[0]) + randomValue,
            bCurve.Evaluate(strengthList[0]) + randomValue,
            aCurve.Evaluate(strengthList[0]) + randomValue));
        }
      }
      return colorList;
    }

    public Material GetMaterial()
    {
      return m[0];
    }
    public void ApplyMaterialName(string newColorName)
    {
      colorName = newColorName;
      parentEffect.ModifierParameterChange(this);
    }
  }

}
