using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{
  public class WaterfallParticle
  {

    public string transformName = "";
    public string baseTransformName = "";
    public string assetName = "";

    public List<WaterfallParticleEmitter> systems;

    public WaterfallParticle()
    { }

    public WaterfallParticle(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue("transform", ref transformName);
      node.TryGetValue("baseTransform", ref baseTransformName);
      node.TryGetValue("assetName", ref assetName);

      Utils.Log(String.Format($"[WaterfallParticle]: Loading new particle {assetName} for {transformName}"), LogType.Effects);
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.ParticleNodeName;
      node.AddValue("transform", transformName);
      node.AddValue("assetName", assetName);
      return node;
    }

    public void Initialize(Transform parentTransform)
    {
      systems = new List<WaterfallParticleEmitter>();


      GameObject go = GameObject.Instantiate(WaterfallParticleLoader.GetParticles(assetName),
            Vector3.zero, Quaternion.identity) as GameObject;
      go.transform.SetParent(parentTransform);
      go.transform.localPosition = Vector3.zero;
      go.transform.localScale = Vector3.one;
      go.transform.localRotation = Quaternion.identity;

      systems.Add(go.AddComponent<WaterfallParticleEmitter>());


      Utils.Log($"[WaterfallParticle]: Initialized Waterfall Particle at {parentTransform}, {systems.Count} Count", LogType.Effects);
    }

    public void SetParticleRange(string propertyName, Vector2 value)
    {
      foreach( var p in systems)
      {
        ParticleUtils.SetParticleSystemRangeValue(propertyName, p.emitter, value);
      }
    }

  }

  public class WaterfallParticleEmitter : MonoBehaviour
  {
    private Transform self;
    private Transform parent;
    public ParticleSystem emitter;
    private ParticleSystemRenderer renderer;

    private ParticleSystem.Particle[] particleBuffer;

    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.EmissionModule particleEmit;
    private ParticleSystem.ShapeModule particleShape;

    public void Awake()
    {
      self = transform;
      parent = self.parent;

      emitter = self.GetComponent<ParticleSystem>();
      renderer = self.GetComponent<ParticleSystemRenderer>();


      if (emitter)
      {
        particleMain = emitter.main;
        particleEmit = emitter.emission;
        particleShape = emitter.shape;
        Utils.Log($"[WaterfallParticleEmitter]: Set up emitter {emitter} on {transform.name}. \n" +
          $"emit: {particleEmit} \n" +
          $"shape {particleShape} \n" +
          $"main {particleMain}", LogType.Effects);
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
          Vector3 nrmVelocity = (-frameVel.normalized);
          while (particleCount > 0)
          {
            particleBuffer[particleCount - 1].position =
              particleBuffer[particleCount - 1].position + (-frameVel * TimeWarp.deltaTime) - UnityEngine.Random.Range(0f, distancePerFrame) * nrmVelocity;
            particleCount--;

          }
          emitter.SetParticles(particleBuffer, pc);
        }
      }


    }

    public Vector2 Get(string paramName)
    {
      if (emitter != null)
        return Vector2.zero;

      switch (paramName)
      {
        case "StartSpeed":
          return new Vector2(particleMain.startSpeed.constantMin, particleMain.startSpeed.constantMax);
        case "StartSize":
          return new Vector2(particleMain.startSize.constantMin, particleMain.startSize.constantMax);
        case "StartLifetime":
          return new Vector2(particleMain.startLifetime.constantMin, particleMain.startLifetime.constantMax);
        case "EmissionRate":
          return new Vector2(particleEmit.rateOverTime.constantMin, particleEmit.rateOverTime.constantMax);
        case "MaxParticles":
          return new Vector2(particleMain.maxParticles, particleMain.maxParticles);
        case "EmissionVolumeLength":
          return new Vector2(particleShape.length, particleShape.length);
        case "EmissionVolumeRadius":
          return new Vector2(particleShape.radius, particleShape.radius);
      }
      return Vector2.zero;
    }
    public void Set(string paramName, Vector2 paramValue)
    {
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
}
