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
  public class EffectLightColorModifier : EffectModifier
  {
    public string colorName;
    public string lightTransformName;
    public float colorBlend;
    public Light[] lights;

    Material[] m;

    public EffectLightColorModifier()
    {


      modifierTypeName = "Material Light Color";
    }
    public EffectLightColorModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);

      node.TryGetValue("colorName", ref colorName);
      node.TryGetValue("lightTransformName", ref lightTransformName);
      node.TryGetValue("colorBlend", ref colorBlend);

      modifierTypeName = "Material Light Color";
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.LightColorNodeName;
      node.AddValue("colorName", colorName);
      node.AddValue("lightTransformName", lightTransformName);
      node.AddValue("colorBlend", colorBlend);
      return node;
    }
    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      m = new Material[xforms.Count];
      lights = new Light[xforms.Count];
      lights = parentEffect.parentModule.GetComponentsInChildren<Light>().ToList().FindAll(x => x.transform.name == parentEffect.parentName).ToArray();
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
      }
    }
    protected override void ApplyReplace(List<float> strengthList)
    {


      for (int i = 0; i < m.Length; i++)
      {
        if (lights != null && lights.Length > i)
          m[i].SetColor(colorName, lights[i].color * colorBlend + Color.white * (1f - colorBlend));
        else if (lights != null && lights.Length > 0)
          m[i].SetColor(colorName, lights[0].color * colorBlend + Color.white * (1f - colorBlend));
      }

    }
    public Material GetMaterial()
    {
      return m[0];
    }
    public void ApplyColorName(string newColorName)
    {
      colorName = newColorName;
    }

    public void ApplyLightName(string newLightName)
    {
      lightTransformName = newLightName;
      lights = new Light[xforms.Count];
      lights = parentEffect.parentModule.GetComponentsInChildren<Light>().ToList().FindAll(x => x.transform.name == lightTransformName).ToArray();
    }
  }

}
