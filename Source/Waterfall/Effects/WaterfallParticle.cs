using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// This manages a particle system
  /// </summary>
  public class WaterfallParticle
  {

    public string transformName = "";
    public string baseTransformName = "";
    public bool worldSpaceAlternateSimulation = false;

    protected ParticleSystem targetParticleSystem;

    public List<ParticleSystem> systems;


    public List<WaterfallParticleProperty> pProperties;

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
      node.TryGetValue("worldSpaceAlternateSimulation", ref worldSpaceAlternateSimulation);
      pProperties = new();
      foreach (var subnode in node.GetNodes(WaterfallConstants.FloatNodeName))
      {
        pProperties.Add(new WaterfallParticleNumericProperty(subnode));
      }
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.ParticleNodeName;
      node.AddValue("worldSpaceAlternateSimulation", worldSpaceAlternateSimulation);

      if (baseTransformName != "")
        node.AddValue("baseTransform", baseTransformName);

      if (transformName != "")
        node.AddValue("transform", transformName);

      foreach (var p in pProperties)
      {
        node.AddNode(p.Save());
      }

      return node;
    }

    public void Initialize(Transform parentTransform)
    {
      systems = new();

      var particleTarget = parentTransform.FindDeepChild(transformName);
      targetParticleSystem = particleTarget.GetComponent<ParticleSystem>();
      systems.Add(targetParticleSystem);


      foreach (var p in pProperties)
      {
        foreach (var sys in systems)
        {
          p.Initialize(sys);
        }
      }

      Utils.Log($"[WaterfallParticle]: Initialized Waterfall Particle at {parentTransform}, tracking {transformName}", LogType.Particles);
    }

    public void SetParticleModuleState(string propertyName, bool state)
    {
      /// TODO: This doesn't support color yet
      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleNumericProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.moduleEnabled = state;
      }
      else
      {
        var newProp = new WaterfallParticleNumericProperty
        {
          propertyName = propertyName,
          moduleEnabled = state
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleModuleState(propertyName, particle, state);
      }
    }
    /// Numeric setters
    public void SetParticleValue(string propertyName, float value)
    {
      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleNumericProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemCurveMode.Constant;
        t.constant1Value = value;
      }
      else
      {
        var newProp = new WaterfallParticleNumericProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemCurveMode.Constant,
          constant1Value = value,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value);
      }
    }
    public void SetParticleValue(string propertyName, float value1, float value2)
    {

      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleNumericProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemCurveMode.TwoConstants;
        t.constant1Value = value1;
        t.constant2Value = value2;
      }
      else
      {
        var newProp = new WaterfallParticleNumericProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemCurveMode.TwoConstants,
          constant1Value = value1,
          constant2Value = value2,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value1, value2);
      }
    }
    public void SetParticleValue(string propertyName, FloatCurve value)
    {
      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleNumericProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemCurveMode.Curve;
        t.curve1Value = value;
      }
      else
      {
        var newProp = new WaterfallParticleNumericProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemCurveMode.Curve,
          curve1Value = value,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value);
      }
    }
    public void SetParticleValue(string propertyName, FloatCurve value1, FloatCurve value2)
    {

      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleNumericProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemCurveMode.TwoCurves;
        t.curve1Value = value1;
        t.curve2Value = value2;
      }
      else
      {
        var newProp = new WaterfallParticleNumericProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemCurveMode.TwoCurves,
          curve1Value = value1,
          curve2Value = value2,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value1, value2);
      }
    }
    /// Color setters
    public void SetParticleValue(string propertyName, Color value)
    {
      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleColorProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemGradientMode.Color;
        t.constant1Value = value;
      }
      else
      {
        var newProp = new WaterfallParticleColorProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemGradientMode.Color,
          constant1Value = value,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value);
      }
    }
    public void SetParticleValue(string propertyName, Color value1, Color value2)
    {

      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleColorProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemGradientMode.TwoColors;
        t.constant1Value = value1;
        t.constant2Value = value2;
      }
      else
      {
        var newProp = new WaterfallParticleColorProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemGradientMode.TwoColors,
          constant1Value = value1,
          constant2Value = value2,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value1, value2);
      }
    }
    public void SetParticleValue(string propertyName, Gradient value)
    {
      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleColorProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemGradientMode.Gradient;
        t.gradient1Value = value;
      }
      else
      {
        var newProp = new WaterfallParticleColorProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemGradientMode.Gradient,
          gradient1Value = value,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value);
      }
    }
    public void SetParticleValue(string propertyName, Gradient value1, Gradient value2)
    {

      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleColorProperty t && prop != null)
      {
        t.propertyName = propertyName;
        t.curveMode = ParticleSystemGradientMode.TwoGradients;
        t.gradient1Value = value1;
        t.gradient2Value = value2;
      }
      else
      {
        var newProp = new WaterfallParticleColorProperty
        {
          propertyName = propertyName,
          curveMode = ParticleSystemGradientMode.TwoGradients,
          gradient1Value = value1,
          gradient2Value = value2,
        };
        pProperties.Add(newProp);
      }
      foreach (var particle in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, particle, value1, value2);
      }
    }
  }

}
