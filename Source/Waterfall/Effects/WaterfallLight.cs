using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class WaterfallLight
  {
    public string    transformName     = "";
    public string    baseTransformName = "";
    public float     intensity         = 1f;
    public float     range             = 1f;
    public Color     color;
    public LightType lightType;

    public float angle;

    public List<Light> lights;

    public WaterfallLight()
    {
      color     = Color.white;
      lightType = LightType.Point;
      intensity = 1f;
      range     = 1f;
    }

    public WaterfallLight(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      color     = Color.white;
      lightType = LightType.Point;
      node.TryGetValue("transform",     ref transformName);
      node.TryGetValue("baseTransform", ref baseTransformName);
      node.TryGetValue("intensity",     ref intensity);
      node.TryGetValue("range",         ref range);
      node.TryGetEnum("lightType", ref lightType, LightType.Point);
      node.TryGetValue("color", ref color);
      node.TryGetValue("angle", ref angle);

      Utils.Log(String.Format("[WaterfallLight]: Loading new light for {0} ", transformName), LogType.Effects);
    }

    public ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.LightNodeName;
      node.AddValue("transform", transformName);
      node.AddValue("intensity", intensity);
      node.AddValue("range",     range);
      node.AddValue("lightType", lightType);
      node.AddValue("color",     color);
      node.AddValue("angle",     angle);
      return node;
    }

    public void Initialize(Transform parentTransform)
    {
      lights = new(); // parentTransform.GetComponentsInChildren<Light>().ToList();

      if (baseTransformName != "")
      {
        var candidates = parentTransform.GetComponentsInChildren<Transform>();
        foreach (var t in candidates)
        {
          var l = t.GetComponent<Light>();
          if (l != null)
          {
            Utils.Log($"[WaterfallLight]: Added light material from {t.name}", LogType.Effects);
            lights.Add(l);
          }
        }
      }
      else
      {
        var materialTarget = parentTransform.FindDeepChild(transformName);
        var l              = materialTarget.GetComponent<Light>();

        l.enabled = Settings.EnableLights;
        lights.Add(l);
      }

      Utils.Log($"[WaterfallLight]: Initialized WaterfallLight at {parentTransform}, {lights.Count} Count", LogType.Effects);

      foreach (var l in lights)
      {
        l.range     = range;
        l.type      = lightType;
        l.intensity = intensity;
        l.spotAngle = angle;
        l.color     = color;
      }
    }

    public void SetRange(float value)
    {
      range = value;
      foreach (var l in lights)
      {
        l.range = range;
      }
    }

    public void SetAngle(float value)
    {
      angle = value;
      foreach (var l in lights)
      {
        l.spotAngle = angle;
      }
    }

    public void SetIntensity(float value)
    {
      intensity = value;
      foreach (var l in lights)
      {
        l.intensity = intensity;
      }
    }

    public void SetColor(Color value)
    {
      color = value;
      foreach (var l in lights)
      {
        l.color = color;
      }
    }

    public void SetLightType(LightType lType)
    {
      lightType = lType;
      foreach (var l in lights)
      {
        l.type = lightType;
      }
    }
  }
}