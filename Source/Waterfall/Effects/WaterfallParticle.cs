using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// This manages a SET OF particle systems
  /// </summary>
  public class WaterfallParticle
  {

    public string transformName = "";
    public string baseTransformName = "";
    public string assetName = "";

    public List<WaterfallParticleProperty> pProperties;

    public Dictionary<string, WaterfallParticleSystem> systems;

    public WaterfallParticle()
    {
      pProperties = new();
    }

    public WaterfallParticle(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue("transform", ref transformName);
      node.TryGetValue("baseTransform", ref baseTransformName);
      node.TryGetValue("assetName", ref assetName);

      Utils.Log(String.Format($"[WaterfallParticle]: Loading new particle {assetName} for {transformName}"), LogType.Particles);

      pProperties = new();
      // TODO: Properties saving
      //foreach (var subnode in node.GetNodes(WaterfallConstants.RangeNodeName))
      //{
      //  pProperties.Add(new WaterfallParticleRangeProperty(subnode));
      //}
      //foreach (var subnode in node.GetNodes(WaterfallConstants.FloatNodeName))
      //{
      //  pProperties.Add(new WaterfallParticleFloatProperty(subnode));
      //}
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.ParticleNodeName;
      node.AddValue("transform", transformName);
      node.AddValue("assetName", assetName);

      // TODO: Properties saving
      //foreach (var p in pProperties)
      //{
      //  node.AddNode(p.Save());
      //}

      return node;
    }

    public void Initialize(Transform parentTransform)
    {
      systems = new Dictionary<string, WaterfallParticleSystem>();


      GameObject go = GameObject.Instantiate(WaterfallParticleLoader.GetParticles(assetName),
            Vector3.zero, Quaternion.identity) as GameObject;
      go.transform.SetParent(parentTransform);
      go.transform.localPosition = Vector3.zero;
      go.transform.localScale = Vector3.one;
      go.transform.localRotation = Quaternion.identity;

      ParticleSystem[] systemsInAsset = go.GetComponentsInChildren<ParticleSystem>();
      foreach (ParticleSystem system in systemsInAsset)
      {
        WaterfallParticleSystem wfSystem = new WaterfallParticleSystem(system);
        systems.Add(system.name, wfSystem);
      }

      /// TODO: Properties apply on load
      //foreach (var p in pProperties)
      //{
      //  foreach(var sys in systems)
      //  {
      //    p.Initialize(sys.emitter);
      //  }
        
      //}

      Utils.Log($"[WaterfallParticle]: Initialized Waterfall Particle at {parentTransform}, {systems.Count} Count", LogType.Particles);
    }

    public void SetParticleValue(string propertyName, string systemName, Vector2 value)
    {
      //var prop = pProperties.Find(x => x.propertyName == propertyName);
      //if (prop is WaterfallParticleRangeProperty t && prop != null)
      //{
      //  t.propertyValue = value;
      //}
      //else
      //{
      //  var newProp = new WaterfallParticleRangeProperty
      //  {
      //    propertyName = propertyName,
      //    propertyValue = value
      //  };
      //  pProperties.Add(newProp);
      //}

      if (systems.ContainsKey(systemName))
      {
        ParticleUtils.SetParticleSystemValue(propertyName, systems[systemName].emitter, value);
      }
    }
    public void SetParticleValue(string propertyName, string systemName, float value)
    {

      //var prop = pProperties.Find(x => x.propertyName == propertyName);
      //if (prop is WaterfallParticleFloatProperty t && prop != null)
      //  t.propertyValue = value;
      //else
      //{
      //  var newProp = new WaterfallParticleFloatProperty
      //  {
      //    propertyName = propertyName,
      //    propertyValue = value
      //  };
      //  pProperties.Add(newProp);
      //}
      if (systems.ContainsKey(systemName))
      {
        ParticleUtils.SetParticleSystemValue(propertyName, systems[systemName].emitter, value);
      }
    }

    public void SetParticleValue(string propertyName, string systemName, Color value)
    {
      //var prop = pProperties.Find(x => x.propertyName == propertyName);
      //if (prop is WaterfallParticleColorProperty t && prop != null)
      //{
      //  t.propertyValue = value;
      //}
      //else
      //{
      //  var newProp = new WaterfallParticleColorProperty
      //  {
      //    propertyName = propertyName,
      //    propertyValue = value
      //  };
      //  pProperties.Add(newProp);
      //}
      if (systems.ContainsKey(systemName))
      {
        ParticleUtils.SetParticleSystemValue(propertyName, systems[systemName].emitter, value);
      }
    }

  }

}
