using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Material color modifier
  /// </summary>
  public class EffectColorFromLightModifier : EffectModifier
  {
    [Persistent] public string colorName;
    [Persistent] public string lightTransformName;
    [Persistent] public float colorBlend;
    public Light[] lights;

    private Material[] m;
    public override bool ValidForIntegrator => false;

    public EffectColorFromLightModifier() : base()
    {
      modifierTypeName = "Material Color From Light";
    }

    public EffectColorFromLightModifier(ConfigNode node) : base(node) { }

    public override ConfigNode Save()
    {
      var node = base.Save();
      node.name = WaterfallConstants.ColorFromLightNodeName;
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      m      = new Material[xforms.Count];
      lights = parentEffect.parentModule.GetComponentsInChildren<Light>().Where(x => x.transform.name == parentEffect.parentName).ToArray();
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
      }
    }

    public Material GetMaterial() => m[0];

    public void ApplyColorName(string newColorName)
    {
      colorName = newColorName;
    }

    public void ApplyLightName(string newLightName)
    {
      lightTransformName = newLightName;
      lights             = parentEffect.parentModule.GetComponentsInChildren<Light>().Where(x => x.transform.name == lightTransformName).ToArray();
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

    public override EffectIntegrator CreateIntegrator()
    {
      Utils.LogError($"EffectUVScrollModifier.CreateIntegrator() called but this has no corresponding integrator!");
      return null;
    }
  }
}
