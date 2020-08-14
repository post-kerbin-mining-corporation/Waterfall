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
    public static Dictionary<string, string> Models;
    public static Dictionary<string, string> Textures;
    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      Models = new Dictionary<string, string>();
      Textures = new Dictionary<string, string>();
      Utils.Log("[Asset Library]: Started loading");
      Utils.Log("[Asset Library]: Loading models");
      foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("WATERFALL_MODEL"))
      {
        string path = "";
        string description = "";
        node.TryGetValue("path", ref path);
        node.TryGetValue("description", ref description);

        Models.Add(path, description);
      }
      Utils.Log($"[Asset Library]: Loaded {Models.Count} models");
      Utils.Log("[Asset Library]: Loading textures");
      foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("WATERFALL_TEXTURE"))
      {
        string path = "";
        string description = "";
        node.TryGetValue("path", ref path);
        node.TryGetValue("description", ref description);

        Textures.Add(path, description);
      }
      Utils.Log($"[Asset Library]: Loaded {Textures.Count} textures");
      Utils.Log("[Asset Library]: Finished loading");
    }
  }
}
