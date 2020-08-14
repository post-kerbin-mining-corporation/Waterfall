using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace Waterfall
{
  [KSPAddon(KSPAddon.Startup.EveryScene, false)]
  public class Waterfall : MonoBehaviour
  {
    public static Waterfall Instance { get; private set; }

    protected void Awake()
    {
      Instance = this;
    }
    protected void Start()
    {
      
      Settings.Load();
      WaterfallAssets.Load();
    }
  }

  /// <summary>
  /// Static class to hold settings and configuration
  /// </summary>
  public static class Settings
  {
    /// Settings go here
    public static bool DebugModules = true;
    public static bool DebugSettings = true;
    public static bool DebugEffects = true;
    public static bool DebugModifiers = true;
    public static bool DebugMode = true;
    public static bool DebugUIMode = true;

    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      ConfigNode settingsNode;

      Utils.Log("[Settings]: Started loading");
      if (GameDatabase.Instance.ExistsConfigNode("Waterfall/WATERFALL_SETTINGS"))
      {
        Utils.Log("[Settings]: Located settings file");

        settingsNode = GameDatabase.Instance.GetConfigNode("Waterfall/WATERFALL_SETTINGS");

        // Setting parsing goes here
        //settingsNode.TryGetValue("MinimumWarpFactor", ref TimeWarpLimit);


      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults");
      }
      Utils.Log("[Settings]: Finished loading");
    }
  }
}
