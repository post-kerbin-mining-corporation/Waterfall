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
    public static bool ShowEffectEditor = true;
    public static bool DebugModules = true;
    public static bool DebugSettings = true;
    public static bool DebugEffects = true;
    public static bool DebugModifiers = true;
    public static bool DebugMode = true;
    public static bool DebugUIMode = true;

    public static float AtmosphereDensityExponent = 0.5128f;

    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      ConfigNode settingsNode;

      Utils.Log("[Settings]: Started loading", LogType.Settings);
      if (GameDatabase.Instance.ExistsConfigNode("Waterfall/WATERFALL_SETTINGS"))
      {
        Utils.Log("[Settings]: Using specified settings", LogType.Settings);

        settingsNode = GameDatabase.Instance.GetConfigNode("Waterfall/WATERFALL_SETTINGS");

        // Setting parsing goes here
        //settingsNode.TryGetValue("MinimumWarpFactor", ref TimeWarpLimit);
        settingsNode.TryGetValue("ShowEffectEditor", ref ShowEffectEditor);
        settingsNode.TryGetValue("DebugModules", ref DebugModules);
        settingsNode.TryGetValue("DebugSettings", ref DebugSettings);
        settingsNode.TryGetValue("DebugEffects", ref DebugEffects);
        settingsNode.TryGetValue("DebugModifiers", ref DebugModifiers);
        settingsNode.TryGetValue("DebugMode", ref DebugMode);
        settingsNode.TryGetValue("DebugUIMode", ref DebugUIMode);
        settingsNode.TryGetValue("AtmosphereDensityExponent", ref AtmosphereDensityExponent);

      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults", LogType.Settings);
      }
      Utils.Log("[Settings]: Finished loading", LogType.Settings);
    }
  }
}
