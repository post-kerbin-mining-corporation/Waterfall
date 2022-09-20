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

    public List<WaterfallParticleProperty> pProperties;

    public List<WaterfallParticleEmitter> systems;

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
      node.TryGetValue("assetName", ref assetName);

      Utils.Log(String.Format($"[WaterfallParticle]: Loading new particle {assetName} for {transformName}"), LogType.Effects);

      pProperties = new();
      foreach (var subnode in node.GetNodes(WaterfallConstants.RangeNodeName))
      {
        pProperties.Add(new WaterfallParticleRangeProperty(subnode));
      }
      foreach (var subnode in node.GetNodes(WaterfallConstants.FloatNodeName))
      {
        pProperties.Add(new WaterfallParticleFloatProperty(subnode));
      }
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.ParticleNodeName;
      node.AddValue("transform", transformName);
      node.AddValue("assetName", assetName);

      foreach (var p in pProperties)
      {
        node.AddNode(p.Save());
      }

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

      foreach (var p in pProperties)
      {
        foreach(var sys in systems)
        {
          p.Initialize(sys.emitter);
        }
        
      }

      Utils.Log($"[WaterfallParticle]: Initialized Waterfall Particle at {parentTransform}, {systems.Count} Count", LogType.Effects);
    }

    public void SetParticleValue(string propertyName, Vector2 value)
    {
      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleRangeProperty t && prop != null)
        t.propertyValue = value;
      else
      {
        var newProp = new WaterfallParticleRangeProperty
        {
          propertyName = propertyName,
          propertyValue = value
        };
        pProperties.Add(newProp);
      }


      foreach ( var p in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, p.emitter, value);
      }
    }
    public void SetParticleValue(string propertyName, float value)
    {

      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleFloatProperty t && prop != null)
        t.propertyValue = value;
      else
      {
        var newProp = new WaterfallParticleFloatProperty
        {
          propertyName = propertyName,
          propertyValue = value
        };
        pProperties.Add(newProp);
      }


      foreach (var p in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, p.emitter, value);
      }
    }

    public void SetParticleValue(string propertyName, Color value)
    {
      var prop = pProperties.Find(x => x.propertyName == propertyName);
      if (prop is WaterfallParticleColorProperty t && prop != null)
        t.propertyValue = value;
      else
      {
        var newProp = new WaterfallParticleColorProperty
        {
          propertyName = propertyName,
          propertyValue = value
        };
        pProperties.Add(newProp);
      }


      foreach (var p in systems)
      {
        ParticleUtils.SetParticleSystemValue(propertyName, p.emitter, value);
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

          Utils.Log($"frame vel at {frameVel.magnitude}: \n - normVel {nrmVelocity}\n - moving particle by {(-frameVel * TimeWarp.deltaTime) - UnityEngine.Random.Range(0f, distancePerFrame) * nrmVelocity}");
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

    public  void Get(string paramName, out Vector2 result)
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
