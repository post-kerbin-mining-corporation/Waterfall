using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectColorIntegrator : EffectIntegrator
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
    protected readonly Color[] modifierData;
    protected readonly Color[] initialValues;
    protected readonly Color[] workingValues;

    private readonly Material[]  m;

    public EffectColorIntegrator(WaterfallEffect effect, EffectColorModifier colorMod) : base(effect, colorMod)
    {
      // color specific
      colorName        = colorMod.colorName;

      m                  = new Material[xforms.Count];
      modifierData = new Color[xforms.Count];
      initialValues = new Color[xforms.Count];
      workingValues = new Color[xforms.Count];
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

    public override void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      
      Array.Copy(initialValues, workingValues, m.Length);

      for (int i = 0; i < handledModifiers.Count; i++)
      {
        var mod = handledModifiers[i];
        if (mod.Controller != null)
        {
          float[] controllerData = mod.Controller.Get();
          ((EffectColorModifier) mod).Get(controllerData, modifierData);
          Integrate(mod.effectMode, workingValues, modifierData);
        }
      }

      for (int i = 0; i < m.Length; i++)
        m[i].SetColor(colorPropertyID, workingValues[i]);
    }
  }
}
