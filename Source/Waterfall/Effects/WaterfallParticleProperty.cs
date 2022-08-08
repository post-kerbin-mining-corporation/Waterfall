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
    Range
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
      Utils.Log($"[WaterfallParticleRangeProperty]: loaded Vector2 {propertyName} with value {propertyValue.ToString()}", LogType.Effects);
    }

    public override void Initialize(ParticleSystem s)
    {
      ParticleUtils.SetParticleSystemRangeValue(propertyName, s, propertyValue);
    }

    public override ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.Vector4NodeName;
      node.AddValue("rangeName", propertyName);
      node.AddValue("value", propertyValue);

      return node;
    }
  }
}
