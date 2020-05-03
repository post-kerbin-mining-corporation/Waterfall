using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// This class represents a single effect. An effect contains a MODEL with various PROPERTIES, modified by EFFECTS
  /// </summary>
  public class WaterfallEffect
  {
    public string name = "";
    public string parentName = "";

    protected WaterfallModel model;
    protected List<EffectModifier> fxModifiers;
    protected Transform parentTransform;
    protected ModuleWaterfallFX parentModule;

    public List<EffectModifier> FXModifiers
    {
      get { return fxModifiers; }
      set { fxModifiers = value; }
    }

    public WaterfallModel FXModel
    {
      get { return model; }
    }

    public WaterfallEffect()
    {}

    public WaterfallEffect(ConfigNode node)
    {
      Load(node);
    }

    /// <summary>
    /// Load the node from config
    /// </summary>
    /// <param name="node"></param>
    public void Load(ConfigNode node)
    {
      node.TryGetValue("name", ref name);

      if (!node.TryGetValue("parentName", ref parentName))
      {
        Utils.LogError(String.Format("[WaterfallEffect]: EFFECT with name {0} does not define parentName, which is required", name));
        return;
      }
      model = new WaterfallModel(node.GetNode(WaterfallConstants.ModelNodeName));
      fxModifiers = new List<EffectModifier>();

      // Scale types
      ConfigNode[] scalingNodes = node.GetNodes("SCALEMODIFIER");
      ConfigNode[] colorNodes = node.GetNodes("COLORMODIFIER");
      ConfigNode[] uvOffsetNodes = node.GetNodes("UVOFFSETMODIFIER");

     
      foreach (ConfigNode subNode in scalingNodes)
      {
        fxModifiers.Add(new EffectScaleModifier(subNode));
      }
      foreach (ConfigNode subNode in colorNodes)
      {
        fxModifiers.Add(new EffectColorModifier(subNode));
      }
      foreach (ConfigNode subNode in uvOffsetNodes)
      {
        fxModifiers.Add(new EffectUVScrollModifier(subNode));
      }
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.EffectNodeName;
      node.AddValue("name", name);
      node.AddValue("parentName", parentName);
      node.AddNode(model.Save());
      for (int i = 0; i < fxModifiers.Count; i++)
      {
        node.AddNode(fxModifiers[i].Save());
      }

      return node;
    }

    public void InitializeEffect(ModuleWaterfallFX host)
    {

      Utils.Log(String.Format("[WaterfallEffect]: Initializing effect {0} ", name));
      parentModule = host;
      parentTransform = parentModule.part.FindModelTransform(parentName);
      if (parentTransform == null)
        Utils.LogError(String.Format("[WaterfallEffect]: Couldn't find parentTransform {0} ", parentName));

      model.Initialize(parentTransform);

      for (int i = 0; i < fxModifiers.Count; i++)
      {
        fxModifiers[i].Init(this);
      }
    }

    public Transform GetModelTransform()
    {
      return model.modelTransform;
    }

    public void Update()
    {
      for (int i=0; i < fxModifiers.Count; i++)
      {
        fxModifiers[i].Apply(parentModule.GetControllerValue(fxModifiers[i].controllerName));
      }
    }
  }

}
