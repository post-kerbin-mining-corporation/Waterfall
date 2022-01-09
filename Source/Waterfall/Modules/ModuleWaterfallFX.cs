using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Waterfall.UI.EffectControllersUI;

namespace Waterfall
{
  public class ModuleWaterfallFX : PartModule
  {
    // This identifies this FX module for reference elsewhere
    [KSPField(isPersistant = false)]
    public string moduleID = "";

    // This links to an EngineID from a ModuleEnginesFX
    [KSPField(isPersistant = false)]
    public string engineID = "";

    [KSPField(isPersistant = false)]
    public bool useRelativeScaling = false;
    
    
    protected Dictionary<string, WaterfallController> allControllers;
    
    protected List<WaterfallEffect> allFX;
    protected List<WaterfallEffectTemplate> allTemplates;

    protected bool initialized = false;

    public List<WaterfallEffect> FX
    { get { return allFX; } }

    public List<WaterfallEffectTemplate> Templates
    { get { return allTemplates; } }

    public List<WaterfallController> Controllers
    { get { return allControllers.Values.ToList(); } }


    /// <summary>
    /// Load alll CONTROLLERS, TEMPLATES and EFFECTS
    /// </summary>
    /// <param name="node"></param>
    /// 

    public override void OnLoad(ConfigNode node)
    {
      base.OnLoad(node);

      Utils.Log($"[ModuleWaterfallFX]: OnLoad called with contents \n{node.ToString()}", LogType.Modules);

      ConfigNode[] effectNodes = node.GetNodes(WaterfallConstants.EffectNodeName);
      ConfigNode[] templateNodes = node.GetNodes(WaterfallConstants.TemplateNodeName);

      if (initialized)
      {
        Utils.Log($"[ModuleWaterfallFX]: Already initialized, cleaning up effects", LogType.Modules);
        CleanupEffects();
      }

      LoadControllers(node);

      Utils.Log(String.Format("[ModuleWaterfallFX]: Loading Effects on moduleID {0}", moduleID), LogType.Modules);

      if (allFX == null)
      {
        allFX = new List<WaterfallEffect>();
      }
      if (allTemplates == null)
      {
        allTemplates = new List<WaterfallEffectTemplate>();
      }
      else
      {
        if (effectNodes.Length > 0 && allFX.Count > 0 || allFX.Count > 0 && templateNodes.Length > 0)
        {
          CleanupEffects();
          allFX.Clear();
          allTemplates.Clear();
        }
      }

      foreach (ConfigNode fxDataNode in effectNodes)
      {
        allFX.Add(new WaterfallEffect(fxDataNode));
      }


      Utils.Log(String.Format("[ModuleWaterfallFX]: Loading Template effects on moduleID {0}", moduleID), LogType.Modules);
      foreach (ConfigNode templateNode in templateNodes)
      {

        WaterfallEffectTemplate template = new WaterfallEffectTemplate(templateNode);
        allTemplates.Add(template);
        foreach (WaterfallEffect fx in template.allFX)
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
      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allFX.Count} effects", LogType.Modules);

      if (initialized)
      {
        Utils.Log($"[ModuleWaterfallFX]: Reinitializing", LogType.Modules);
        InitializeControllers();
        ReinitializeEffects();
      }

    }
    /// <summary>
    /// Dumps the entire set of EFFECTS to a confignode, then to a string
    /// </summary>
    /// <returns></returns>
    public String ExportModule()
    {
      ConfigNode newNode = new ConfigNode("MODULE");
      newNode.AddValue("name", "ModuleWaterfallFX");
      newNode.AddValue("moduleID", moduleID);
      //newNode.AddValue("engineID", engineID);

      string toRet = "";
      foreach (WaterfallController ctrl in Controllers)
      {
        newNode.AddNode(ctrl.Save());
      }
      if (Templates.Count > 0)
      {
        foreach (WaterfallEffectTemplate templ in Templates)
        {
          newNode.AddNode(templ.Save());
        }
      }
      else
      {
        foreach (WaterfallEffect fx in allFX)
        {
          newNode.AddNode(fx.Save());
        }
      }
      return newNode.ToString();
    }
    public String ExportControllers()
    {
      string toRet = "";
      foreach (WaterfallController ctrl in Controllers)
      {
        toRet += $"{ctrl.Save().ToString()}{Environment.NewLine}";
      }
      return toRet;
    }
    /// <summary>
    /// Dumps the entire set of EFFECTS to a confignode, then to a string
    /// </summary>
    /// <returns></returns>
    public String ExportEffects()
    {
      string toRet = "";
      foreach (WaterfallEffect fx in allFX)
      {
        toRet += $"{fx.Save().ToString()}{Environment.NewLine}";
      }
      return toRet;
    }
    public override void OnAwake()
    {
      base.OnAwake();
      if (HighLogic.LoadedSceneIsFlight)
      {
        
      }
    }
    public void Start()
    {

      if (HighLogic.LoadedSceneIsFlight)
      {
        if (allControllers == null || allControllers.Count == 0 || allFX == null || allFX.Count == 0)
        {
          ConfigNode oldNode = FetchConfig();
          if (oldNode != null)
          {
            if (allControllers == null || allControllers.Count == 0)
              LoadControllers(oldNode);
            if (allFX == null || allFX.Count == 0)
            {
              LoadEffects(oldNode);
            }
          }
        }
        Initialize();
      }
    }
    void LoadEffects(ConfigNode node)
    {
      Utils.Log(String.Format("[ModuleWaterfallFX]: Loading Effects on moduleID {0}", moduleID), LogType.Modules);
      ConfigNode[] effectNodes = node.GetNodes(WaterfallConstants.EffectNodeName);
      ConfigNode[] templateNodes = node.GetNodes(WaterfallConstants.TemplateNodeName);
      if (allFX == null) allFX = new List<WaterfallEffect>();
      if (allTemplates == null) allTemplates = new List<WaterfallEffectTemplate>();
      if (allFX.Count == 0)
      {


        foreach (ConfigNode fxDataNode in effectNodes)
        {
          allFX.Add(new WaterfallEffect(fxDataNode));
        }


        Utils.Log(String.Format("[ModuleWaterfallFX]: Loading Template effects on moduleID {0}", moduleID), LogType.Modules);
        foreach (ConfigNode templateNode in templateNodes)
        {
          WaterfallEffectTemplate template = new WaterfallEffectTemplate(templateNode);
          allTemplates.Add(template);
          foreach (WaterfallEffect fx in template.allFX)
          {
            allFX.Add(fx);
          }
          Utils.Log($"[ModuleWaterfallFX]: Loaded effect template {template.templateName}", LogType.Modules);

        


        }
      }
      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allTemplates.Count} templates", LogType.Modules);
      Utils.Log($"[ModuleWaterfallFX]: Finished loading {allFX.Count} effects", LogType.Modules);
    }

