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
      ConfigNode node = new();
      node.name = WaterfallConstants.FloatNodeName;
      node.AddValue("paramName", propertyName);
      node.AddValue("curveMode", curveMode);
      node.AddValue("moduleEnabled", moduleEnabled);

      switch (curveMode)
      {
        case ParticleSystemCurveMode.Constant:
          node.AddValue("constant1", constant1Value);
          break;
        case ParticleSystemCurveMode.TwoConstants:
          node.AddValue("constant1", constant1Value);
          node.AddValue("constant2", constant2Value);
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

  public class WaterfallParticleColorProperty : WaterfallParticleProperty
  {
    public Color constant1Value;
    public Color constant2Value;

    public Gradient gradient1Value;
    public Gradient gradient2Value;

    public ParticleSystemGradientMode curveMode;

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
      gradient1Value = new();
      gradient2Value = new();
      ConfigNode gradientNode = new();

      base.Load(node);
      node.TryGetValue("paramName", ref propertyName);
      node.TryGetValue("moduleEnabled", ref moduleEnabled);
      node.TryGetEnum("curveMode", ref curveMode, ParticleSystemGradientMode.Color);
      // TODO: load gradients
      switch (curveMode)
      {
        case ParticleSystemGradientMode.Color:
          node.TryGetValue("constant1", ref constant1Value);
          break;
        case ParticleSystemGradientMode.TwoColors:
          node.TryGetValue("constant1", ref constant1Value);
          node.TryGetValue("constant2", ref constant2Value);
          break;
        case ParticleSystemGradientMode.Gradient:
          if (node.TryGetNode("gradient1", ref gradientNode))
          {
            gradientNode.TryParseGradient(ref gradient1Value);
          }
          break;
        case ParticleSystemGradientMode.TwoGradients:
          if (node.TryGetNode("gradient1", ref gradientNode))
          {
            gradientNode.TryParseGradient(ref gradient1Value);
          }
          if (node.TryGetNode("gradient2", ref gradientNode))
          {
            gradientNode.TryParseGradient(ref gradient2Value);
          }
          break;
      }
    }

    public override void Initialize(ParticleSystem s)
    {
      // TODO: Set the module enabled state
      ParticleUtils.SetParticleSystemColorMode(propertyName, s, curveMode);
      switch (curveMode)
      {
        case ParticleSystemGradientMode.Color:
          ParticleUtils.SetParticleSystemValue(propertyName, s, constant1Value);
          break;
        case ParticleSystemGradientMode.TwoColors:
          ParticleUtils.SetParticleSystemValue(propertyName, s, constant1Value);
          ParticleUtils.SetParticleSystemValue(propertyName, s, constant2Value);
          break;
        case ParticleSystemGradientMode.Gradient:
          ParticleUtils.SetParticleSystemValue(propertyName, s, gradient1Value);
          break;
        case ParticleSystemGradientMode.TwoGradients:
          ParticleUtils.SetParticleSystemValue(propertyName, s, gradient1Value);
          ParticleUtils.SetParticleSystemValue(propertyName, s, gradient2Value);
          break;
      }
    }

    public override ConfigNode Save()
    {
      ConfigNode node = new();
      node.name = WaterfallConstants.ColorNodeName;
      node.AddValue("paramName", propertyName);
      node.AddValue("curveMode", curveMode);
      node.AddValue("moduleEnabled", moduleEnabled);
      switch (curveMode)
      {
        case ParticleSystemGradientMode.Color:
          node.AddValue("constant1", constant1Value);
          break;
        case ParticleSystemGradientMode.TwoColors:
          node.AddValue("constant1", constant1Value);
          node.AddValue("constant2", constant2Value);
          break;
        case ParticleSystemGradientMode.Gradient:
          node.AddNode(Utils.SerializeGradient("gradient1", gradient1Value, 1f));
          break;
        case ParticleSystemGradientMode.TwoGradients:
          node.AddNode(Utils.SerializeGradient("gradient1", gradient1Value, 1f));
          node.AddNode(Utils.SerializeGradient("gradient2", gradient2Value, 1f));
          break;
      }

      return node;
    }
  }
}
