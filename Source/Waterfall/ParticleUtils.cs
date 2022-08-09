using UnityEngine;

namespace Waterfall
{
  public static class ParticleUtils
  {
    public static void GetParticleSystemValue(string paramName, ParticleSystem system, out Vector2 result)
    {
      if (system == null)
      {
        result = Vector2.zero;
        return;
      }

      switch (paramName)
      {
        case "StartSpeed":
          result = new Vector2(system.main.startSpeed.constantMin, system.main.startSpeed.constantMax);
          break;
        case "StartSize":
          result = new Vector2(system.main.startSize.constantMin, system.main.startSize.constantMax);
          break;
        case "StartLifetime":
          result = new Vector2(system.main.startLifetime.constantMin, system.main.startLifetime.constantMax);
          break;
        case "EmissionRate":
          result = new Vector2(system.emission.rateOverTime.constantMin, system.emission.rateOverTime.constantMax);
          break;
        default:
          result = Vector2.zero;
          break;
      }

    }
    public static void GetParticleSystemValue(string paramName, ParticleSystem system, out float result)
    {
      if (system == null)
      {
        result = 0f;
        return;
      }
      switch (paramName)
      {
        case "MaxParticles":
          result = system.main.maxParticles;
          break;
        case "EmissionVolumeLength":
          result = system.shape.length;
          break;
        case "EmissionVolumeRadius":
          result = system.shape.radius;
          break;
        default:
          break;
      }
      result = 0f;

    }
    public static void SetParticleSystemValue(string paramName, ParticleSystem system, Vector2 paramValue)
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
        default:
          break;

      }

    }
    public static void SetParticleSystemValue(string paramName, ParticleSystem system, float paramValue)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.EmissionModule particleEmit = system.emission;
      ParticleSystem.ShapeModule particleShape = system.shape;

      switch (paramName)
      {
        case "MaxParticles":
          particleMain.maxParticles = (int)paramValue;
          break;
        case "EmissionVolumeLength":
          particleShape.length = paramValue;
          break;
        case "EmissionVolumeRadius":
          particleShape.radius = paramValue;
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