    /// <summary>
    ///     Determine controller type from config node in the old format (config node named <see cref="WaterfallConstants.LegacyControllerNodeName"/> with <see cref="WaterfallController.LegacyControllerTypeNodeName"/> property).
    ///     If value of config node entry named <see cref="WaterfallController.LegacyControllerTypeNodeName"/> is missing or incorrect, default to <see cref="ThrottleController"/> type.
    /// </summary>
    /// <remarks>
    ///     <see href="https://github.com/post-kerbin-mining-corporation/Waterfall/pull/95"/> for info regarding change to new format.
    /// </remarks>
    EffectControllerInfo DetermineControllerTypeFromLegacyConfigNode(ConfigNode configNode)
    {
      EffectControllerInfo controllerType = EffectControllersMetadata.ControllersByType[typeof(ThrottleController)];

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

    void LoadControllers(ConfigNode node)
    {
      if (allControllers != null && allControllers.Count != 0)
        return;

      Utils.Log($"[ModuleWaterfallFX]: Loading effect controllers on moduleID {moduleID}", LogType.Modules);

      allControllers = new();
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

    ConfigNode FetchConfig()
    {
      Utils.Log(String.Format("[ModuleWaterfallFX]: Finding config for {0}", moduleID), LogType.Modules);
      
      foreach (UrlDir.UrlConfig pNode in GameDatabase.Instance.GetConfigs("PART"))
      {
        if (pNode.name.Replace("_", ".") == part.partInfo.name)
        {
          List<ConfigNode> fxNodes = pNode.config.GetNodes("MODULE").ToList().FindAll(n => n.GetValue("name") == moduleName);
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
              Utils.Log(String.Format("[ModuleWaterfallFX]: Critical configuration error: No ModuleWaterfallFX nodes found in part"), LogType.Modules);
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
    public string GetModuleTitle()
    {
      return "";
    }

    public override string GetInfo()
    {
      return "";
    }

    /// <summary>
    /// Gets the value of the requested controller by name
    /// </summary>
    /// <param name="controllerName"></param>
    /// <returns></returns>
    public List<float> GetControllerValue(string controllerName)
    {
      if (allControllers.ContainsKey(controllerName))
      {
        return allControllers[controllerName].Get();
      }

      return new List<float> { 0f };

    }

    /// <summary>
    /// Gets the list of controllers
    /// </summary>
    /// <param name="controllerName"></param>
    /// <returns></returns>
    public List<string> GetControllerNames()
    {
      return allControllers.Keys.ToList();

    }

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

      if (newEffect.parentTemplate != null && Templates != null)
      {
        foreach(WaterfallEffectTemplate t in Templates )
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

      WaterfallEffect newEffect = new WaterfallEffect(toCopy);

      if (Templates != null && template != null)
      {
        foreach (WaterfallEffectTemplate t in Templates)
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
      if (toRemove.parentTemplate != null && Templates != null)
      {
        foreach (WaterfallEffectTemplate t in Templates)
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
    /// Does initialization of everything woo
    /// </summary>
    protected void Initialize()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing", LogType.Modules);
      InitializeControllers();
      InitializeEffects();
      initialized = true;
    }

    /// <summary>
    /// Initialize the CONTROLLERS
    /// </summary>
    protected void InitializeControllers()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing Controllers", LogType.Modules);
      foreach (KeyValuePair<string, WaterfallController> kvp in allControllers)
      {
        Utils.Log($"[ModuleWaterfallFX]: Initializing controller {kvp.Key}", LogType.Modules);
        allControllers[kvp.Key].Initialize(this);
      }
    }

    /// <summary>
    /// Initialize the EFFECTS
    /// </summary>
    protected void InitializeEffects()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing Effects", LogType.Modules);
      for (int i = 0; i < allFX.Count; i++)
      {
        Utils.Log($"[ModuleWaterfallFX]: Initializing effect {allFX[i].name}");
        allFX[i].InitializeEffect(this, false, useRelativeScaling);
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
      for (int i = 0; i < allFX.Count; i++)
      {
        allFX[i].CleanupEffect(this);
      }
    }
    bool isHDR = false;
    protected void LateUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight && allFX != null)
      {
        bool changeHDR = false;
        if (FlightCamera.fetch.cameras[0].allowHDR != isHDR)
        {
          changeHDR = true;
          isHDR = FlightCamera.fetch.cameras[0].allowHDR;
        }
        for (int i = 0; i < allFX.Count; i++)
        {
          allFX[i].Update();
          if (changeHDR)
            allFX[i].SetHDR(isHDR);
        }
      }
    }

    /// <summary>
    /// Sets a specific controller's value
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetControllerValue(string name, float value)
    {
      if (allControllers.ContainsKey(name))
        allControllers[name].Set(value);
      else
        Utils.Log($"[ModuleWaterfallFX] Couldn't SetControllerValue for id {name}", LogType.Modules);
    }

    /// <summary>
    /// Sets this module controllers as overridden by something: its controllers will be set to override
    /// </summary>
    /// <param name="mode"></param>
    public void SetControllerOverride(bool mode)
    {
      foreach (KeyValuePair<string, WaterfallController> kvp in allControllers)
      {
        allControllers[kvp.Key].SetOverride(mode);

      }
    }

    /// <summary>
    /// Sets this module controllers as overridden by something: its controllers will be set to override
    /// </summary>
    /// <param name="mode"></param>
    public void SetControllerOverride(string name, bool mode)
    {
      if (allControllers.ContainsKey(name))
        allControllers[name].SetOverride(mode);
      else
        Utils.Log($"[ModuleWaterfallFX] Couldn't SetControllerOverride for id {name}", LogType.Modules);

    }

    /// <summary>
    /// Sets a specific controller as overridden by something: its controllers will be set to override and a value will be supplied
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetControllerOverrideValue(string name, float value)
    {
      if (allControllers.ContainsKey(name))
        allControllers[name].SetOverrideValue(value);
      else
        Utils.Log($"[ModuleWaterfallFX] Couldn't SetControllerOverrideValue for id {name}", LogType.Modules);
    }


  }
}
