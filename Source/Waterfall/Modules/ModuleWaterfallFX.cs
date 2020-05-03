using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

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


    Dictionary<string, WaterfallController> allControllers;
    List<WaterfallEffect> allFX;

    public List<WaterfallEffect> FX
    { get { return allFX; } }

    /// <summary>
    /// Load alll CONTROLLERS and EFFECTS
    /// </summary>
    /// <param name="node"></param>
    public override void OnLoad(ConfigNode node)
    {
      base.OnLoad(node);

      ConfigNode[] controllerNodes = node.GetNodes(WaterfallConstants.ControllerNodeName);
      ConfigNode[] effectNodes = node.GetNodes(WaterfallConstants.EffectNodeName);

      Utils.Log(String.Format("[ModuleWaterfallFX]: Loading controllers on moduleID {0}", moduleID));
      allControllers = new Dictionary<string, WaterfallController>();
      foreach (ConfigNode controllerDataNode in controllerNodes)
      {
        string ctrlType = "throttle";
        if (!controllerDataNode.TryGetValue("linkedTo", ref ctrlType))
        {
          Utils.LogWarning(String.Format("[ModuleWaterfallFX]: Controller on moduleID {0} does not define linkedTo, setting throttle as default ", moduleID));
        }
        if (ctrlType == "throttle")
        {
          ThrottleController tCtrl = new ThrottleController(controllerDataNode);
          allControllers.Add(tCtrl.name, tCtrl);
        }
        if (ctrlType == "atmosphere_density")
        {
          AtmosphereDensityController aCtrl = new AtmosphereDensityController(controllerDataNode);
          allControllers.Add(aCtrl.name, aCtrl);
        }
      }

      Utils.Log(String.Format("[ModuleWaterfallFX]: Loading effects on moduleID {0}", moduleID));
      allFX = new List<WaterfallEffect>();
      foreach (ConfigNode fxDataNode in effectNodes)
      {
        allFX.Add(new WaterfallEffect(fxDataNode));
      }

      Utils.Log("[ModuleWaterfallFX]: Finished loading");
    }

    public ConfigNode Export()
    {
      ConfigNode node = new ConfigNode();
      foreach (WaterfallEffect fx in allFX)
      {
        node.AddNode(fx.Save());
      }
      return node;
    }

    public void Start()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        ReloadDatabaseNodes();
        Initialize();
      }
    }
    void ReloadDatabaseNodes()
    {
      if (allFX == null || allFX.Count == 0)
      {
        Debug.Log(String.Format("[ModuleWaterfallFX]: Reloading ConfigNodes for {0}", part.partInfo.name));

        ConfigNode cfg;
        foreach (UrlDir.UrlConfig pNode in GameDatabase.Instance.GetConfigs("PART"))
        {
          if (pNode.name.Replace("_", ".") == part.partInfo.name)
          {
            List<ConfigNode> fxNodes = pNode.config.GetNodes("MODULE").ToList().FindAll(n => n.GetValue("name") == moduleName);
            if (fxNodes.Count > 1)
            {
              try
              {
                ConfigNode node = fxNodes.Single(n => n.GetValue("moduleID") == moduleID);
                OnLoad(node);
              }
              catch (InvalidOperationException)
              {
                // Thrown if predicate is not fulfilled, ie moduleName is not unqiue
                Debug.Log(String.Format("[ModuleWaterfallFX]: Critical configuration error: Multiple ModuleWaterfallFX nodes found with identical or no moduleName"));
              }
              catch (ArgumentNullException)
              {
                // Thrown if ModuleCryoTank is not found (a Large Problem (tm))
                Debug.Log(String.Format("[ModuleWaterfallFX]: Critical configuration error: No ModuleWaterfallFX nodes found in part"));
              }
            }
            else
            {
              OnLoad(fxNodes[0]);
            }
          }
        }
      }
    }
    public string GetModuleTitle()
    {
      return "";
    }

    public override string GetInfo()
    {

      return "";
    }

    public float GetControllerValue(string controllerName)
    {
      if (allControllers.ContainsKey(controllerName))
      {
        return allControllers[controllerName].Get();
      }
      return 0f;
      
    }

    /// <summary>
    /// Does initialization of everything woo
    /// </summary>
    protected void Initialize()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing");
      InitializeControllers();
      InitializeEffects();
    }

    /// <summary>
    /// Initialize the CONTROLLERS
    /// </summary>
    protected void InitializeControllers()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing Controllers");
      foreach (KeyValuePair<string, WaterfallController> kvp in allControllers)
      {
        allControllers[kvp.Key].Initialize(this);
      }
    }

    /// <summary>
    /// Initialize the EFFECTS
    /// </summary>
    protected void InitializeEffects()
    {
      Utils.Log("[ModuleWaterfallFX]: Initializing Effects");
      for (int i = 0; i < allFX.Count; i++)
      {
        allFX[i].InitializeEffect(this);
      }
    }

    protected void LateUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        for (int i = 0; i < allFX.Count; i++)
        {
          allFX[i].Update();
        }
      }
    }
    public void SetControllerOverride(bool mode)
    {
      foreach (KeyValuePair<string, WaterfallController> kvp in allControllers)
      {
        allControllers[kvp.Key].SetOverride(mode);
      }
    }
    public void SetControllerOverrideValue(string name, float value)
    {
      allControllers[name].SetOverrideValue(value);
    }

  }
}
