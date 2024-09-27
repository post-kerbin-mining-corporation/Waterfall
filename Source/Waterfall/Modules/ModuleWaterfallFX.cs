﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using Waterfall.EffectControllers;

namespace Waterfall
{
  public enum Version
  {
    Initial,
    FixedRampRates,
  }

  public class ModuleWaterfallFX : PartModule
  {
    public static readonly Version CurrentVersion = Enum.GetValues(typeof(Version)).Cast<Version>().Max();

    // This identifies this FX module for reference elsewhere
    [KSPField(isPersistant = false)] public string moduleID = "";

    // This links to an EngineID from a ModuleEnginesFX
    [KSPField(isPersistant = false)] public string engineID = "";

    [KSPField(isPersistant = false)] public bool useRelativeScaling;

    [KSPField(isPersistant = false)] public Version version = CurrentVersion;

    protected readonly Dictionary<string, WaterfallController> allControllers = new(16);
    protected readonly List<WaterfallEffect> allFX = new(16);
    protected readonly List<WaterfallEffect> activeFX = new(16);
    protected readonly List<WaterfallEffectTemplate> allTemplates = new(16);
    protected readonly List<Renderer> allRenderers = new(128);
    protected bool refreshRenderers = true;

    protected bool started;
    private bool isHDR;
    protected ConfigNode config;
    protected ConfigNode[] effectsNodes = { };
    protected ConfigNode[] templatesNodes = { };

    public List<WaterfallEffect> FX => allFX;

    public List<WaterfallEffectTemplate> Templates => allTemplates;

    public List<WaterfallController> Controllers => allControllers.Values.ToList();
    public Dictionary<string, WaterfallController> AllControllersDict => allControllers;

    bool isAwake;

    /// <summary>
    /// Sets the value of a specific controller
    /// Interface method. Used by other mods to push values to controllers
    /// </summary>
    /// <param name="controllerID"></param>
    /// <param name="value"></param>
    public void SetControllerValue(string controllerID, float value)
    {
      allControllers[controllerID].Set(value);
    }

    public override void OnAwake()
    {
      base.OnAwake();
    }

