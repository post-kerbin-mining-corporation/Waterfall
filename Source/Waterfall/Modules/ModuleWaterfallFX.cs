using System;
using System.Collections.Generic;
using System.Linq;
using Waterfall.EffectControllers;

namespace Waterfall
{
  public class ModuleWaterfallFX : PartModule
  {
    // This identifies this FX module for reference elsewhere
    [KSPField(isPersistant = false)] public string moduleID = "";

    // This links to an EngineID from a ModuleEnginesFX
    [KSPField(isPersistant = false)] public string engineID = "";

    [KSPField(isPersistant = false)] public bool useRelativeScaling;


    protected readonly Dictionary<string, WaterfallController> allControllers = new(16);

    protected readonly List<WaterfallEffect>         allFX = new(16);
    protected readonly List<WaterfallEffectTemplate> allTemplates = new(16);

    protected bool initialized;
    private   bool isHDR;

    public List<WaterfallEffect> FX => allFX;

    public List<WaterfallEffectTemplate> Templates => allTemplates;

    public List<WaterfallController> Controllers => allControllers.Values.ToList();

    public void Start()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        if (allControllers.Count == 0 || allFX.Count == 0)
        {
          var oldNode = FetchConfig();
          if (oldNode != null)
          {
            if (allControllers.Count == 0)
              LoadControllers(oldNode);
            if (allFX.Count == 0)
            {
              LoadEffects(oldNode);
            }
          }
        }

