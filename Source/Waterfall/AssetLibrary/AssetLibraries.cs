using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace Waterfall
{
  
  /// <summary>
  /// Static class to hold settings and configuration
  /// </summary>
  public static class WaterfallAssets
  {

    public static List<WaterfallAsset> Models;
    public static List<WaterfallAsset> Textures;
    public static List<WaterfallAsset> Shaders;
    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      Models = new List<WaterfallAsset>();
      Textures = new List<WaterfallAsset>();
      Shaders = new List<WaterfallAsset>();
      Utils.Log("[Asset Library]: Started loading");
      Utils.Log("[Asset Library]: Loading models");
      foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("WATERFALL_MODEL"))
      {
        try
        {
          Models.Add(new WaterfallAsset(node));
        }
        catch
        {
          Utils.LogError($"[Asset Libary] Issue loading model from node: {node}");
        }
      }
      Utils.Log($"[Asset Library]: Loaded {Models.Count} models");
      Utils.Log("[Asset Library]: Loading textures");
      foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("WATERFALL_TEXTURE"))
      {
        try
        {
          Textures.Add(new WaterfallAsset(node));
        }
        catch
        {
          Utils.LogError($"[Asset Libary] Issue loading model from node: {node}");
        }
      }
      Utils.Log($"[Asset Library]: Loaded {Textures.Count} textures");

      Utils.Log("[Asset Library]: Loading shaders");
      foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("WATERFALL_SHADER"))
      {
        try
        {
          Shaders.Add(new WaterfallAsset(node));
        }
        catch
        {
          Utils.LogError($"[Asset Libary] Issue loading model from node: {node}");
        }
      }
      Utils.Log($"[Asset Library]: Loaded {Shaders.Count} shaders");
      Utils.Log("[Asset Library]: Finished loading");
    }

    public static List<WaterfallAsset> GetModels(AssetWorkflow flow)
    {
      return Models.FindAll(x => x.Workflow == flow);
    }
    public static List<WaterfallAsset> GetShaders(AssetWorkflow flow)
    {
      return Shaders.FindAll(x => x.Workflow == flow);
    }
  }
}