    public void Start()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        Initialize();
        started = true;
      }
    }

    private void GatherRenderers()
    {
      if (refreshRenderers)
      {
        allRenderers.Clear();
        foreach (var fx in activeFX)
        {
          foreach (var renderer in fx.effectRenderers)
          {
            Material mat = renderer.material;

            // distortion effects get a constant renderqueue value, so they don't need to be sorted
            if (mat.HasProperty(ShaderPropertyID._Strength))
            {
              int qDelta = Settings.DistortQueue;
              if (mat.HasProperty(ShaderPropertyID._Intensity))
                qDelta += 1;
              mat.renderQueue = Settings.TransparentQueueBase + qDelta;
            }
            else
            {
              allRenderers.AddUnique(renderer);
            }
          }
        }

        refreshRenderers = false;
      }
    }

    private static readonly ProfilerMarker luSetup = new ProfilerMarker("Waterfall.LateUpdate.Setup");
    private static readonly ProfilerMarker luControllers = new ProfilerMarker("Waterfall.LateUpdate.Controllers");
    private static readonly ProfilerMarker luEffects = new ProfilerMarker("Waterfall.LateUpdate.Effects");

    protected void LateUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        luSetup.Begin();
        GatherRenderers();
        var camera = FlightCamera.fetch.cameras[0];
        bool changeHDR = camera.allowHDR != isHDR;
        if (changeHDR)
          isHDR = !isHDR;
        luSetup.End();
        luControllers.Begin();
        bool controllersAwake = false;
        foreach (var controller in allControllers.Values)
        {
          controllersAwake = controller.Update() || controllersAwake;
        }
        luControllers.End();

        isAwake = isAwake || controllersAwake;

        if (isAwake)
        {
          bool effectsAwake = false;
          luEffects.Begin();
          for (int fxIndex = 0; fxIndex < activeFX.Count; ++fxIndex)
          {
            var fx = activeFX[fxIndex];
            effectsAwake = fx.Update() || effectsAwake;
            if (changeHDR)
              fx.SetHDR(isHDR);
          }

          if (!controllersAwake && !effectsAwake)
          {
            isAwake = false;
          }

          luEffects.End();
          WaterfallEffect.SetupRenderersForCamera(camera, allRenderers);
        }
      }
    }

    private void FetchConfigFromPrefab()
    {
      config = part.partInfo.partPrefab.FindModulesImplementing<ModuleWaterfallFX>().FirstOrDefault(x => x.moduleID == moduleID).config;
      effectsNodes = config.GetNodes(WaterfallConstants.EffectNodeName);
      templatesNodes = config.GetNodes(WaterfallConstants.TemplateNodeName);
    }

    /// <summary>
    ///   Load all CONTROLLERS, TEMPLATES and EFFECTS
    /// </summary>
    /// <param name="node"></param>
    public override void OnLoad(ConfigNode node)
    {
      base.OnLoad(node);
      Utils.Log($"[ModuleWaterfallFX]: OnLoad called with contents \n{node}", LogType.Modules);

      if (HighLogic.LoadedScene == GameScenes.LOADING)  // Store the node for later, nothing else to do now
      {
        config = node.CreateCopy();
        return;
      }
      else if (HighLogic.LoadedSceneIsEditor) return; // Nothing to do in the Editor

      // KSP behaviour is to only provide the Persistent data, everything else is in the prefab.
      if (!node.HasNode(WaterfallConstants.EffectNodeName) && !node.HasNode(WaterfallConstants.TemplateNodeName))
      {
        FetchConfigFromPrefab();
      }
      else
      {
        config = config.CreateCopy();
        if (node.HasNode(WaterfallConstants.EffectNodeName))
          effectsNodes = node.GetNodes(WaterfallConstants.EffectNodeName);
        if (node.HasNode(WaterfallConstants.TemplateNodeName))
          templatesNodes = node.GetNodes(WaterfallConstants.TemplateNodeName);
        config.RemoveNodes(WaterfallConstants.EffectNodeName);
        config.RemoveNodes(WaterfallConstants.TemplateNodeName);
        foreach (var n in effectsNodes)
          config.AddNode(n);
        foreach (var n in templatesNodes)
          config.AddNode(n);
      }

      // Wait for OnStart to initialize, if it hasn't already run. (B9PS can call after OnStart())
      if (started)
        Initialize();
    }

    /// <summary>
    ///   Dumps the entire set of EFFECTS to a confignode, then to a string
    /// </summary>
    /// <returns></returns>
    public string ExportModule()
    {
      var newNode = new ConfigNode("MODULE");
      newNode.AddValue("name", "ModuleWaterfallFX");
      newNode.AddValue("moduleID", moduleID);
      newNode.AddValue("version", version);
      //newNode.AddValue("engineID", engineID);

      foreach (var ctrl in Controllers)
      {
        newNode.AddNode(ctrl.Save());
      }

      if (Templates.Count > 0)
      {
        foreach (var templ in Templates)
        {
          newNode.AddNode(templ.Save());
        }
      }
      else
      {
        foreach (var fx in allFX)
        {
          newNode.AddNode(fx.Save());
        }
      }

      return newNode.ToString();
    }

    public string ExportControllers()
    {
      var toRet = StringBuilderCache.Acquire();
      foreach (var ctrl in Controllers)
      {
        toRet.AppendLine($"{ctrl.Save()}");
      }

      return toRet.ToStringAndRelease();
    }

    /// <summary>
    ///   Dumps the entire set of EFFECTS to a confignode, then to a string
    /// </summary>
    /// <returns></returns>
    public string ExportEffects()
    {
      var toRet = StringBuilderCache.Acquire();
      foreach (var fx in allFX)
      {
        toRet.AppendLine($"{fx.Save()}");
      }

      return toRet.ToStringAndRelease();
    }

    private void LoadEffects(ConfigNode node)
    {
      Utils.Log($"[ModuleWaterfallFX]: Loading Effects on moduleID {moduleID}", LogType.Modules);
      var effectNodes = node.GetNodes(WaterfallConstants.EffectNodeName);
      var templateNodes = node.GetNodes(WaterfallConstants.TemplateNodeName);

      foreach (var fxDataNode in effectNodes)
      {
        allFX.Add(new(fxDataNode));
      }

      Utils.Log($"[ModuleWaterfallFX]: Loading Template effects on moduleID {moduleID}", LogType.Modules);
      foreach (var templateNode in templateNodes)
      {
        var template = new WaterfallEffectTemplate(templateNode);
        allTemplates.Add(template);
        foreach (var fx in template.allFX)
        {
          allFX.Add(fx);
        }

        Utils.Log($"[ModuleWaterfallFX]: Loaded effect template {template.templateName}", LogType.Modules);
      }

      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allTemplates.Count} templates", LogType.Modules);
      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allFX.Count} effects", LogType.Modules);
    }

    /// <summary>
    ///   Determine controller type from config node in the old format (config node named <see cref="WaterfallConstants.LegacyControllerNodeName" /> with <see cref="WaterfallController.LegacyControllerTypeNodeName" /> property).
    ///   If value of config node entry named <see cref="WaterfallController.LegacyControllerTypeNodeName" /> is missing or incorrect, default to <see cref="ThrottleController" /> type.
    /// </summary>
    /// <remarks>
    ///   <see href="https://github.com/post-kerbin-mining-corporation/Waterfall/pull/95" /> for info regarding change to new format.
    /// </remarks>
    private EffectControllerInfo DetermineControllerTypeFromLegacyConfigNode(ConfigNode configNode)
    {
      var controllerType = EffectControllersMetadata.ControllersByType[typeof(ThrottleController)];

      string linkedToValue = null;
      if (configNode.TryGetValue(WaterfallController.LegacyControllerTypeNodeName, ref linkedToValue))
      {
        if (EffectControllersMetadata.ControllersByLegacyControllerTypeIds.TryGetValue(linkedToValue, out var legacyControllerType))
        {
          controllerType = legacyControllerType;
        }
        else
        {
          Utils.LogWarning($"[ModuleWaterfallFX]: Unknown {WaterfallController.LegacyControllerTypeNodeName} value {linkedToValue} on moduleID {moduleID}, setting throttle as default");
        }
      }
      else
      {
        Utils.LogWarning($"[ModuleWaterfallFX]: Controller on moduleID {moduleID} does not define {WaterfallController.LegacyControllerTypeNodeName} field, setting throttle as default");
      }

      return controllerType;
    }

    private void LoadControllers(ConfigNode node)
    {
      Utils.Log($"[ModuleWaterfallFX]: Loading effect controllers on moduleID {moduleID}", LogType.Modules);

      allControllers.Clear();
      foreach (var childNode in node.GetNodes())
      {
        EffectControllerInfo controllerType;

        // Check if node is either called CONTROLLER (old format) or something like THROTTLECONTROLLER (new format)
        if (childNode.name == WaterfallConstants.LegacyControllerNodeName)
        {
          controllerType = DetermineControllerTypeFromLegacyConfigNode(childNode);
        }
        else if (EffectControllersMetadata.ControllersByConfigNodeName.TryGetValue(childNode.name, out var info))
        {
          controllerType = info;
        }
        else
        {
          continue;
        }

        var controller = controllerType.CreateFromConfig(childNode);
        Utils.Log($"[ModuleWaterfallFX]: Loaded effect controller of type {controller} named {controller.name} on moduleID {moduleID}, adding to loaded controllers dictionary", LogType.Modules);

        try
        {
          allControllers.Add(controller.name, controller);
        }
        catch (Exception ex)
        {
          Utils.LogError($"[ModuleWaterfallFX]: unable to add controller {controller} named {controller.name} to controllers dictionary on moduleID {moduleID}: {ex.Message}");
        }
      }

      Utils.Log($"[ModuleWaterfallFX]: Finished loading effect controllers on moduleID {moduleID}", LogType.Modules);
    }

    // VAB Inforstrings are blank
    public string GetModuleTitle() => "";

    public override string GetInfo() => "";
    /// <summary>
    ///   Gets the list of controllers
    /// </summary>
    /// <param name="controllerName"></param>
    /// <returns></returns>
    public List<string> GetControllerNames() => allControllers.Keys.ToList();

    public void AddController(WaterfallController newController)
    {
      Utils.Log("[ModuleWaterfallFX]: Added new controller", LogType.Modules);
      allControllers.Add(newController.name, newController);
      newController.Initialize(this);
      InitializeEffects();
    }

    public void RemoveController(WaterfallController toRemove)
    {
      Utils.Log("[ModuleWaterfallFX]: Deleting controller", LogType.Modules);
      allControllers.Remove(toRemove.name);
      refreshRenderers = true;
    }

    public void AddEffect(WaterfallEffect newEffect)
    {
      Utils.Log("[ModuleWaterfallFX]: Added new effect", LogType.Modules);
      AddWithInitialize(newEffect, newEffect.parentTemplate, true);
    }

    public void CopyEffect(WaterfallEffect toCopy, WaterfallEffectTemplate template)
    {
      Utils.Log($"[ModuleWaterfallFX]: Copying effect {toCopy}", LogType.Modules);
      var newEffect = new WaterfallEffect(toCopy);
      AddWithInitialize(newEffect, template, false);
    }

    private void AddWithInitialize(WaterfallEffect effect, WaterfallEffectTemplate target, bool fromNothing)
    {
      if (target != null)
      {
        foreach (var t in Templates.Where(x => x == target))
        {
          t.allFX.Add(effect);
        }
      }

      allFX.Add(effect);
      if (effect.InitializeEffect(this, fromNothing, useRelativeScaling))
      {
        activeFX.Add(effect);
      }
      refreshRenderers = true;
    }

    public void RemoveEffect(WaterfallEffect toRemove)
    {
      Utils.Log("[ModuleWaterfallFX]: Deleting effect", LogType.Modules);

      toRemove.CleanupEffect();
      if (toRemove.parentTemplate != null)
      {
        foreach (var t in Templates)
        {
          if (t == toRemove.parentTemplate)
          {
            t.allFX.Remove(toRemove);
          }
        }
      }

      allFX.Remove(toRemove);
      refreshRenderers = true;
    }

    /// <summary>
    ///   Does initialization of everything woo
    /// </summary>
    protected void Initialize()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing", LogType.Modules);

      // Some shaders require the depth texture; force-enable that.
      FlightCamera.fetch.mainCamera.depthTextureMode |= DepthTextureMode.Depth;

      // we load controllers and effects here instead of OnLoad because OnLoad will not be called for modules that exist in a prefab but not craft file
      // e.g. if a craft file was saved without waterfall installed
      if (config == null)
      {
        FetchConfigFromPrefab();
      }
      
      if (config != null)
      {
        CleanupEffects();
        allFX.Clear();
        allTemplates.Clear();
        allControllers.Clear();

        if (!config.HasValue(nameof(version)))
        {
          version = Version.Initial;
        }

        LoadControllers(config);
        LoadEffects(config);
      }

      InitializeControllers();
      InitializeEffects();

      UpgradeToCurrentVersion();

      isAwake = true;
    }

    private void UpgradeToCurrentVersion()
    {
      if (version < CurrentVersion)
      {
        foreach (var c in Controllers)
        {
          c.UpgradeToCurrentVersion(version);
        }
        version = CurrentVersion;
      }
    }

    /// <summary>
    ///   Initialize the CONTROLLERS
    /// </summary>
    protected void InitializeControllers()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing Controllers", LogType.Modules);
      foreach (var kvp in allControllers)
      {
        Utils.Log($"[ModuleWaterfallFX]: Initializing controller {kvp.Key}", LogType.Modules);
        kvp.Value.Initialize(this);
      }
    }

    /// <summary>
    ///   Initialize the EFFECTS
    /// </summary>
    protected void InitializeEffects()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing Effects", LogType.Modules);
      activeFX.Clear();
      foreach (var fx in allFX)
      {
        Utils.Log($"[ModuleWaterfallFX]: Initializing effect {fx.name}");
        if (fx.InitializeEffect(this, false, useRelativeScaling))
        {
          activeFX.Add(fx);
        }
      }
      refreshRenderers = true;
    }

    protected void CleanupEffects()
    {
      Utils.Log("[ModuleWaterfallFX]: Cleanup Effects", LogType.Modules);
      foreach (var fx in allFX)
      {
        fx.CleanupEffect();
      }
    }
  }
}
