using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Waterfall
{
  public class EffectColorIntegrator : EffectIntegrator_Color
  {
    private int colorPropertyID;
    private string _colorName;
    public string colorName
    {
      get { return _colorName; }
      set
      {
        _colorName = value;
        colorPropertyID = Shader.PropertyToID(_colorName);
      }
    }

    private readonly Material[]  m;

    public EffectColorIntegrator(WaterfallEffect effect, EffectColorModifier colorMod) : base(effect, colorMod)
    {
      // color specific
      colorName        = colorMod.colorName;

      m                  = new Material[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
        if (m[i].HasProperty(colorPropertyID))
        {
          initialValues[i] = m[i].GetColor(colorPropertyID);
        }
        else
        {
          Utils.LogError($"Material {m[i].name} does not have color property {colorName} for modifier {colorMod.fxName} in module {effect.parentModule.moduleID}");
        }
      }
    }

    protected override void Apply()
    {
      for (int i = 0; i < m.Length; i++)
        m[i].SetColor(colorPropertyID, workingValues[i]);
    }
  }
}
