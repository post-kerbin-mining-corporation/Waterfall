using System.Collections.Generic;
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
      Utils.Log("[WaterfallParticleLoader]: Getting particle " + particleName);
      return ParticleDictionary.ContainsKey(particleName) ? ParticleDictionary[particleName] : null;
    }


    public static Dictionary<string, ParticleData> GetParticlePropertyMap() => ParticlePropertyMap;


    /// <summary>
    ///   Requests a shader by name
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllParticlesNames() => ParticleDictionary.Keys.ToList();

    /// <summary>
    ///   Loads all particles in the plugin's data directory
    /// </summary>
    public static void LoadParticles()
    {
      Utils.Log("[WaterfallParticleLoader]: Loading shaders");
      string path = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/Waterfall/Particles/");
      string pathSpec;

      pathSpec = "*.particle";


      string[] bundlePaths = Directory.GetFiles(path, pathSpec);

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
      Utils.Log($"[WaterfallParticleLoader]: Loading {Path.GetFileNameWithoutExtension(bundlePath)}");
      var bundle = AssetBundle.LoadFromFile(bundlePath);
      var systems = bundle.LoadAllAssets<GameObject>();

      foreach (var sys in systems)
      {
        Utils.Log($"[WaterfallParticleLoader]: Adding {sys.name}");
        if (!ParticleDictionary.ContainsKey(sys.name))
          ParticleDictionary.Add(sys.name, sys);
        else
          Utils.LogWarning($"[WaterfallParticleLoader]: A particle with {sys.name} already exists");
      }

      //bundle.Unload(false); // unload the raw asset bundle
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
          Utils.Log($"[WaterfallParticleLoader]: Adding {propertyName} property");
          ParticlePropertyMap.Add(propertyName, m);
        }
        catch
        {
          Utils.LogError($"[WaterfallParticleLoader] Issue loading particle param from node: {node}");
        }
      }
    }
  }
}