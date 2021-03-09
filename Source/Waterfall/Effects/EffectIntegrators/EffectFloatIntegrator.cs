using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectIntegrator
  {
    public string transformName;
    protected WaterfallEffect parentEffect;
    protected List<Transform> xforms;
    
    public EffectIntegrator()
    {

    }

  }

  public class EffectFloatIntegrator: EffectIntegrator
  {

    Material[] m;
    Renderer[] r;
    public string floatName;
    List<float> initialFloatValues;
    public List<EffectFloatModifier> handledModifiers;

    bool testIntensity = false;
    
    public EffectFloatIntegrator(WaterfallEffect effect, EffectFloatModifier floatMod)
    {
      Utils.Log(String.Format("[EffectFloatIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, floatMod.fxName), LogType.Modifiers);
      xforms = new List<Transform>();
      transformName = floatMod.transformName;
      parentEffect = effect;
      List<Transform> roots = parentEffect.GetModelTransforms();
      foreach (Transform t in roots)
      {
        Transform t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectFloatIntegrator]: Unable to find transform {0} on modifier {1}", transformName, floatMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


      // float specific
      floatName = floatMod.floatName;
      handledModifiers = new List<EffectFloatModifier>();
      handledModifiers.Add(floatMod);

     
      foreach (string nm in WaterfallConstants.ShaderPropertyHideFloatNames)
      {
        if (floatName == nm)
          testIntensity = true;
      }

      initialFloatValues = new List<float>();
      m = new Material[xforms.Count];
      r = new Renderer[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        r[i] = xforms[i].GetComponent<Renderer>();
        m[i] = r[i].material;
        initialFloatValues.Add(m[i].GetFloat(floatName));
      }
    }
    public void AddModifier(EffectFloatModifier newMod)
    {
      handledModifiers.Add(newMod);
    }
    public void RemoveModifier(EffectFloatModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }
    public void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      List<float> applyValues = initialFloatValues.ToList();

      foreach (EffectFloatModifier floatMod in handledModifiers)
      {
        List<float> modResult = floatMod.Get(parentEffect.parentModule.GetControllerValue(floatMod.controllerName));

        if (floatMod.effectMode == EffectModifierMode.REPLACE)
          applyValues = modResult;

        if (floatMod.effectMode == EffectModifierMode.MULTIPLY)
          for (int i = 0; i < applyValues.Count;i++)
            applyValues[i] = applyValues[i] * modResult[i];

        if (floatMod.effectMode == EffectModifierMode.ADD)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] + modResult[i];

        if (floatMod.effectMode == EffectModifierMode.SUBTRACT)
          for (int i = 0; i < applyValues.Count; i++)
            applyValues[i] = applyValues[i] - modResult[i];

      }

      for (int i = 0; i < m.Length; i++)
      {

        if (testIntensity)
        {
          if (r[i].enabled && applyValues[i] < Settings.MinimumEffectIntensity)
          {
            r[i].enabled = false;
          }
          else if (!r[i].enabled && applyValues[i] >= Settings.MinimumEffectIntensity)
          {

            r[i].enabled = true;
            m[i].SetFloat(floatName, applyValues[i]);
          }
          else if (r[i].enabled && applyValues[i] >= Settings.MinimumEffectIntensity)
          {
            m[i].SetFloat(floatName, applyValues[i]);
          }

        }
        else
          m[i].SetFloat(floatName, applyValues[i]);
      }

    }
  }

 
  
}