        Initialize();
      }
    }

    protected void LateUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight && allFX != null)
      {
        bool changeHDR = FlightCamera.fetch.cameras[0].allowHDR != isHDR;
        if (changeHDR)
          isHDR = !isHDR;

        foreach (var fx in allFX)
        {
          fx.Update();
          if (changeHDR)
            fx.SetHDR(isHDR);
        }
      }
    }


    /// <summary>
    ///   Load alll CONTROLLERS, TEMPLATES and EFFECTS
    /// </summary>
    /// <param name="node"></param>
    public override void OnLoad(ConfigNode node)
    {
      base.OnLoad(node);

      Utils.Log($"[ModuleWaterfallFX]: OnLoad called with contents \n{node}", LogType.Modules);

      var effectNodes   = node.GetNodes(WaterfallConstants.EffectNodeName);
      var templateNodes = node.GetNodes(WaterfallConstants.TemplateNodeName);

      if (initialized)
      {
        Utils.Log("[ModuleWaterfallFX]: Already initialized, cleaning up effects", LogType.Modules);
        CleanupEffects();
      }

      LoadControllers(node);

      Utils.Log($"[ModuleWaterfallFX]: Loading Effects on moduleID {moduleID}", LogType.Modules);

      if (effectNodes.Length > 0 && allFX.Count > 0 || allFX.Count > 0 && templateNodes.Length > 0)
      {
        CleanupEffects();
        allFX.Clear();
        allTemplates.Clear();
      }

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
        //string templateName = "";
        //string overrideTransformName = "";
        //Vector3 scaleOffset = Vector3.one;
        //Vector3 positionOffset = Vector3.zero;
        //Vector3 rotationOffset = Vector3.zero;


        //templateNode.TryGetValue("templateName", ref templateName);
        //templateNode.TryGetValue("overrideParentTransform", ref overrideTransformName);
        //templateNode.TryParseVector3("scale", ref scaleOffset);
        //templateNode.TryParseVector3("rotation", ref rotationOffset);
        //templateNode.TryParseVector3("position", ref positionOffset);

        //WaterfallTemplate template = WaterfallTemplates.GetTemplate(templateName);

        //foreach (WaterfallEffect fx in template.allFX)
        //{
        //  allFX.Add(new WaterfallEffect(fx, positionOffset, rotationOffset, scaleOffset, overrideTransformName));
        //}
        //Utils.Log($"[ModuleWaterfallFX]: Loaded effect template {template.templateName}", LogType.Modules);
      }

      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allTemplates.Count} templates", LogType.Modules);
      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allFX.Count} effects",          LogType.Modules);

      if (initialized)
      {
        Utils.Log("[ModuleWaterfallFX]: Reinitializing", LogType.Modules);
        InitializeControllers();
        ReinitializeEffects();
      }
    }

    /// <summary>
    ///   Dumps the entire set of EFFECTS to a confignode, then to a string
    /// </summary>
    /// <returns></returns>
    public string ExportModule()
    {
      var newNode = new ConfigNode("MODULE");
      newNode.AddValue("name",     "ModuleWaterfallFX");
      newNode.AddValue("moduleID", moduleID);
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

    public override void OnAwake()
    {
      base.OnAwake();
      if (HighLogic.LoadedSceneIsFlight) { }
    }

    private void LoadEffects(ConfigNode node)
    {
      Utils.Log($"[ModuleWaterfallFX]: Loading Effects on moduleID {moduleID}", LogType.Modules);
      var effectNodes   = node.GetNodes(WaterfallConstants.EffectNodeName);
      var templateNodes = node.GetNodes(WaterfallConstants.TemplateNodeName);
      if (allFX.Count == 0)
      {
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
      }

      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allTemplates.Count} templates", LogType.Modules);
      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allFX.Count} effects",          LogType.Modules);
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
      if (allControllers.Count != 0)
        return;

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

    private ConfigNode FetchConfig()
    {
      Utils.Log(String.Format("[ModuleWaterfallFX]: Finding config for {0}", moduleID), LogType.Modules);

      foreach (var pNode in GameDatabase.Instance.GetConfigs("PART"))
      {
        if (pNode.name.Replace("_", ".") == part.partInfo.name)
        {
          var fxNodes = pNode.config.GetNodes("MODULE").ToList().FindAll(n => n.GetValue("name") == moduleName);
          if (fxNodes.Count > 1)
          {
            try
            {
              return fxNodes.Single(n => n.GetValue("moduleID") == moduleID);
            }
            catch (InvalidOperationException)
            {
              // Thrown if predicate is not fulfilled, ie moduleName is not unqiue
              Utils.Log(String.Format("[ModuleWaterfallFX]: Critical configuration error: Multiple ModuleWaterfallFX nodes found with identical or no moduleID {0}", moduleID), LogType.Modules);
            }
            catch (ArgumentNullException)
            {
              // Thrown if ModuleCryoTank is not found (a Large Problem (tm))
              Utils.Log("[ModuleWaterfallFX]: Critical configuration error: No ModuleWaterfallFX nodes found in part", LogType.Modules);
            }
          }
          else
          {
            return fxNodes[0];
          }
        }
      }

      return null;
    }


    // VAB Inforstrings are blank
    public string GetModuleTitle() => "";

    public override string GetInfo() => "";

    /// <summary>
    ///   Gets the value of the requested controller by name
    /// </summary>
    /// <param name="controllerName"></param>
    /// <returns></returns>
    public List<float> GetControllerValue(string controllerName)
    {
      return allControllers.TryGetValue(controllerName, out var controllerValue) ? controllerValue.Get() : (new() { 0f });
    }

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
    }

    public void RemoveController(WaterfallController toRemove)
    {
      Utils.Log("[ModuleWaterfallFX]: Deleting controller", LogType.Modules);
      allControllers.Remove(toRemove.name);
    }

    public void AddEffect(WaterfallEffect newEffect)
    {
      Utils.Log("[ModuleWaterfallFX]: Added new effect", LogType.Modules);

      if (newEffect.parentTemplate != null)
      {
        foreach (var t in Templates)
        {
          if (t == newEffect.parentTemplate)
          {
            t.allFX.Add(newEffect);
          }
        }
      }

      allFX.Add(newEffect);
      newEffect.InitializeEffect(this, true, useRelativeScaling);
    }

    public void CopyEffect(WaterfallEffect toCopy, WaterfallEffectTemplate template)
    {
      Utils.Log($"[ModuleWaterfallFX]: Copying effect {toCopy}", LogType.Modules);

      var newEffect = new WaterfallEffect(toCopy);

      if (template != null)
      {
        foreach (var t in Templates)
        {
          if (t == template)
          {
            t.allFX.Add(newEffect);
          }
        }
      }

      allFX.Add(newEffect);
      newEffect.InitializeEffect(this, false, useRelativeScaling);
    }

    public void RemoveEffect(WaterfallEffect toRemove)
    {
      Utils.Log("[ModuleWaterfallFX]: Deleting effect", LogType.Modules);

      toRemove.CleanupEffect(this);
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
    }

    /// <summary>
    ///   Does initialization of everything woo
    /// </summary>
    protected void Initialize()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing", LogType.Modules);
      InitializeControllers();
      InitializeEffects();
      initialized = true;
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
      foreach (var fx in allFX)
      {
        Utils.Log($"[ModuleWaterfallFX]: Initializing effect {fx.name}");
        fx.InitializeEffect(this, false, useRelativeScaling);
      }
    }

    protected void ReinitializeEffects()
    {
      Utils.Log("[ModuleWaterfallFX]: Reinitializing Effects", LogType.Modules);
      for (int i = 0; i < allFX.Count; i++)
      {
        allFX[i].InitializeEffect(this, false, useRelativeScaling);
      }
    }

    protected void CleanupEffects()
    {
      Utils.Log("[ModuleWaterfallFX]: Cleanup Effects", LogType.Modules);
      foreach (var fx in allFX)
      {
        fx.CleanupEffect(this);
      }
    }

    /// <summary>
    ///   Sets a specific controller's value
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetControllerValue(string name, float value)
    {
      if (allControllers.TryGetValue(name, out var controller))
        controller.Set(value);
      else
        Utils.Log($"[ModuleWaterfallFX] Couldn't SetControllerValue for id {name}", LogType.Modules);
    }

    /// <summary>
    ///   Sets this module controllers as overridden by something: its controllers will be set to override
    /// </summary>
    /// <param name="mode"></param>
    public void SetControllerOverride(bool mode)
    {
      foreach (var kvp in allControllers)
      {
        kvp.Value.SetOverride(mode);
      }
    }

    /// <summary>
    ///   Sets this module controllers as overridden by something: its controllers will be set to override
    /// </summary>
    /// <param name="mode"></param>
    public void SetControllerOverride(string name, bool mode)
    {
      if (allControllers.TryGetValue(name, out var controller))
        controller.SetOverride(mode);
      else
        Utils.Log($"[ModuleWaterfallFX] Couldn't SetControllerOverride for id {name}", LogType.Modules);
    }

    /// <summary>
    ///   Sets a specific controller as overridden by something: its controllers will be set to override and a value will be supplied
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetControllerOverrideValue(string name, float value)
    {
      if (allControllers.TryGetValue(name, out var controller))
        controller.SetOverrideValue(value);
      else
        Utils.Log($"[ModuleWaterfallFX] Couldn't SetControllerOverrideValue for id {name}", LogType.Modules);
    }
  }
}