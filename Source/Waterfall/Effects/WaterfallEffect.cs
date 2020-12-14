using System;
using System.Collections.Generic;
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

    public Vector3 TemplatePositionOffset { get; set; }
    public Vector3 TemplateRotationOffset { get; set; }
    public Vector3 TemplateScaleOffset { get; set; }
    public List<Vector3> baseScales;
    public ModuleWaterfallFX parentModule;

    protected WaterfallModel model;
    protected List<EffectModifier> fxModifiers;
    protected Transform parentTransform;
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

    public WaterfallEffect() { }

    public WaterfallEffect(string parent, WaterfallModel mdl)
    {
      parentName = parent;
      model = mdl;
      TemplatePositionOffset = Vector3.zero;
      TemplateRotationOffset = Vector3.zero;
      TemplateScaleOffset = Vector3.one;
      fxModifiers = new List<EffectModifier>();
    }

    public WaterfallEffect(ConfigNode node)
    {
      TemplatePositionOffset = Vector3.zero;
      TemplateRotationOffset = Vector3.zero;
      TemplateScaleOffset = Vector3.one;
      Load(node);
    }

    public WaterfallEffect(ConfigNode node, Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset)
    {
      TemplatePositionOffset = positionOffset;
      TemplateRotationOffset = rotationOffset;
      TemplateScaleOffset = scaleOffset;

      Load(node);
    }

    public WaterfallEffect(WaterfallEffect fx, Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset, string overrideTransformName)
    {
      TemplatePositionOffset = positionOffset;
      TemplateRotationOffset = rotationOffset;
      TemplateScaleOffset = scaleOffset;
      if (overrideTransformName != "")
        fx.savedNode.SetValue("parentName", overrideTransformName, true);
      Load(fx.savedNode);
    }
    public WaterfallEffect(WaterfallEffect fx)
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
      model = new WaterfallModel(node.GetNode(WaterfallConstants.ModelNodeName));
      fxModifiers = new List<EffectModifier>();

      // types
      ConfigNode[] positionNodes = node.GetNodes(WaterfallConstants.PositionModifierNodeName);
      ConfigNode[] rotationNodes = node.GetNodes(WaterfallConstants.RotationModifierNodeName);
      ConfigNode[] scalingNodes = node.GetNodes(WaterfallConstants.ScaleModifierNodeName);
      ConfigNode[] colorNodes = node.GetNodes(WaterfallConstants.ColorModifierNodeName);
      ConfigNode[] uvOffsetNodes = node.GetNodes(WaterfallConstants.UVScrollModifierNodeName);
      ConfigNode[] floatNodes = node.GetNodes(WaterfallConstants.FloatModifierNodeName);

      ConfigNode[] lightColorNodes = node.GetNodes(WaterfallConstants.LightColorNodeName);

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
      foreach (ConfigNode subNode in lightColorNodes)
      {
        fxModifiers.Add(new EffectLightColorModifier(subNode));
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

      
      parentModule = host;
      Transform[] parents = parentModule.part.FindModelTransforms(parentName);
      Utils.Log(String.Format("[WaterfallEffect]: Initializing effect {0} at {1} [{2} instances]", name, parentName, parents.Length), LogType.Effects);
      effectTransforms = new List<Transform>();
      baseScales = new List<Vector3>();
      for (int i = 0; i < parents.Length; i++)
      {
        GameObject effect = new GameObject($"Waterfall_FX_{name}_{i}");
        Transform effectTransform = effect.transform;

        if (parents[i] == null)
        {
          Utils.LogError(String.Format("[WaterfallEffect]: Couldn't find Parent Transform {0} on model to attach effect to", parentName, LogType.Any));
          return;
        }
        effectTransform.SetParent(parents[i], true);
        effectTransform.localPosition = Vector3.zero;
        effectTransform.localEulerAngles = Vector3.zero;

        model.Initialize(effectTransform, fromNothing);


        
        baseScales.Add(effectTransform.localScale);
        Utils.Log($"[WaterfallEffect] local Scale {baseScales[i]}, baseScale, {effectTransform.localScale}", LogType.Effects);

        effectTransform.localPosition = TemplatePositionOffset;
        effectTransform.localEulerAngles = TemplateRotationOffset;
        effectTransform.localScale = Vector3.Scale(baseScales[i], TemplateScaleOffset);

        Utils.Log($"[WaterfallEffect] local Scale {effectTransform.localScale}, baseScale, {baseScales[i]}, {Vector3.Scale(baseScales[i], TemplateScaleOffset)}", LogType.Effects);

        Utils.Log($"[WaterfallEffect] Applied template offsets {TemplatePositionOffset}, {TemplateRotationOffset}, {TemplateScaleOffset}", LogType.Effects);

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

      Utils.Log($"[WaterfallEffect] Applying template offsets from FN2 {position}, {rotation}, {scale}", LogType.Effects);


      for (int i=0; i< effectTransforms.Count;  i++)
      { 




        effectTransforms[i].localPosition = TemplatePositionOffset;
      effectTransforms[i].localScale = Vector3.Scale(baseScales[i], TemplateScaleOffset); 

        if (TemplateRotationOffset == Vector3.zero)
        effectTransforms[i].localRotation = Quaternion.identity;
        else
        {
        effectTransforms[i].localEulerAngles = TemplateRotationOffset;
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
      if (effectTransforms != null)

        for (int i = 0; i < effectTransforms.Count; i++)
        {
          if (state)
        {
            
            effectTransforms[i].localScale = Vector3.Scale(baseScales[i], TemplateScaleOffset);
        } else
        {
            effectTransforms[i].localScale = Vector3.one*0.00001f;
        }
      }
      effectVisible = state;
     
    }
  }

}
