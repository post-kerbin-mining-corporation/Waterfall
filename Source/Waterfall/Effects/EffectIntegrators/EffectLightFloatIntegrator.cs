using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
 
  public class EffectLightFloatIntegrator: EffectIntegrator
  {

    Light[] l;
    public string floatName;
    List<float> initialFloatValues;
    public List<EffectLightFloatModifier> handledModifiers;

    bool testIntensity = false;
    
    public EffectLightFloatIntegrator(WaterfallEffect effect, EffectLightFloatModifier floatMod)
    {
      Utils.Log(String.Format("[EffectLightFloatIntegrator]: Initializing integrator for {0} on modifier {1}", effect.name, floatMod.fxName), LogType.Modifiers);
      xforms = new List<Transform>();
      transformName = floatMod.transformName;
      parentEffect = effect;
      List<Transform> roots = parentEffect.GetModelTransforms();
      foreach (Transform t in roots)
      {
        Transform t1 = t.FindDeepChild(transformName);
        if (t1 == null)
        {
          Utils.LogError(String.Format("[EffectLightFloatIntegrator]: Unable to find transform {0} on modifier {1}", transformName, floatMod.fxName));
        }
        else
        {
          xforms.Add(t1);
        }
      }


      // float specific
      floatName = floatMod.floatName;
      handledModifiers = new List<EffectLightFloatModifier>();
      handledModifiers.Add(floatMod);

      foreach (string nm in WaterfallConstants.ShaderPropertyHideFloatNames)
      {
        if (floatName == nm)
          testIntensity = true;
      }

      initialFloatValues = new List<float>();

      l = new Light[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();

        if (floatName == "Intensity")
        initialFloatValues.Add(l[i].intensity);
        if (floatName == "Range")
          initialFloatValues.Add(l[i].range);
        if (floatName == "SpotAngle")
          initialFloatValues.Add(l[i].spotAngle);
      }
    }
    public void AddModifier(EffectLightFloatModifier newMod)
    {
      handledModifiers.Add(newMod);
    }
    public void RemoveModifier(EffectLightFloatModifier newMod)
    {
      handledModifiers.Remove(newMod);
    }
    public void Update()
    {
      if (Settings.EnableLights)
      {
        if (handledModifiers.Count > 0)
        {
          List<float> applyValues = initialFloatValues;
          foreach (EffectLightFloatModifier floatMod in handledModifiers)
          {
            List<float> modResult = floatMod.Get(parentEffect.parentModule.GetControllerValue(floatMod.controllerName));

            if (floatMod.effectMode == EffectModifierMode.REPLACE)
              applyValues = modResult;

            if (floatMod.effectMode == EffectModifierMode.MULTIPLY)
              for (int i = 0; i < applyValues.Count; i++)
                applyValues[i] = applyValues[i] * modResult[i];

            if (floatMod.effectMode == EffectModifierMode.ADD)
              for (int i = 0; i < applyValues.Count; i++)
                applyValues[i] = applyValues[i] + modResult[i];

            if (floatMod.effectMode == EffectModifierMode.SUBTRACT)
              for (int i = 0; i < applyValues.Count; i++)
                applyValues[i] = applyValues[i] - modResult[i];

          }
          for (int i = 0; i < l.Length; i++)
          {

            if (testIntensity)
            {
              if (l[i].enabled && applyValues[i] < Settings.MinimumLightIntensity)
              {
                l[i].enabled = false;
              }
              else if (!l[i].enabled && applyValues[i] >= Settings.MinimumLightIntensity)
              {

                l[i].enabled = true;

                if (floatName == "Intensity")
                  l[i].intensity = applyValues[i];
                if (floatName == "Range")
                  l[i].range = applyValues[i];
                if (floatName == "SpotAngle")
                  l[i].spotAngle = applyValues[i];
              }
              else if (l[i].enabled && applyValues[i] >= Settings.MinimumLightIntensity)
              {
                if (floatName == "Intensity")
                  l[i].intensity = applyValues[i];
                if (floatName == "Range")
                  l[i].range = applyValues[i];
                if (floatName == "SpotAngle")
                  l[i].spotAngle = applyValues[i];
              }

            }
            else
            {
              if (floatName == "Intensity")
                l[i].intensity = applyValues[i];
              if (floatName == "Range")
                l[i].range = applyValues[i];
              if (floatName == "SpotAngle")
                l[i].spotAngle = applyValues[i];
            }
          }
        }
      }
    }
  }

 
  
}
