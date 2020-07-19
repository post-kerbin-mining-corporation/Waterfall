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

    public Vector3 PositionOffset { get; set; }
    public Vector3 RotationOffset { get; set; }
    public Vector3 ScaleOffset { get; set; }
    public ModuleWaterfallFX parentModule;

    protected WaterfallModel model;
    protected List<EffectModifier> fxModifiers;
    protected Transform parentTransform;
    protected Transform effectTransform;
    protected ConfigNode savedNode;
    protected bool effectVisible = true;
    protected Vector3 savedScale;
    protected List<Transform> effectTransforms;
    public List<EffectModifier> FXModifiers
    {
      get { return fxModifiers; }
      set { fxModifiers = value; }
    }

    public WaterfallModel FXModel
    {
      get { return model; }
    }

    public ConfigNode SavedNode
    {
      get { return savedNode; }
    }

    public WaterfallEffect() {}

    public WaterfallEffect(ConfigNode node)
    {
      PositionOffset = Vector3.zero;
      RotationOffset = Vector3.zero;
      ScaleOffset = Vector3.one;
      Load(node);
    }

    public WaterfallEffect(ConfigNode node, Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset)
    {
      PositionOffset = positionOffset;
      RotationOffset = rotationOffset;
      ScaleOffset = scaleOffset;

      Load(node);
    }

    public WaterfallEffect(WaterfallEffect fx, Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset, string overrideTransformName)
    {
      PositionOffset = positionOffset;
      RotationOffset = rotationOffset;
      ScaleOffset = scaleOffset;
      if (overrideTransformName != "")
        fx.savedNode.SetValue("parentName", overrideTransformName, true);
      Load(fx.savedNode);
    }

    /// <summary>
    /// Load the node from config
    /// </summary>
    /// <param name="node"></param>
    public void Load(ConfigNode node)
    {
      savedNode = node;
      node.TryGetValue("name", ref name);

      if (!node.TryGetValue("parentName", ref parentName))
      {
        Utils.LogError(String.Format("[WaterfallEffect]: EFFECT with name {0} does not define parentName, which is required", name));
        return;
      }
      model = new WaterfallModel(node.GetNode(WaterfallConstants.ModelNodeName));
      fxModifiers = new List<EffectModifier>();

      // types
      ConfigNode[] positionNodes = node.GetNodes(WaterfallConstants.PositionModifierNodeName);
      ConfigNode[] rotationNodes = node.GetNodes(WaterfallConstants.RotationModifierNodeName);
      ConfigNode[] scalingNodes = node.GetNodes(WaterfallConstants.ScaleModifierNodeName);
      ConfigNode[] colorNodes = node.GetNodes(WaterfallConstants.ColorModifierNodeName);
      ConfigNode[] uvOffsetNodes = node.GetNodes(WaterfallConstants.UVScrollModifierNodeName);
      ConfigNode[] floatNodes = node.GetNodes(WaterfallConstants.FloatModifierNodeName);

      foreach (ConfigNode subNode in positionNodes)
      {
        fxModifiers.Add(new EffectPositionModifier(subNode));
      }
      foreach (ConfigNode subNode in rotationNodes)
      {
        fxModifiers.Add(new EffectRotationModifier(subNode));
      }
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
      foreach (ConfigNode subNode in floatNodes)
      {
        fxModifiers.Add(new EffectFloatModifier(subNode));
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
    public void CleanupEffect(ModuleWaterfallFX host)
    {
      Utils.Log(String.Format("[WaterfallEffect]: Deleting effect {0} ", name), LogType.Effects);
      foreach (Transform t in effectTransforms)
      {
        GameObject.Destroy(effectTransform.gameObject);
      }
    }
     public void InitializeEffect(ModuleWaterfallFX host)
    {

      Utils.Log(String.Format("[WaterfallEffect]: Initializing effect {0} ", name), LogType.Effects);
      parentModule = host;
      Transform[] parents = parentModule.part.FindModelTransforms(parentName);
      effectTransforms = new List<Transform>();
      for (int i = 0; i < parents.Length; i++)
      {
        GameObject effect = new GameObject($"Waterfall_FX_{name}_{i}");
        effectTransform = effect.transform;

        if (parents[i] == null)
        {
          Utils.LogError(String.Format("[WaterfallEffect]: Couldn't find Parent Transform {0} on model to attach effect to", parentName));
          return;
        }
        model.Initialize(effectTransform);

        effectTransform.SetParent(parents[i], true);
        effectTransform.localPosition = PositionOffset;

        if (RotationOffset == Vector3.zero)
          effectTransform.localRotation = Quaternion.identity;
        else
          effectTransform.localRotation = Quaternion.LookRotation(RotationOffset);

        effectTransform.localScale = new Vector3(effectTransform.localScale.x * ScaleOffset.x, effectTransform.localScale.y * ScaleOffset.y, effectTransform.localScale.z * ScaleOffset.z);
        savedScale = effectTransform.localScale;
        Utils.Log($"[WaterfallEffect]: Effect GameObject {effect.name} generated at {effectTransform.localPosition}, {effectTransform.localRotation}, {effectTransform.localScale}", LogType.Effects);
        effectTransforms.Add(effectTransform);
      }

      for (int i = 0; i < fxModifiers.Count; i++)
      {
        fxModifiers[i].Init(this);
      }
    }

    public List<Transform> GetModelTransforms()
    {
      return model.modelTransforms;
    }

    public void Update()
    {
      if (effectVisible)
      {
        for (int i = 0; i < fxModifiers.Count; i++)
        {
          fxModifiers[i].Apply(parentModule.GetControllerValue(fxModifiers[i].controllerName));
        }
      }
    }


    public void RemoveModifier(EffectModifier mod)
    {
      fxModifiers.Remove(mod);

    }

    public void AddModifier(EffectModifier mod)
    {
      mod.Init(this);
      fxModifiers.Add(mod);

    }

    public void SetEnabled(bool state)
    {
      if (effectVisible != state)
      {
        if (state)
        {
          effectTransform.localScale = savedScale;
        } else
        {
          effectTransform.localScale = Vector3.one*0.00001f;
        }
        effectVisible = state;
      }
    }
  }

}
