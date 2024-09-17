using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall
{





  public class OLDWaterfallParticleProperty
  {
    public string propertyName;
    public string linkedSystem;
    public WaterfallParticlePropertyType propertyType;

    public virtual void Load(ConfigNode node) 
    { 
      node.TryGetValue("systemName", ref linkedSystem);
    }

    public virtual ConfigNode Save() => null;

    public virtual void Initialize(ParticleSystem p) { }
  }

  public class OLDWaterfallParticleRangeProperty : OLDWaterfallParticleProperty
  {
    public Vector2 propertyValue;


    public OLDWaterfallParticleRangeProperty()
    {
      propertyType = WaterfallParticlePropertyType.Range;
    }

    public OLDWaterfallParticleRangeProperty(ConfigNode node)
    {
      Load(node);
      propertyType = WaterfallParticlePropertyType.Range;
    }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      node.TryGetValue("rangeName", ref propertyName);
      node.TryGetValue("value", ref propertyValue);
    }

    public override void Initialize(ParticleSystem s)
    {
      //ParticleUtils.SetParticleSystemValue(propertyName, s, propertyValue);
    }

    public override ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.RangeNodeName;
      node.AddValue("linkedSystem", linkedSystem);      
      node.AddValue("rangeName", propertyName);
      node.AddValue("value", propertyValue);

      return node;
    }
  }


  public class OLDWaterfallParticleFloatProperty : OLDWaterfallParticleProperty
  {
    public float propertyValue;

    public OLDWaterfallParticleFloatProperty()
    {
      propertyType = WaterfallParticlePropertyType.Float;
    }

    public OLDWaterfallParticleFloatProperty(ConfigNode node)
    {
      Load(node);
      propertyType = WaterfallParticlePropertyType.Float;
    }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      node.TryGetValue("floatName", ref propertyName);
      node.TryGetValue("value", ref propertyValue);
    }

    public override void Initialize(ParticleSystem s)
    {
      ParticleUtils.SetParticleSystemValue(propertyName, s, propertyValue);
    }

    public override ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.FloatNodeName;
      node.AddValue("linkedSystem", linkedSystem);
      node.AddValue("floatName", propertyName);
      node.AddValue("value", propertyValue);

      return node;
    }
  }

  public class OLDWaterfallParticleColorProperty : OLDWaterfallParticleProperty
  {
    public Color propertyValue;

    public OLDWaterfallParticleColorProperty()
    {
      propertyType = WaterfallParticlePropertyType.Color;
    }

    public OLDWaterfallParticleColorProperty(ConfigNode node)
    {
      Load(node);
      propertyType = WaterfallParticlePropertyType.Color;
    }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      node.TryGetValue("colorName", ref propertyName);
      node.TryGetValue("value", ref propertyValue);
    }

    public override void Initialize(ParticleSystem s)
    {
      //ParticleUtils.SetParticleSystemValue(propertyName, s, propertyValue);
    }

    public override ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.ColorNodeName;
      node.AddValue("colorName", propertyName);
      node.AddValue("linkedSystem", linkedSystem);
      node.AddValue("value", propertyValue);

      return node;
    }
  }
}
