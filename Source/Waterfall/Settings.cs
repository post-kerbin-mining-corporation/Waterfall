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
    public static float MinimumEffectIntensity = 0.005f;
    public static float MinimumLightIntensity = 0.05f;
    public static bool EnableLights = true;
    public static bool EnableDistortion = true;

    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      ConfigNode settingsNode = GameDatabase.Instance.GetConfigNode("Waterfall/WATERFALL_SETTINGS"); ;

      Utils.Log("[Settings]: Started loading", LogType.Settings);
      if (settingsNode != null)
      {
        Utils.Log("[Settings]: Using specified settings", LogType.Settings);
        // Setting parsing goes here
        
        settingsNode.TryGetValue("ShowEffectEditor", ref ShowEffectEditor);
        settingsNode.TryGetValue("DebugModules", ref DebugModules);
        settingsNode.TryGetValue("DebugSettings", ref DebugSettings);
        settingsNode.TryGetValue("DebugEffects", ref DebugEffects);
        settingsNode.TryGetValue("DebugModifiers", ref DebugModifiers);
        settingsNode.TryGetValue("DebugMode", ref DebugMode);
        settingsNode.TryGetValue("DebugUIMode", ref DebugUIMode);
        settingsNode.TryGetValue("AtmosphereDensityExponent", ref AtmosphereDensityExponent);
        settingsNode.TryGetValue("MinimumEffectIntensity", ref MinimumEffectIntensity);
        settingsNode.TryGetValue("MinimumLightIntensity", ref MinimumLightIntensity);
        settingsNode.TryGetValue("EnableLights", ref EnableLights);
        settingsNode.TryGetValue("EnableDistortion", ref EnableDistortion);
      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults", LogType.Settings);
      }
      Utils.Log("[Settings]: Finished loading", LogType.Settings);
    }
  }
}
