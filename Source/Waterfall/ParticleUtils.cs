using System.Collections.Generic;
using UnityEngine;
using Waterfall.UI;

namespace Waterfall
{
  public static class ParticleUtils
  {
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
        case "StartSpeed":
          result = system.main.startSpeed.constant;
          break;
        case "StartSize":
          result = system.main.startSize.constant;
          break;
        case "StartLifetime":
          result = system.main.startLifetime.constant;
          break;
        case "StartRotation":
          result = system.main.startRotation.constant;
          break;
        case "EmissionRateTime":
          result = system.emission.rateOverTime.constant;
          break;
        //case "EmissionRateBurst":
        //  result =  ParticleSystemCurveMode.Constant;
        //  break;
        case "EmissionVolumeLength":
          result = system.shape.length;
          break;
        case "EmissionVolumeRadius":
          result = system.shape.radius;
          break;
        case "EmissionArc":
          result = system.shape.arc;
          break;
        case "EmissionVolumeRadiusThickness":
          result = system.shape.radiusThickness;
          break;
        case "LimitVelocityMaxSpeed":
          result = system.limitVelocityOverLifetime.limit.constant;
          break;
        case "LimitVelocityDamping":
          result = system.limitVelocityOverLifetime.dampen;
          break;
        case "LimitVelocityDrag":
          result = system.limitVelocityOverLifetime.drag.constant;
          break;
        case "ForceX":
          result = system.forceOverLifetime.x.constant;
          break;
        case "ForceY":
          result = system.forceOverLifetime.y.constant;
          break;
        case "ForceZ":
          result = system.forceOverLifetime.z.constant;
          break;
        case "Size":
          result = system.sizeOverLifetime.size.constant;
          break;
        case "AngularVelocity":
          result = system.rotationOverLifetime.x.constant;
          break;
        default:
          result = 0f;
          break;
      }
    }

    public static void GetParticleSystemValue(string paramName, ParticleSystem system, out float result1, out float result2)
    {
      if (system == null)
      {
        result1 = 0f;
        result2 = 0f;
        return;
      }
      switch (paramName)
      {
        case "StartSpeed":
          result1 = system.main.startSpeed.constantMin;
          result2 = system.main.startSpeed.constantMax;
          break;
        case "StartSize":
          result1 = system.main.startSize.constantMin;
          result2 = system.main.startSize.constantMax;
          break;
        case "StartLifetime":
          result1 = system.main.startLifetime.constantMin;
          result2 = system.main.startLifetime.constantMax;
          break;
        case "StartRotation":
          result1 = system.main.startRotation.constantMin;
          result2 = system.main.startRotation.constantMax;
          break;
        case "EmissionRateTime":
          result1 = system.emission.rateOverTime.constantMin;
          result2 = system.emission.rateOverTime.constantMax;
          break;
        case "LimitVelocityMaxSpeed":
          result1 = system.limitVelocityOverLifetime.limit.constantMin;
          result2 = system.limitVelocityOverLifetime.limit.constantMax;
          break;
        case "LimitVelocityDrag":
          result1 = system.limitVelocityOverLifetime.drag.constantMin;
          result2 = system.limitVelocityOverLifetime.drag.constantMax;
          break;
        case "ForceX":
          result1 = system.forceOverLifetime.x.constantMin;
          result2 = system.forceOverLifetime.x.constantMax;
          break;
        case "ForceY":
          result1 = system.forceOverLifetime.y.constantMin;
          result2 = system.forceOverLifetime.y.constantMax;
          break;
        case "ForceZ":
          result1 = system.forceOverLifetime.z.constantMin;
          result2 = system.forceOverLifetime.z.constantMax;
          break;
        case "Size":
          result1 = system.sizeOverLifetime.size.constantMin;
          result2 = system.sizeOverLifetime.size.constantMax;
          break;
        case "AngularVelocity":
          result1 = system.rotationOverLifetime.x.constantMin;
          result2 = system.rotationOverLifetime.x.constantMax;
          break;
        default:
          result1 = 0f;
          result2 = 0f;
          break;
      }
    }

    public static void GetParticleSystemValue(string paramName, ParticleSystem system, ref FloatCurve result)
    {
      if (system == null)
      {
        return;
      }
      switch (paramName)
      {
        case "StartSpeed":
          result.Curve = system.main.startSpeed.curve;
          break;
        case "StartSize":
          result.Curve = system.main.startSize.curve;
          break;
        case "StartLifetime":
          result.Curve = system.main.startLifetime.curve;
          break;
        case "StartRotation":
          result.Curve = system.main.startRotation.curve;
          break;
        case "EmissionRateTime":
          result.Curve = system.emission.rateOverTime.curve;
          break;
        case "LimitVelocityMaxSpeed":
          result.Curve = system.limitVelocityOverLifetime.limit.curve;
          break;
        case "LimitVelocityDrag":
          result.Curve = system.limitVelocityOverLifetime.drag.curve;
          break;
        case "ForceX":
          result.Curve = system.forceOverLifetime.x.curve;
          break;
        case "ForceY":
          result.Curve = system.forceOverLifetime.y.curve;
          break;
        case "ForceZ":
          result.Curve = system.forceOverLifetime.z.curve;
          break;
        case "Size":
          result.Curve = system.sizeOverLifetime.size.curve;
          break;
        case "AngularVelocity":
          result.Curve = system.rotationOverLifetime.x.curve;
          break;
        default:
          break;
      }
    }

    public static void GetParticleSystemValue(string paramName, ParticleSystem system, ref FloatCurve result1, ref FloatCurve result2)
    {
      if (system == null)
      {
        return;
      }
      switch (paramName)
      {
        case "StartSpeed":
          result1.Curve = system.main.startSpeed.curveMin;
          result2.Curve = system.main.startSpeed.curveMax;
          break;
        case "StartSize":
          result1.Curve = system.main.startSize.curveMin;
          result2.Curve = system.main.startSize.curveMax;
          break;
        case "StartLifetime":
          result1.Curve = system.main.startLifetime.curveMin;
          result2.Curve = system.main.startLifetime.curveMax;
          break;
        case "StartRotation":
          result1.Curve = system.main.startRotation.curveMin;
          result2.Curve = system.main.startRotation.curveMax;
          break;
        case "EmissionRateTime":
          result1.Curve = system.emission.rateOverTime.curveMin;
          result2.Curve = system.emission.rateOverTime.curveMax;
          break;
        case "LimitVelocityMaxSpeed":
          result1.Curve = system.limitVelocityOverLifetime.limit.curveMin;
          result2.Curve = system.limitVelocityOverLifetime.limit.curve;
          break;
        case "LimitVelocityDrag":
          result1.Curve = system.limitVelocityOverLifetime.drag.curveMin;
          result2.Curve = system.limitVelocityOverLifetime.drag.curveMax;
          break;
        case "ForceX":
          result1.Curve = system.forceOverLifetime.x.curveMin;
          result2.Curve = system.forceOverLifetime.x.curveMax;
          break;
        case "ForceY":
          result1.Curve = system.forceOverLifetime.y.curveMin;
          result2.Curve = system.forceOverLifetime.y.curveMax;
          break;
        case "ForceZ":
          result1.Curve = system.forceOverLifetime.z.curveMin;
          result2.Curve = system.forceOverLifetime.z.curveMax;
          break;
        case "Size":
          result1.Curve = system.sizeOverLifetime.size.curveMin;
          result2.Curve = system.sizeOverLifetime.size.curveMax;
          break;
        case "AngularVelocity":
          result1.Curve = system.rotationOverLifetime.x.curveMin;
          result2.Curve = system.rotationOverLifetime.x.curveMax;
          break;
        default:
          break;
      }
    }
    public static void GetParticleSystemValue(string paramName, ParticleSystem system, out Color color)
    {
      if (system == null)
      {
        color = Color.white;
        return;
      }

      switch (paramName)
      {
        case "StartColor":
          color = system.main.startColor.color;
          break;
        case "Color":
          color = system.colorOverLifetime.color.color;
          break;
        default:
          color = Color.white;
          break;
      }
    }
    public static void GetParticleSystemValue(string paramName, ParticleSystem system, out Gradient gradient)
    {
      if (system == null)
      {
        gradient = new();
        return;
      }

      switch (paramName)
      {
        case "StartColor":
          gradient = system.main.startColor.gradient;
          break;
        case "Color":
          gradient = system.colorOverLifetime.color.gradient;
          break;
        default:
          gradient = new();
          break;
      }
    }
    public static void GetParticleSystemValue(string paramName, ParticleSystem system, out Color color1, out Color color2)
    {
      if (system == null)
      {
        color1 = Color.white;
        color2 = Color.white;
        return;
      }

      switch (paramName)
      {
        case "StartColor":
          color1 = system.main.startColor.colorMin;
          color2 = system.main.startColor.colorMax;
          break;
        case "Color":
          color1 = system.colorOverLifetime.color.colorMin;
          color2 = system.colorOverLifetime.color.colorMax;
          break;
        default:
          color1 = Color.white;
          color2 = Color.white;
          break;
      }
    }
    public static void GetParticleSystemValue(string paramName, ParticleSystem system, out Gradient gradient1, out Gradient gradient2)
    {
      if (system == null)
      {
        gradient1 = new();
        gradient2 = new();
        return;
      }

      switch (paramName)
      {
        case "StartColor":
          gradient1 = system.main.startColor.gradientMin;
          gradient2 = system.main.startColor.gradientMax;
          break;
        case "Color":
          gradient1 = system.colorOverLifetime.color.gradientMin;
          gradient2 = system.colorOverLifetime.color.gradientMax;
          break;
        default:
          gradient1 = new();
          gradient2 = new();
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
      ParticleSystem.ForceOverLifetimeModule particleForce = system.forceOverLifetime;
      ParticleSystem.RotationOverLifetimeModule particleRotation = system.rotationOverLifetime;
      ParticleSystem.SizeOverLifetimeModule particleSize = system.sizeOverLifetime;
      ParticleSystem.VelocityOverLifetimeModule particleVelocity = system.velocityOverLifetime;
      ParticleSystem.LimitVelocityOverLifetimeModule particleLimitVelocity = system.limitVelocityOverLifetime;

      ParticleSystem.MinMaxCurve paramCurve = new ParticleSystem.MinMaxCurve(paramValue);

      switch (paramName)
      {
        case "MaxParticles":
          particleMain.maxParticles = (int)paramValue;
          break;
        case "StartSpeed":
          particleMain.startSpeed = paramCurve;
          break;
        case "StartSize":
          particleMain.startSize = paramCurve;
          break;
        case "StartLifetime":
          particleMain.startLifetime = paramCurve;
          break;
        case "StartRotation":
          particleMain.startRotation = paramCurve;
          break;
        case "EmissionRateTime":
          particleEmit.rateOverTime = paramCurve;
          break;
        case "EmissionVolumeLength":
          particleShape.length = paramValue;
          break;
        case "EmissionVolumeRadius":
          particleShape.radius = paramValue;
          break;
        case "EmissionArc":
          particleShape.arc = paramValue;
          break;
        case "EmissionVolumeRadiusThickness":
          particleShape.radiusThickness = paramValue;
          break;
        case "LimitVelocityMaxSpeed":
          particleLimitVelocity.limit = paramValue;
          break;
        case "LimitVelocityDamping":
          particleLimitVelocity.dampen = paramValue;
          break;
        case "LimitVelocityDrag":
          particleLimitVelocity.drag = paramCurve;
          break;
        case "ForceX":
          particleForce.x = paramCurve;
          break;
        case "ForceY":
          particleForce.y = paramCurve;
          break;
        case "ForceZ":
          particleForce.z = paramCurve;
          break;
        case "Size":
          particleSize.size = paramCurve;
          break;
        case "AngularVelocity":
          particleRotation.x = paramCurve;
          break;
        default:
          break;
      }
    }

    public static void SetParticleSystemValue(string paramName, ParticleSystem system, float paramValue, float paramValue2)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.EmissionModule particleEmit = system.emission;
      ParticleSystem.ShapeModule particleShape = system.shape;
      ParticleSystem.ForceOverLifetimeModule particleForce = system.forceOverLifetime;
      ParticleSystem.RotationOverLifetimeModule particleRotation = system.rotationOverLifetime;
      ParticleSystem.SizeOverLifetimeModule particleSize = system.sizeOverLifetime;
      ParticleSystem.VelocityOverLifetimeModule particleVelocity = system.velocityOverLifetime;
      ParticleSystem.LimitVelocityOverLifetimeModule particleLimitVelocity = system.limitVelocityOverLifetime;

      ParticleSystem.MinMaxCurve paramCurve = new ParticleSystem.MinMaxCurve(paramValue, paramValue2);

      switch (paramName)
      {
        case "StartSpeed":
          particleMain.startSpeed = paramCurve;
          break;
        case "StartSize":
          particleMain.startSize = paramCurve;
          break;
        case "StartLifetime":
          particleMain.startLifetime = paramCurve;
          break;
        case "StartRotation":
          particleMain.startRotation = paramCurve;
          break;
        case "EmissionRateTime":
          particleEmit.rateOverTime = paramCurve;
          break;
        case "LimitVelocityDrag":
          particleLimitVelocity.drag = paramCurve;
          break;
        case "ForceX":
          particleForce.x = paramCurve;
          break;
        case "ForceY":
          particleForce.y = paramCurve;
          break;
        case "ForceZ":
          particleForce.z = paramCurve;
          break;
        case "Size":
          particleSize.size = paramCurve;
          break;
        case "AngularVelocity":
          particleRotation.x = paramCurve;
          break;
        default:
          break;
      }
    }
    public static void SetParticleSystemValue(string paramName, ParticleSystem system, FloatCurve curve)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.EmissionModule particleEmit = system.emission;
      ParticleSystem.ShapeModule particleShape = system.shape;
      ParticleSystem.ForceOverLifetimeModule particleForce = system.forceOverLifetime;
      ParticleSystem.RotationOverLifetimeModule particleRotation = system.rotationOverLifetime;
      ParticleSystem.SizeOverLifetimeModule particleSize = system.sizeOverLifetime;
      ParticleSystem.VelocityOverLifetimeModule particleVelocity = system.velocityOverLifetime;
      ParticleSystem.LimitVelocityOverLifetimeModule particleLimitVelocity = system.limitVelocityOverLifetime;

      ParticleSystem.MinMaxCurve paramCurve = new ParticleSystem.MinMaxCurve(1f, curve.Curve);

      switch (paramName)
      {
        case "StartSpeed":
          particleMain.startSpeed = paramCurve;
          break;
        case "StartSize":
          particleMain.startSize = paramCurve;
          break;
        case "StartLifetime":
          particleMain.startLifetime = paramCurve;
          break;
        case "StartRotation":
          particleMain.startRotation = paramCurve;
          break;
        case "EmissionRateTime":
          particleEmit.rateOverTime = paramCurve;
          break;
        case "LimitVelocityDrag":
          particleLimitVelocity.drag = paramCurve;
          break;
        case "ForceX":
          particleForce.x = paramCurve;
          break;
        case "ForceY":
          particleForce.y = paramCurve;
          break;
        case "ForceZ":
          particleForce.z = paramCurve;
          break;
        case "Size":
          particleSize.size = paramCurve;
          break;
        case "AngularVelocity":
          particleRotation.x = paramCurve;
          break;
        default:
          break;
      }
    }

    public static void SetParticleSystemValue(string paramName, ParticleSystem system, FloatCurve curve1, FloatCurve curve2)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.EmissionModule particleEmit = system.emission;
      ParticleSystem.ShapeModule particleShape = system.shape;
      ParticleSystem.ForceOverLifetimeModule particleForce = system.forceOverLifetime;
      ParticleSystem.RotationOverLifetimeModule particleRotation = system.rotationOverLifetime;
      ParticleSystem.SizeOverLifetimeModule particleSize = system.sizeOverLifetime;
      ParticleSystem.VelocityOverLifetimeModule particleVelocity = system.velocityOverLifetime;
      ParticleSystem.LimitVelocityOverLifetimeModule particleLimitVelocity = system.limitVelocityOverLifetime;

      ParticleSystem.MinMaxCurve paramCurve = new ParticleSystem.MinMaxCurve(1f, curve1.Curve, curve2.Curve);

      switch (paramName)
      {
        case "StartSpeed":
          particleMain.startSpeed = paramCurve;
          break;
        case "StartSize":
          particleMain.startSize = paramCurve;
          break;
        case "StartLifetime":
          particleMain.startLifetime = paramCurve;
          break;
        case "StartRotation":
          particleMain.startRotation = paramCurve;
          break;
        case "EmissionRateTime":
          particleEmit.rateOverTime = paramCurve;
          break;
        case "LimitVelocityDrag":
          particleLimitVelocity.drag = paramCurve;
          break;
        case "ForceX":
          particleForce.x = paramCurve;
          break;
        case "ForceY":
          particleForce.y = paramCurve;
          break;
        case "ForceZ":
          particleForce.z = paramCurve;
          break;
        case "Size":
          particleSize.size = paramCurve;
          break;
        case "AngularVelocity":
          particleRotation.x = paramCurve;
          break;
        default:
          break;
      }
    }

    public static void SetParticleSystemValue(string paramName, ParticleSystem system, Color color)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.ColorOverLifetimeModule particleColor = system.colorOverLifetime;

      ParticleSystem.MinMaxGradient colorGrad = new ParticleSystem.MinMaxGradient(color);

      switch (paramName)
      {
        case "StartColor":
          particleMain.startColor = colorGrad;
          break;
        case "Color":
          particleColor.color = colorGrad;
          break;
        default:
          break;
      }
    }
    public static void SetParticleSystemValue(string paramName, ParticleSystem system, Gradient gradient)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.ColorOverLifetimeModule particleColor = system.colorOverLifetime;

      ParticleSystem.MinMaxGradient colorGrad = new ParticleSystem.MinMaxGradient(gradient);

      switch (paramName)
      {
        case "StartColor":
          particleMain.startColor = colorGrad;
          break;
        case "Color":
          particleColor.color = colorGrad;
          break;
        default:
          break;
      }
    }
    public static void SetParticleSystemValue(string paramName, ParticleSystem system, Color color1, Color color2)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.ColorOverLifetimeModule particleColor = system.colorOverLifetime;

      ParticleSystem.MinMaxGradient colorGrad = new ParticleSystem.MinMaxGradient(color1, color2);

      switch (paramName)
      {
        case "StartColor":
          particleMain.startColor = colorGrad;
          break;
        case "Color":
          particleColor.color = colorGrad;
          break;
        default:
          break;
      }
    }
    public static void SetParticleSystemValue(string paramName, ParticleSystem system, Gradient gradient1, Gradient gradient2)
    {
      if (system == null)
        return;

      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.ColorOverLifetimeModule particleColor = system.colorOverLifetime;

      ParticleSystem.MinMaxGradient colorGrad = new ParticleSystem.MinMaxGradient(gradient1, gradient2);

      switch (paramName)
      {
        case "StartColor":
          particleMain.startColor = colorGrad;
          break;
        case "Color":
          particleColor.color = colorGrad;
          break;
        default:
          break;
      }
    }
    public static ParticleSystemCurveMode GetParticleSystemMode(string paramName, ParticleSystem system)
    {
      if (system == null)
        return ParticleSystemCurveMode.Constant;

      switch (paramName)
      {
        case "MaxParticles":
          return ParticleSystemCurveMode.Constant;
        case "StartSpeed":
          return system.main.startSpeed.mode;
        case "StartSize":
          return system.main.startSize.mode;
        case "StartLifetime":
          return system.main.startLifetime.mode;
        case "StartRotation":
          return system.main.startRotation.mode;
        case "EmissionRateTime":
          return system.emission.rateOverTime.mode;
        case "EmissionRateBurst":
          return ParticleSystemCurveMode.Constant;
        case "EmissionVolumeLength":
          return ParticleSystemCurveMode.Constant;
        case "EmissionVolumeRadius":
          return ParticleSystemCurveMode.Constant;
        case "EmissionArc":
          return ParticleSystemCurveMode.Constant;
        case "EmissionVolumeRadiusAlternate":
          return ParticleSystemCurveMode.Constant;
        case "LimitVelocityMaxSpeed":
          return system.limitVelocityOverLifetime.limit.mode;
        case "LimitVelocityDamping":
          return ParticleSystemCurveMode.Constant;
        case "LimitVelocityDrag":
          return system.limitVelocityOverLifetime.drag.mode; ;
        case "ForceX":
          return system.forceOverLifetime.x.mode;
        case "ForceY":
          return system.forceOverLifetime.y.mode;
        case "ForceZ":
          return system.forceOverLifetime.z.mode;
        case "Size":
          return system.sizeOverLifetime.size.mode;
        case "AngularVelocity":
          return system.rotationOverLifetime.x.mode;
        default:
          return ParticleSystemCurveMode.Constant;
      }

    }

    public static void SetParticleSystemMode(string paramName, ParticleSystem system, ParticleSystemCurveMode newMode)
    {
      if (system == null)
        return;


      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.EmissionModule particleEmit = system.emission;
      ParticleSystem.ShapeModule particleShape = system.shape;
      ParticleSystem.ForceOverLifetimeModule particleForce = system.forceOverLifetime;
      ParticleSystem.RotationOverLifetimeModule particleRotation = system.rotationOverLifetime;
      ParticleSystem.SizeOverLifetimeModule particleSize = system.sizeOverLifetime;
      ParticleSystem.VelocityOverLifetimeModule particleVelocity = system.velocityOverLifetime;
      ParticleSystem.LimitVelocityOverLifetimeModule particleLimitVelocity = system.limitVelocityOverLifetime;

      // For the love of god why unity
      switch (paramName)
      {
        case "StartSpeed":
          var spd = particleMain.startSpeed;
          spd.mode = newMode;
          particleMain.startSpeed = spd;
          break;
        case "StartSize":
          var sz = particleMain.startSize;
          sz.mode = newMode;
          particleMain.startSize = sz;
          break;
        case "StartLifetime":
          var life = particleMain.startLifetime;
          life.mode = newMode;
          particleMain.startLifetime = life;
          break;
        case "StartRotation":
          var rot = particleMain.startRotation;
          rot.mode = newMode;
          particleMain.startRotation = rot;
          break;
        case "EmissionRateTime":
          var er = particleEmit.rateOverTime;
          er.mode = newMode;
          particleEmit.rateOverTime = er;
          break;
        case "LimitVelocityMaxSpeed":
          var limitSpd = particleLimitVelocity.limit;
          limitSpd.mode = newMode;
          particleLimitVelocity.limit = limitSpd;
          break;
        case "LimitVelocityDrag":
          var drag = particleLimitVelocity.drag;
          drag.mode = newMode;
          particleLimitVelocity.limit = drag;
          break;
        case "ForceX":
          var fx = particleForce.x;
          fx.mode = newMode;
          particleForce.x = fx;
          break;
        case "ForceY":
          var fy = particleForce.x;
          fy.mode = newMode;
          particleForce.x = fy;
          break;
        case "ForceZ":
          var fz = particleForce.x;
          fz.mode = newMode;
          particleForce.x = fz;
          break;
        case "Size":
          var szL = particleSize.x;
          szL.mode = newMode;
          particleSize.x = szL;
          break;
        case "AngularVelocity":
          var rx = particleRotation.x;
          rx.mode = newMode;
          particleRotation.x = rx;
          break;

      }

    }

    public static ParticleSystemGradientMode GetParticleSystemColorMode(string paramName, ParticleSystem system)
    {
      if (system == null)
        return ParticleSystemGradientMode.Color;

      switch (paramName)
      {
        case "StartColor":
          return system.main.startColor.mode;
        case "Color":
          return system.colorOverLifetime.color.mode;
        default:
          return ParticleSystemGradientMode.Color;
      }
    }
    public static void SetParticleSystemColorMode(string paramName, ParticleSystem system, ParticleSystemGradientMode newMode)
    {
      if (system == null)
        return;
      ParticleSystem.MainModule particleMain = system.main;
      ParticleSystem.ColorOverLifetimeModule particleColor = system.colorOverLifetime;

      // For the love of god why unity
      switch (paramName)
      {
        case "StartColor":
          var col = particleMain.startColor;
          col.mode = newMode;
          particleMain.startColor = col;
          break;
        case "Color":
          var sz = particleColor.color;
          sz.mode = newMode;
          particleColor.color = sz;
          break;
      }
    }
    public static bool GetParticleModuleState(string paramName, ParticleSystem system)
    {
      if (system = null)
        return false;

      switch (paramName)
      {
        case "Main":
          return true;
        case "Emission":
          return true;
        case "Emitter Shape":
          return system.shape.enabled;
        case "Limit Velocity over Lifetime":
          return system.limitVelocityOverLifetime.enabled;
        case "Color vs Lifetime":
          return system.colorOverLifetime.enabled;
        case "Size vs Lifetime":
          return system.sizeOverLifetime.enabled;
        case "Rotation vs Lifetime":
          return system.rotationOverLifetime.enabled;
        default: return false;
      }
    }
    public static void SetParticleModuleState(string paramName, ParticleSystem system, bool state)
    {
      if (system == null)
        return;

      ParticleSystem.EmissionModule particleEmit = system.emission;
      ParticleSystem.ShapeModule particleShape = system.shape;
      ParticleSystem.RotationOverLifetimeModule particleRotation = system.rotationOverLifetime;
      ParticleSystem.SizeOverLifetimeModule particleSize = system.sizeOverLifetime;
      ParticleSystem.LimitVelocityOverLifetimeModule particleLimitVelocity = system.limitVelocityOverLifetime;
      ParticleSystem.ColorOverLifetimeModule particleColor = system.colorOverLifetime;

      switch (paramName)
      {
        case "Emission": 
          particleEmit.enabled = state;
          break;
        case "Emitter Shape": 
          particleShape.enabled = state;
          break;
        case "Limit Velocity over Lifetime":
          particleLimitVelocity.enabled = state; 
          break;
        case "Color vs Lifetime":
          particleColor.enabled = state; 
          break;
        case "Size vs Lifetime":
          particleSize.enabled = state; 
          break;
        case "Rotation vs Lifetime":
          particleRotation.enabled = state; 
          break;
        default: break;

      }
    }
  }



  public enum WaterfallParticleParameterMode
  {
    Constant,
    TwoConstants,
    Curve,
    TwoCurves,
    Color,
    TwoColors,
    Gradient,
    TwoGradients
  }
  public enum ParticleParameterType
  {
    Value,
    Vector2,
    Vector3,
    Bool,
    Enum
  }

  public class ParticleData
  {
    public string name;
    public string category;
    public Vector2 floatRange;
    public WaterfallParticlePropertyType type;
    public List<WaterfallParticleParameterMode> validModes;

    public ParticleData(string theName, string theCategory, WaterfallParticlePropertyType theType, List<WaterfallParticleParameterMode> modes, Vector2 range)
    {
      name = theName;
      type = theType;
      category = theCategory;
      validModes = modes;
      floatRange = range;
    }

  }

}
