using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public static class ParticleUtils
  {
    public static Vector2 GetParticleSystemRangeValue(string paramName, ParticleSystem system)
    {
      if (system == null)
        return Vector2.zero;

      switch (paramName)
      {
        case "StartSpeed":
          return new Vector2(system.main.startSpeed.constantMin, system.main.startSpeed.constantMax);
        case "StartSize":
          return new Vector2(system.main.startSize.constantMin, system.main.startSize.constantMax);
        case "StartLifetime":
          return new Vector2(system.main.startLifetime.constantMin, system.main.startLifetime.constantMax);
        case "EmissionRate":
          return new Vector2(system.emission.rateOverTime.constantMin, system.emission.rateOverTime.constantMax);
        case "MaxParticles":
          return new Vector2(system.main.maxParticles, system.main.maxParticles);
        case "EmissionVolumeLength":
          return new Vector2(system.shape.length, system.shape.length);
        case "EmissionVolumeRadius":
          return new Vector2(system.shape.radius, system.shape.radius);
      }
      return Vector2.zero;

    }
    public static void SetParticleSystemRangeValue(string paramName, ParticleSystem system, Vector2 paramValue)
    {
      if (system == null)
        return;

      ParticleSystem.MinMaxCurve newCurve = new ParticleSystem.MinMaxCurve(paramValue.x, paramValue.y);
      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.EmissionModule particleEmit = system.emission;
      ParticleSystem.ShapeModule particleShape = system.shape;

      switch (paramName)
      {
        case "StartSpeed":
          particleMain.startSpeed = newCurve;
          break;
        case "StartSize":
          particleMain.startSize = newCurve;
          break;
        case "StartLifetime":
          particleMain.startLifetime = newCurve;
          break;
        case "EmissionRate":
          particleEmit.rateOverTime = newCurve;
          break;
        case "MaxParticles":
          particleMain.maxParticles = (int)paramValue.x;
          break;
        case "EmissionVolumeLength":
          particleShape.length = paramValue.x;
          break;
        case "EmissionVolumeRadius":
          particleShape.radius = paramValue.x;
          break;
        default:
          break;

      }

    }

  }

  public enum ParticleParameterType
  {
    Value,
    Range
  }
  public class ParticleParameterData
  {
    /// <summary>
    /// The parameter name
    /// </summary>
    public string Name;

    /// <summary>
    /// The type,
    /// </summary>
    public ParticleParameterType ParamType;

    public ParticleParameterData(string nm, ParticleParameterType tp)
    {

    }
  }

  public class ParticleData
  {
    public Vector2 floatRange;
    public WaterfallParticlePropertyType type;

    public ParticleData(WaterfallParticlePropertyType theType, Vector2 range)
    {
      type = theType;
      floatRange = range;
    }

    public ParticleData(WaterfallParticlePropertyType theType)
    {
      type = theType;
    }
  }

}
