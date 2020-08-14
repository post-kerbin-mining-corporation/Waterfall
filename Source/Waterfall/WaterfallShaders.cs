using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace Waterfall
{
  /// <summary>
  /// Class for loading and retrieving shaders
  /// </summary>
  public static class ShaderLoader
  {
    /// <summary>
    /// A collection of all shaders loaded by Waterfall
    /// </summary>
    private static readonly Dictionary<String, Shader> ShaderDictionary = new Dictionary<String, Shader>();

    /// <summary>
    /// Requests a shader by name
    /// </summary>
    /// <param name="shaderName"></param>
    /// <returns></returns>
    public static Shader GetShader(String shaderName)
    {
      Utils.Log("[ShaderLoader]: Getting shader " + shaderName);
      return ShaderDictionary.ContainsKey(shaderName) ? ShaderDictionary[shaderName] : null;
    }

    /// <summary>
    /// Requests a shader by name
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllShadersNames()
    {
      return ShaderDictionary.Keys.ToList();
    }

    /// <summary>
    /// Loads all shaders in the plugin's data directory
    /// </summary>
    public static void LoadShaders()
    {
      Utils.Log("[ShaderLoader]: Loading shaders");
      String path = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/Waterfall/Shaders/");
      String pathSpec;
      if (Application.platform == RuntimePlatform.WindowsPlayer &&
         SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
      {
        pathSpec = "*-linux.waterfall"; // fixes OpenGL on windows
      }
      else if (Application.platform == RuntimePlatform.WindowsPlayer)
      {
        pathSpec = "*-windows.waterfall"; 
      }
      else if (Application.platform == RuntimePlatform.LinuxPlayer)
      {
        pathSpec = "*-linux.waterfall"; 
      }
      else
      {
        pathSpec = "*-macos.waterfall"; 
      }
   
      String[] bundlePaths = Directory.GetFiles(path, pathSpec);

      foreach (String bundle in bundlePaths)
      {
        ShaderLoader.LoadAssetBundleAtPath(bundle);
      }
    }
    /// <summary>
    /// Manually load Shader Asset bundles by path
    /// </summary>
    public static void LoadAssetBundleAtPath(String bundlePath)
    {

      Utils.Log($"[ShaderLoader]: Loading {Path.GetFileNameWithoutExtension(bundlePath)}");
      AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
      Shader[] shaders = bundle.LoadAllAssets<Shader>();

      foreach (Shader shader in shaders)
      {
        Utils.Log($"[ShaderLoader]: Adding {shader.name}");
        if (!ShaderDictionary.ContainsKey(shader.name))
          ShaderDictionary.Add(shader.name, shader);
        else
          Utils.LogWarning($"[ShaderLoader]: A shader with {shader.name} already exists");
      }

      bundle.Unload(false); // unload the raw asset bundle
    }
  }
}
