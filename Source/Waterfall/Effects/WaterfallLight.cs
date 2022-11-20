using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class WaterfallLight
  {
    public string    transformName     = "";
    public string    baseTransformName = "";
    [Persistent] public float     intensity = 1f;
    [Persistent] public float     range = 1f;
    [Persistent] public Color     color;
    [Persistent] public LightType lightType = LightType.Point;
    [Persistent] public float     angle;

    public List<Light> lights;

    public WaterfallLight()
    {
      color     = Color.white;
    }

    public WaterfallLight(ConfigNode node) : this()
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      ConfigNode.LoadObjectFromConfig(this, node);
      node.TryGetValue("transform",     ref transformName);
      node.TryGetValue("baseTransform", ref baseTransformName);

      Utils.Log($"[WaterfallLight]: Loading new light for {transformName}", LogType.Effects);
    }

    public ConfigNode Save()
    {
      var node = ConfigNode.CreateConfigFromObject(this);
      node.name = WaterfallConstants.LightNodeName;
      node.AddValue("transform", transformName);
      node.AddValue("baseTransform", baseTransformName);
      return node;
    }

    public void Initialize(Transform parentTransform)
    {
      lights = new(); // parentTransform.GetComponentsInChildren<Light>().ToList();

      if (baseTransformName != "")
      {
        var candidates = parentTransform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < candidates.Length; i++)
        {
          var t = candidates[i];
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

      for (int i = 0; i < lights.Count; i++)
      {
        var l = lights[i];
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
      for (int i = 0; i < lights.Count; i++)
      {
        var l = lights[i];
        l.range = range;
      }
    }

    public void SetAngle(float value)
    {
      angle = value;
      for (int i = 0; i < lights.Count; i++)
      {
        var l = lights[i];
        l.spotAngle = angle;
      }
    }

    public void SetIntensity(float value)
    {
      intensity = value;
      for (int i = 0; i < lights.Count; i++)
      {
        var l = lights[i];
        l.intensity = intensity;
      }
    }

    public void SetColor(Color value)
    {
      color = value;
      for (int i = 0; i < lights.Count; i++)
      {
        var l = lights[i];
        l.color = color;
      }
    }

    public void SetLightType(LightType lType)
    {
      lightType = lType;
      for (int i = 0; i < lights.Count; i++)
      {
        var l = lights[i];
        l.type = lightType;
      }
    }
  }
}
