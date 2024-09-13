﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Class for loading and retrieving particles from assetbundles
  /// </summary>
  public static class WaterfallParticleLoader
  {
    /// <summary>
    ///   A collection of all particle assets loaded by Waterfall
    /// </summary>
    private static readonly Dictionary<string, GameObject> ParticleDictionary = new();
    /// <summary>
    ///   A collection of all editable particle properies
    /// </summary>
    private static readonly Dictionary<string, ParticleData> ParticlePropertyMap = new();

    /// <summary>
    ///   Requests a particle by name
    /// </summary>
    /// <param name="shaderName"></param>
    /// <returns></returns>
    public static GameObject GetParticles(string particleName)
    {
      Utils.Log("[WaterfallParticleLoader]: Getting particle " + particleName, LogType.Effects);
      return ParticleDictionary.ContainsKey(particleName) ? ParticleDictionary[particleName] : null;
    }

    public static Dictionary<string, ParticleData> GetParticlePropertyMap() => ParticlePropertyMap;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllParticlesNames() => ParticleDictionary.Keys.ToList();

    /// <summary>
    ///   Loads all particles in the plugin's data directory
    /// </summary>
    public static void LoadParticles()
    {
      Utils.Log("[Particles]: Loading particle systems", LogType.Loading);
      string pathSpec = "*.particle";
      string path = Path.Combine(KSPUtil.ApplicationRootPath);

      List<string> bundlePaths = Directory.GetFiles(path, pathSpec, SearchOption.AllDirectories).ToList();
      List<string> orderedBundles = bundlePaths.OrderBy(x => Path.GetFileNameWithoutExtension(x)).ToList();

      foreach (string bundle in bundlePaths)
      {
        LoadAssetBundleAtPath(bundle);
      }
    }

    /// <summary>
    ///   Manually load Particle Asset bundles by path
    /// </summary>
    public static void LoadAssetBundleAtPath(string bundlePath)
    {
      Utils.Log($"[Particles]: Loading {Path.GetFileNameWithoutExtension(bundlePath)}", LogType.Loading);
      var bundle = AssetBundle.LoadFromFile(bundlePath);
      var systems = bundle.LoadAllAssets<GameObject>();

      foreach (var sys in systems)
      {
        Utils.Log($"[Particles]: Adding {sys.name}", LogType.Loading);
        if (!ParticleDictionary.ContainsKey(sys.name))
        {
          ParticleDictionary.Add(sys.name, sys);
        }
        else
        {
          Utils.LogWarning($"[Particles]: A particle with {sys.name} already exists");
        }
      }

      Utils.Log($"[Particles]: Loaded {ParticleDictionary.Count} particle assets", LogType.Loading);
    }
    public static List<string> FindValidParticleProperties(WaterfallParticlePropertyType propType)
    {
      var validProps = new List<string>();
      foreach (var mProp in ParticlePropertyMap)
      {
        if (mProp.Value.type == propType)
        {
          validProps.Add(mProp.Key);
        }
      }
      return validProps;
    }
    public static void LoadParticleProperties()
    {
      foreach (var node in GameDatabase.Instance.GetConfigNodes(WaterfallConstants.ParticlePropertyNodeName))
      {
        try
        {
          string propertyName = node.GetValue("name");
          var range = Vector2.zero;

          node.TryGetValue("range", ref range);
          var t = WaterfallParticlePropertyType.Range;
          node.TryGetEnum("type", ref t, WaterfallParticlePropertyType.Range);
          var m = new ParticleData(t, range);

          Utils.Log($"[Particles]: Adding {propertyName} property", LogType.Loading);
          ParticlePropertyMap.Add(propertyName, m);
        }
        catch
        {
          Utils.LogError($"[Particles] Issue loading particle property from node: {node}");
        }
      }
    }
  }
}