using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// This class manages a single Shuriken system
  /// </summary>
  public class WaterfallParticleSystem
  {
    private Transform transform;
    private Transform parent;
    public ParticleSystem emitter;
    private ParticleSystemRenderer renderer;

    private ParticleSystem.Particle[] particleBuffer;

    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.EmissionModule particleEmit;
    private ParticleSystem.ShapeModule particleShape;

    public WaterfallParticleSystem(ParticleSystem source)
    {
      transform = source.transform;
      parent = transform.parent;

      emitter = source;
      renderer = source.GetComponent<ParticleSystemRenderer>();


      if (emitter)
      {
        particleMain = emitter.main;
        particleEmit = emitter.emission;
        particleShape = emitter.shape;

        Utils.Log($"[WaterfallParticleSystem]: Set up emitter {emitter} on {transform.name}. \n" +
          $"emit: {particleEmit} \n" +
          $"shape {particleShape} \n" +
          $"main {particleMain}", LogType.Particles);
      }
    }


    public void Update()
    {
      if (emitter != null)
      {
        if (FlightGlobals.ActiveVessel != null && (Krakensbane.GetFrameVelocity().magnitude > 0f))
        {
          Vector3d frameVel = Krakensbane.GetFrameVelocity();


          particleBuffer = new ParticleSystem.Particle[emitter.particleCount];

          int particleCount = emitter.GetParticles(particleBuffer);
          int pc = particleCount;

          float distancePerFrame = (float)frameVel.magnitude * TimeWarp.deltaTime;
          Vector3 nrmVelocity = -frameVel.normalized;
          while (particleCount > 0)
          {
            particleBuffer[particleCount - 1].position =
              particleBuffer[particleCount - 1].position +
              (-frameVel * TimeWarp.deltaTime) -
              UnityEngine.Random.Range(0f, distancePerFrame) * nrmVelocity;
            particleCount--;

          }
          emitter.SetParticles(particleBuffer, pc);
        }
      }


    }

    public void Get(string paramName, out Color result)
    {
      if (emitter == null)
      {
        result = Color.white;
        return;
      }
      switch (paramName)
      {
        case "StartColor":
          result = particleMain.startColor.color;
          break;
        default:
          result = Color.white;
          break;
      }
    }

    public void Get(string paramName, out Vector2 result)
    {
      if (emitter == null)
      {
        result = Vector2.zero;
        return;
      }

      switch (paramName)
      {
        case "StartSpeed":
          result = new Vector2(particleMain.startSpeed.constantMin, particleMain.startSpeed.constantMax);
          break;
        case "StartSize":
          result = new Vector2(particleMain.startSize.constantMin, particleMain.startSize.constantMax);
          break;
        case "StartLifetime":
          result = new Vector2(particleMain.startLifetime.constantMin, particleMain.startLifetime.constantMax);
          break;
        case "EmissionRate":
          result = new Vector2(particleEmit.rateOverTime.constantMin, particleEmit.rateOverTime.constantMax);
          break;
        default:
          result = Vector2.zero;
          break;
      }

    }
    public void Get(string paramName, out float result)
    {
      if (emitter == null)
      {
        result = 0f;
        return;
      }
      switch (paramName)
      {
        case "MaxParticles":
          result = particleMain.maxParticles;
          break;
        case "EmissionVolumeLength":
          result = particleShape.length;
          break;
        case "EmissionVolumeRadius":
          result = particleShape.radius;
          break;
        default:
          break;
      }
      result = 0f;

    }
    public void Set(string paramName, Vector2 paramValue)
    {
      if (emitter == null)
        return;

      ParticleSystem.MinMaxCurve newCurve = new ParticleSystem.MinMaxCurve(paramValue.x, paramValue.y);

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
    public void Set(string paramName, float paramValue)
    {
      if (emitter == null)
        return;

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
    public void Set(string paramName, Color paramValue)
    {
      if (emitter == null)
        return;

      ParticleSystem.MinMaxGradient newCurve = new ParticleSystem.MinMaxGradient(paramValue);

      switch (paramName)
      {
        case "StartColor":
          particleMain.startColor = newCurve;
          break;
        default:
          break;
      }

    }



  }
}
