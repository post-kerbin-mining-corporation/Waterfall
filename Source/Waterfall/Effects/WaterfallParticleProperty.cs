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
    Numeric,
    Color,
  }
  public class WaterfallParticleProperty
  {
    public string propertyName;
    public bool moduleEnabled;
    public WaterfallParticlePropertyType propertyType;

    public virtual void Load(ConfigNode node)
    {
    }

    public virtual ConfigNode Save() => null;

    public virtual void Initialize(ParticleSystem p) { }
  }

  public class WaterfallParticleNumericProperty : WaterfallParticleProperty
  {
    public float constant1Value;
    public float constant2Value;

    public FloatCurve curve1Value;
    public FloatCurve curve2Value;

    public ParticleSystemCurveMode curveMode;

    public WaterfallParticleNumericProperty()
    {
      propertyType = WaterfallParticlePropertyType.Numeric;
    }

    public WaterfallParticleNumericProperty(ConfigNode node)
    {
      Load(node);
      propertyType = WaterfallParticlePropertyType.Numeric;
    }

    public override void Load(ConfigNode node)
    {
      curve1Value = new();
      curve2Value = new();

      base.Load(node);
      node.TryGetValue("paramName", ref propertyName);
      node.TryGetValue("moduleEnabled", ref moduleEnabled);
      node.TryGetEnum("curveMode", ref curveMode, ParticleSystemCurveMode.Constant);
      switch (curveMode)
      {
        case ParticleSystemCurveMode.Constant:
          node.TryGetValue("constant1", ref constant1Value);
          break;
        case ParticleSystemCurveMode.TwoConstants:
          node.TryGetValue("constant1", ref constant1Value);
          node.TryGetValue("constant2", ref constant2Value);
          break;
        case ParticleSystemCurveMode.Curve:
          curve1Value.Load(node.GetNode("curve1"));
          break;
        case ParticleSystemCurveMode.TwoCurves:
          curve1Value.Load(node.GetNode("curve1"));
          curve1Value.Load(node.GetNode("curve2"));
          break;
      }
    }

    public override void Initialize(ParticleSystem s)
    {
      // TODO: Set the module enabled state
      ParticleUtils.SetParticleSystemMode(propertyName, s, curveMode); 
      switch (curveMode)
      {
        case ParticleSystemCurveMode.Constant:
          ParticleUtils.SetParticleSystemValue(propertyName, s, constant1Value);
          break;
        case ParticleSystemCurveMode.TwoConstants:
          ParticleUtils.SetParticleSystemValue(propertyName, s, constant1Value);
          ParticleUtils.SetParticleSystemValue(propertyName, s, constant2Value);
          break;
        case ParticleSystemCurveMode.Curve:
          ParticleUtils.SetParticleSystemValue(propertyName, s, curve1Value);
          break;
        case ParticleSystemCurveMode.TwoCurves:
          ParticleUtils.SetParticleSystemValue(propertyName, s, curve1Value);
          ParticleUtils.SetParticleSystemValue(propertyName, s, curve2Value);
          break;
      }
    }

    public override ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.FloatNodeName;
      node.AddValue("paramName", propertyName);
      node.AddValue("curveMode", curveMode);
      node.AddValue("moduleEnabled", moduleEnabled);

      switch (curveMode)
      {
        case ParticleSystemCurveMode.Constant:
          node.AddValue("constant1", constant1Value);
          node.AddValue("constant2", constant2Value);
          break;
        case ParticleSystemCurveMode.TwoConstants:
          node.AddValue("constant1", constant1Value);
          break;
        case ParticleSystemCurveMode.Curve:
          node.AddNode(Utils.SerializeFloatCurve("curve1", curve1Value));
          break;
        case ParticleSystemCurveMode.TwoCurves:
          node.AddNode(Utils.SerializeFloatCurve("curve1", curve1Value));
          node.AddNode(Utils.SerializeFloatCurve("curve2", curve2Value));
          break;
      }

      return node;
    }
  }
}
