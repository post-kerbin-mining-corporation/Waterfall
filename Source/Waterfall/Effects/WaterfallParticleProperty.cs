using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall
{
  public enum WaterfallParticlePropertyType
  {
    Range,
    Float,
    Color
  }

  public class WaterfallParticleProperty
  {
    public string propertyName;
    public WaterfallParticlePropertyType propertyType;

    public virtual void Load(ConfigNode node) { }

    public virtual ConfigNode Save() => null;

    public virtual void Initialize(ParticleSystem p) { }
  }

  public class WaterfallParticleRangeProperty : WaterfallParticleProperty
  {
    public Vector2 propertyValue;


    public WaterfallParticleRangeProperty()
    {
      propertyType = WaterfallParticlePropertyType.Range;
    }

    public WaterfallParticleRangeProperty(ConfigNode node)
    {
      Load(node);
      propertyType = WaterfallParticlePropertyType.Range;
    }

    public override void Load(ConfigNode node)
    {
      node.TryGetValue("rangeName", ref propertyName);
      node.TryGetValue("value", ref propertyValue);
    }

    public override void Initialize(ParticleSystem s)
    {
      ParticleUtils.SetParticleSystemValue(propertyName, s, propertyValue);
    }

    public override ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.RangeNodeName;
      node.AddValue("rangeName", propertyName);
      node.AddValue("value", propertyValue);

      return node;
    }
  }


  public class WaterfallParticleFloatProperty : WaterfallParticleProperty
  {
    public float propertyValue;

    public WaterfallParticleFloatProperty()
    {
      propertyType = WaterfallParticlePropertyType.Float;
    }

    public WaterfallParticleFloatProperty(ConfigNode node)
    {
      Load(node);
      propertyType = WaterfallParticlePropertyType.Float;
    }

    public override void Load(ConfigNode node)
    {
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
      node.AddValue("floatName", propertyName);
      node.AddValue("value", propertyValue);

      return node;
    }
  }

  public class WaterfallParticleColorProperty : WaterfallParticleProperty
  {
    public Color propertyValue;

    public WaterfallParticleColorProperty()
    {
      propertyType = WaterfallParticlePropertyType.Color;
    }

    public WaterfallParticleColorProperty(ConfigNode node)
    {
      Load(node);
      propertyType = WaterfallParticlePropertyType.Color;
    }

    public override void Load(ConfigNode node)
    {
      node.TryGetValue("colorName", ref propertyName);
      node.TryGetValue("value", ref propertyValue);
    }

    public override void Initialize(ParticleSystem s)
    {
      ParticleUtils.SetParticleSystemValue(propertyName, s, propertyValue);
    }

    public override ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.ColorNodeName;
      node.AddValue("colorName", propertyName);
      node.AddValue("value", propertyValue);

      return node;
    }
  }
}
