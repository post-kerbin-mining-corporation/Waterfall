using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using System.IO;
namespace Waterfall
{
  /// <summary>
  /// Class for loading and retrieving shaders
  /// </summary>
  public static class WaterfallParticleLoader
  {
    /// <summary>
    /// A collection of all shaders loaded by Waterfall
    /// </summary>
    private static readonly Dictionary<String, GameObject> ParticleDictionary = new Dictionary<String, GameObject>();

    /// <summary>
    /// Requests a shader by name
    /// </summary>
    /// <param name="shaderName"></param>
    /// <returns></returns>
    public static GameObject GetParticles(String particleName)
    {
      Utils.Log("[WaterfallParticleLoader]: Getting shader " + particleName);
      return ParticleDictionary.ContainsKey(particleName) ? ParticleDictionary[particleName] : null;
    }

    /// <summary>
    /// Requests a shader by name
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllShadersNames()
    {
      return ParticleDictionary.Keys.ToList();
    }

    /// <summary>
    /// Loads all shaders in the plugin's data directory
    /// </summary>
    public static void LoadParticles()
    {
      Utils.Log("[WaterfallParticleLoader]: Loading shaders");
      String path = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/Waterfall/Particles/");
      String pathSpec;
      
      pathSpec = "*.particle"; // fixes OpenGL on windows
      

      String[] bundlePaths = Directory.GetFiles(path, pathSpec);

      foreach (String bundle in bundlePaths)
      {
        WaterfallParticleLoader.LoadAssetBundleAtPath(bundle);
      }
    }
    /// <summary>
    /// Manually load Shader Asset bundles by path
    /// </summary>
    public static void LoadAssetBundleAtPath(String bundlePath)
    {

      Utils.Log($"[WaterfallParticleLoader]: Loading {Path.GetFileNameWithoutExtension(bundlePath)}");
      AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
      GameObject[] systems = bundle.LoadAllAssets<GameObject>();

      foreach (GameObject sys in systems)
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
