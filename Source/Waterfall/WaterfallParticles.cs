using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Class for loading and retrieving shaders
  /// </summary>
  public static class WaterfallParticleLoader
  {
    /// <summary>
    ///   A collection of all shaders loaded by Waterfall
    /// </summary>
    private static readonly Dictionary<string, GameObject> ParticleDictionary = new();

    /// <summary>
    ///   Requests a shader by name
    /// </summary>
    /// <param name="shaderName"></param>
    /// <returns></returns>
    public static GameObject GetParticles(string particleName)
    {
      Utils.Log("[WaterfallParticleLoader]: Getting shader " + particleName);
      return ParticleDictionary.ContainsKey(particleName) ? ParticleDictionary[particleName] : null;
    }

    /// <summary>
    ///   Requests a shader by name
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllShadersNames() => ParticleDictionary.Keys.ToList();

    /// <summary>
    ///   Loads all shaders in the plugin's data directory
    /// </summary>
    public static void LoadParticles()
    {
      Utils.Log("[WaterfallParticleLoader]: Loading shaders");
      string path = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/Waterfall/Particles/");
      string pathSpec;

      pathSpec = "*.particle"; // fixes OpenGL on windows


      string[] bundlePaths = Directory.GetFiles(path, pathSpec);

      foreach (string bundle in bundlePaths)
      {
        LoadAssetBundleAtPath(bundle);
      }
    }

    /// <summary>
    ///   Manually load Shader Asset bundles by path
    /// </summary>
    public static void LoadAssetBundleAtPath(string bundlePath)
    {
      Utils.Log($"[WaterfallParticleLoader]: Loading {Path.GetFileNameWithoutExtension(bundlePath)}");
      var bundle  = AssetBundle.LoadFromFile(bundlePath);
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
  }
}