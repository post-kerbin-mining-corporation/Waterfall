using System.Collections.Generic;
using UnityEngine;

namespace Waterfall.Modules
{
  public class ModuleWaterfallSmoke : PartModule
  {
    // This links to an EngineID from a ModuleEnginesFX
    [KSPField(isPersistant = false)] public string waterfallModuleID = "";

    // This links to an EngineID from a ModuleEnginesFX
    [KSPField(isPersistant = false)] public string smokeTransformName = "";

    // This links to an EngineID from a ModuleEnginesFX
    [KSPField(isPersistant = false)] public string smokePrefabName = "WaterfallSmokeProto";

    // Map speed to emission
    [KSPField(isPersistant = false)] public FloatCurve SpeedEmissionScaleCurve = new();

    [KSPField(isPersistant = false)] public FloatCurve SpeedLifetimeScaleCurve = new();

    [KSPField(isPersistant = false)] public FloatCurve AtmoSizeScaleCurve = new();

    [KSPField(isPersistant = false)] public FloatCurve AtmoAlphaFadeCurve = new();

    [KSPField(isPersistant = false)] public Vector2 emissionRateRange;

    [KSPField(isPersistant = false)] public Vector2 emissionSpeedRange;

    [KSPField(isPersistant = false)] public Vector2 startSizeRange;

    [KSPField(isPersistant = false)] public Vector2 lifetimeRange;

    private List<WaterfallSmokeEmitter> emitters;

    public void Start()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        InstantiateEffect();
      }
    }

    public void FixedUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight && emitters != null)
      {
        SetParticles();
        for (int i = 0; i < emitters.Count; i++)
        {
          emitters[i].Update();
        }
      }
    }

    public void InstantiateEffect()
    {
      emitters = new();
      if (smokeTransformName != "")
      {
        foreach (var t in part.FindModelTransforms(smokeTransformName))
        {
          emitters.Add(new(smokePrefabName, t));
        }
      }
      else
      {
        emitters.Add(new(smokePrefabName, part.transform));
      }
    }

    public void SetRanges(
      Vector2 emissionRangeNew,
      Vector2 speedRangeNew,
      Vector2 sizeRangeNew,
      Vector2 lifetimeRangeNew)
    {
      emissionRateRange  = emissionRangeNew;
      emissionSpeedRange = speedRangeNew;
      startSizeRange     = sizeRangeNew;
      lifetimeRange      = lifetimeRangeNew;
    }

    protected void SetParticles()
    {
      float srfSpeed    = (float)part.vessel.srfSpeed;
      float pressureAtm = (float)vessel.mainBody.GetPressureAtm(part.vessel.altitude);
      for (int i = 0; i < emitters.Count; i++)
      {
        emitters[i]
          .Set(emissionRateRange * SpeedEmissionScaleCurve.Evaluate(srfSpeed),
               emissionSpeedRange,
               startSizeRange * AtmoSizeScaleCurve.Evaluate(pressureAtm),
               lifetimeRange  * SpeedLifetimeScaleCurve.Evaluate(srfSpeed),
               AtmoAlphaFadeCurve.Evaluate(pressureAtm));
      }
    }
  }

  public class WaterfallSmokeEmitter
  {
    private static   ParticleSystem.Particle[] particles;
    private readonly ParticleSystem            emitter;
    private          string                    prefab;
    private          Transform                 parent;
    private          ParticleSystemRenderer    renderer;

    public WaterfallSmokeEmitter(string prefabName, Transform parentTransform)
    {
      prefab = prefabName;
      parent = parentTransform;
      var go = Object.Instantiate(WaterfallParticleLoader.GetParticles(prefabName), Vector3.zero, Quaternion.identity);

      emitter  = go.GetComponent<ParticleSystem>();
      renderer = go.GetComponent<ParticleSystemRenderer>();
      go.transform.SetParent(parentTransform);
      go.transform.localPosition = Vector3.zero;
      go.transform.localScale    = Vector3.one;
      go.transform.localRotation = Quaternion.identity;

      FloatingOrigin.RegisterParticleSystem(emitter);
    }

    public void Update()
    {
      //if (particles == null || emitter.main.maxParticles > particles.Length)
      //  particles = new ParticleSystem.Particle[emitter.main.maxParticles];

      //int numParticlesAlive = emitter.GetParticles(particles);

      //for (int j = 0; j < numParticlesAlive; j++)
      //{
      //  //particles[j].v Krakensbane.GetFrameVelocity
      //}
    }

    public void Set(
      Vector2 emissionRange,
      Vector2 speedRange,
      Vector2 sizeRange,
      Vector2 lifetimeRange,
      float   fade)
    {
      var main = emitter.main;
      main.startSpeed    = new(speedRange.x, speedRange.y);
      main.startSize     = new(sizeRange.x, sizeRange.y);
      main.startLifetime = new(lifetimeRange.x, lifetimeRange.y);

      var emit = emitter.emission;
      emit.rateOverTime = new(emissionRange.x, emissionRange.y);

      var color = emitter.colorOverLifetime;

      var grad = color.color.gradient;
      grad.SetKeys(grad.colorKeys, new[] { new(0f, 0f), new GradientAlphaKey(fade * 1f, 0.01f), new GradientAlphaKey(fade * 0.3f, 0.5f), new GradientAlphaKey(0f, 1f) });
      color.color = new(grad);
    }
  }
}