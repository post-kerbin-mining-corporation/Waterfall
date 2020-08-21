using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// This class represents a single effect. An effect contains a MODEL with various PROPERTIES, modified by EFFECTS
  /// </summary>
  public class WaterfallEffect:ScriptableObject
  {
    public string name = "";
    public string parentName = "";

    public Vector3 TemplatePositionOffset { get; set; }
    public Vector3 TemplateRotationOffset { get; set; }
    public Vector3 TemplateScaleOffset { get; set; }
    public Vector3 baseScale { get; set; }
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

    public void SetupEffect() { }

    public void SetupEffect(string parent, WaterfallModel mdl)
    {
      parentName = parent;
      model = mdl;
      TemplatePositionOffset = Vector3.zero;
      TemplateRotationOffset = Vector3.zero;
      TemplateScaleOffset = Vector3.one;
      fxModifiers = new List<EffectModifier>();
    }

    public void SetupEffect(ConfigNode node)
    {
      TemplatePositionOffset = Vector3.zero;
      TemplateRotationOffset = Vector3.zero;
      TemplateScaleOffset = Vector3.one;
      Load(node);
    }

    public void SetupEffect(ConfigNode node, Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset)
    {
      TemplatePositionOffset = positionOffset;
      TemplateRotationOffset = rotationOffset;
      TemplateScaleOffset = scaleOffset;

      Load(node);
    }

    public void SetupEffect(WaterfallEffect fx, Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset, string overrideTransformName)
    {
      TemplatePositionOffset = positionOffset;
      TemplateRotationOffset = rotationOffset;
      TemplateScaleOffset = scaleOffset;
      if (overrideTransformName != "")
        fx.savedNode.SetValue("parentName", overrideTransformName, true);
      Load(fx.savedNode);
    }
    public void SetupEffect(WaterfallEffect fx)
    {
      TemplatePositionOffset = fx.TemplatePositionOffset;
      TemplateRotationOffset = fx.TemplateRotationOffset;
      TemplateScaleOffset = fx.TemplateScaleOffset;
      Load(fx.Save());
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
      model = ScriptableObject.CreateInstance<WaterfallModel>();
        model.SetupModel(node.GetNode(WaterfallConstants.ModelNodeName));
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
      for (int i= model.modelTransforms.Count-1; i >= 0 ; i--)
      
      {
        GameObject.Destroy(model.modelTransforms[i].gameObject);
      }
    }

    public void InitializeEffect(ModuleWaterfallFX host, bool fromNothing)
    {

      Utils.Log(String.Format("[WaterfallEffect]: Initializing effect {0} at {1}", name, parentName), LogType.Effects);
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
        effectTransform.SetParent(parents[i], true);
        effectTransform.localPosition = Vector3.zero;
        effectTransform.localEulerAngles = Vector3.zero;

        model.Initialize(effectTransform, fromNothing);


        baseScale = effectTransform.localScale;
        effectTransform.localPosition = TemplatePositionOffset;
        effectTransform.localEulerAngles = TemplateRotationOffset;
        effectTransform.localScale = Vector3.Scale(baseScale, TemplateScaleOffset);
        savedScale = effectTransform.localScale;


        Utils.Log($"[WaterfallEffect] Applyied template offsets {TemplatePositionOffset}, {TemplateRotationOffset}, {TemplateScaleOffset}");

        effectTransforms.Add(effectTransform);
      }

      for (int i = 0; i < fxModifiers.Count; i++)
      {
        fxModifiers[i].Init(this);
      }
    }
    public void ApplyTemplateOffsets(Vector3 position, Vector3 rotation, Vector3 scale)
    {
      TemplatePositionOffset = position;
      TemplateRotationOffset = rotation;
      TemplateScaleOffset = scale;

      Utils.Log($"[WaterfallEffect] Applying template offsets {position}, {rotation}, {scale}");


      foreach (Transform modelTransform in effectTransforms)
      {
        modelTransform.localPosition = TemplatePositionOffset;
        modelTransform.localScale = Vector3.Scale(baseScale, TemplateScaleOffset); ;

        if (TemplateRotationOffset == Vector3.zero)
          modelTransform.localRotation = Quaternion.identity;
        else
        {
          modelTransform.localEulerAngles = TemplateRotationOffset;
        }
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
        model.Update();
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
      foreach (Transform t in effectTransforms)
      {
        if (state)
        {
          t.localScale = Vector3.Scale(baseScale, TemplateScaleOffset);
        } else
        {
          t.localScale = Vector3.one*0.00001f;
        }
      }
      effectVisible = state;
     
    }
  }

}
