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

    private List<WaterfallParticleEmitter> systems;

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


    //public void SetRanges(
    //  Vector2 emissionRangeNew,
    //  Vector2 speedRangeNew,
    //  Vector2 sizeRangeNew,
    //  Vector2 lifetimeRangeNew)
    //{
    //  emissionRateRange = emissionRangeNew;
    //  emissionSpeedRange = speedRangeNew;
    //  startSizeRange = sizeRangeNew;
    //  lifetimeRange = lifetimeRangeNew;
    //}
    //protected void SetParticles()
    //{
    //  float srfSpeed = 0;// (float)part.vessel.srfSpeed;
    //  float pressureAtm = 1;// (float)vessel.mainBody.GetPressureAtm(part.vessel.altitude);
    //  for (int i = 0; i < emitters.Count; i++)
    //  {
    //    emitters[i].Set(
    //      emissionRateRange * SpeedEmissionScaleCurve.Evaluate(srfSpeed),
    //      emissionSpeedRange,
    //      startSizeRange * AtmoSizeScaleCurve.Evaluate(pressureAtm),
    //      lifetimeRange * SpeedLifetimeScaleCurve.Evaluate(srfSpeed),
    //      AtmoAlphaFadeCurve.Evaluate(pressureAtm));
    //  }
    //}

  }

  public class WaterfallParticleEmitter : MonoBehaviour
  {
    private Transform self;
    private Transform parent;
    private ParticleSystem emitter;
    private ParticleSystemRenderer renderer;

    private ParticleSystem.MainModule main;
    private ParticleSystem.EmissionModule emit;
    private ParticleSystem.ShapeModule shape;
    private static ParticleSystem.Particle[] particles;

    public void Start()
    {
      self = transform;
      parent = self.parent;

      emitter = self.GetComponent<ParticleSystem>();
      renderer = self.GetComponent<ParticleSystemRenderer>();


      if (emitter)
      {
        main = emitter.main;
        emit = emitter.emission;
        shape = emitter.shape;
      }

      // FloatingOrigin.RegisterParticleSystem(emitter);
    }

    Vector3d trueVelocity;

    ParticleSystem.Particle[] particleBuffer;

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
          //Utils.Log($"D/frame: {distancePerFrame}, |v|: {frameVel.magnitude}");
          while (particleCount > 0)
          {
            particleBuffer[particleCount - 1].position =
              particleBuffer[particleCount - 1].position + (-frameVel * TimeWarp.deltaTime) - UnityEngine.Random.Range(0f, distancePerFrame) * nrmVelocity;
            //particleBuffer[particleCount - 1].velocity = -frameVel + UnityEngine.Random.insideUnitSphere*0.4f;
            particleCount--;

          }
          emitter.SetParticles(particleBuffer, pc);
        }
        ///Utils.Log($"Krakensbane vel {Krakensbane.GetFrameVelocity()}, thresh {FloatingOrigin.fetch.threshold} ({FlightGlobals.ActiveVessel.obt_velocity.magnitude})");
      }


    }

    public Vector2 Get(string paramName)
    {

      switch (paramName)
      {
        case "StartSpeed":
          return new Vector2(main.startSpeed.constantMin, main.startSpeed.constantMax);
        case "StartSize":
          return new Vector2(main.startSize.constantMin, main.startSize.constantMax);
        case "StartLifetime":
          return new Vector2(main.startLifetime.constantMin, main.startLifetime.constantMax);
        case "EmissionRate":
          return new Vector2(emit.rateOverTime.constantMin, emit.rateOverTime.constantMax);
        case "MaxParticles":
          return new Vector2(main.maxParticles, main.maxParticles);
        case "EmissionVolumeLength":
          return new Vector2(shape.length, shape.length);
        case "EmissionVolumeRadius":
          return new Vector2(shape.radius, shape.radius);
      }
      return Vector2.zero;
    }
    public void Set(string paramName, Vector2 paramValue)
    {
      ParticleSystem.MinMaxCurve newCurve = new ParticleSystem.MinMaxCurve(paramValue.x, paramValue.y);
      switch (paramName)
      {
        case "StartSpeed":
          main.startSpeed = newCurve;
          break;
        case "StartSize":
          main.startSize = newCurve;
          break;
        case "StartLifetime":
          main.startLifetime = newCurve;
          break;
        case "EmissionRate":
          emit.rateOverTime = newCurve;
          break;
        case "MaxParticles":
          main.maxParticles = (int)paramValue.x;
          break;
        case "EmissionVolumeLength":
          shape.length = paramValue.x;
          break;
        case "EmissionVolumeRadius":
          shape.radius = paramValue.x;
          break;
        default:
          break;

      }
    }

    //public void Set(
    //  Vector2 emissionRange,
    //  Vector2 speedRange,
    //  Vector2 sizeRange,
    //  Vector2 lifetimeRange,
    //  float fade
    //  )
    //{
    //  var main = emitter.main;
    //  main.startSpeed = new ParticleSystem.MinMaxCurve(speedRange.x, speedRange.y);
    //  main.startSize = new ParticleSystem.MinMaxCurve(sizeRange.x, sizeRange.y);
    //  main.startLifetime = new ParticleSystem.MinMaxCurve(lifetimeRange.x, lifetimeRange.y);

    //  var emit = emitter.emission;
    //  emit.rateOverTime = new ParticleSystem.MinMaxCurve(emissionRange.x, emissionRange.y);

    //  var color = emitter.colorOverLifetime;

    //  Gradient grad = color.color.gradient;
    //  grad.SetKeys(grad.colorKeys, new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(fade * 1f, 0.01f), new GradientAlphaKey(fade * 0.3f, 0.5f), new GradientAlphaKey(0f, 1f) });
    //  color.color = new ParticleSystem.MinMaxGradient(grad);

    //}
  }
}
