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
    public WaterfallEffectTemplate parentTemplate;

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

    public WaterfallEffect(string parent, WaterfallModel mdl, WaterfallEffectTemplate templateOwner)
    {
      parentName = parent;
      model = mdl;

      parentTemplate = templateOwner;
      TemplatePositionOffset = parentTemplate.position;
      TemplateRotationOffset = parentTemplate.rotation;
      TemplateScaleOffset = parentTemplate.scale;
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
    public WaterfallEffect(WaterfallEffect fx, WaterfallEffectTemplate templateOwner)
    {
      parentTemplate = templateOwner;
      TemplatePositionOffset = parentTemplate.position;
      TemplateRotationOffset = parentTemplate.rotation;
      TemplateScaleOffset = parentTemplate.scale;

      if (parentTemplate.overrideParentTransform != "" && parentTemplate.overrideParentTransform != null)
        fx.savedNode.SetValue("parentName", parentTemplate.overrideParentTransform, true);

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

      ConfigNode[] colorLightNodes = node.GetNodes(WaterfallConstants.ColorFromLightNodeName);

      ConfigNode[] lightFloatNodes = node.GetNodes(WaterfallConstants.LightFloatModifierNodeName);
      ConfigNode[] lightColorNodes = node.GetNodes(WaterfallConstants.LightColorModifierNodeName);

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
      foreach (ConfigNode subNode in colorLightNodes)
      {
        fxModifiers.Add(new EffectColorFromLightModifier(subNode));
      }
      foreach (ConfigNode subNode in lightFloatNodes)
      {
        fxModifiers.Add(new EffectLightFloatModifier(subNode));
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
      for (int i = model.modelTransforms.Count - 1; i >= 0; i--)

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

      effectRendererMaterials = new List<Material>();
      effectRendererTransforms = new List<Transform>();
      effectRenderers = new List<Renderer>();
      foreach (Transform t in model.modelTransforms)
      {
        Renderer[] renderers = t.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
          effectRenderers.Add(r);
          effectRendererMaterials.Add(r.material);
          effectRendererTransforms.Add(r.transform);
        }
      }
      InitializeIntegrators();
    }
    List<Material> effectRendererMaterials;
    List<Transform> effectRendererTransforms;
    List<Renderer> effectRenderers;
    public List<EffectFloatIntegrator> floatIntegrators;
    public List<EffectLightFloatIntegrator> lightFloatIntegrators;
    public List<EffectLightColorIntegrator> lightColorIntegrators;
    public List<EffectPositionIntegrator> positionIntegrators;
    public List<EffectRotationIntegrator> rotationIntegrators;
    public List<EffectScaleIntegrator> scaleIntegrators;
    public List<EffectColorIntegrator> colorIntegrators;

    public void InitializeIntegrators()
    {
      floatIntegrators = new List<EffectFloatIntegrator>();
      positionIntegrators = new List<EffectPositionIntegrator>();
      colorIntegrators = new List<EffectColorIntegrator>();
      rotationIntegrators = new List<EffectRotationIntegrator>();
      scaleIntegrators = new List<EffectScaleIntegrator>();
      lightFloatIntegrators = new List<EffectLightFloatIntegrator>();
      lightColorIntegrators = new List<EffectLightColorIntegrator>();

      foreach (EffectModifier fxMod in fxModifiers)
      {
        ParseFloatModifier(fxMod);
        ParseColorModifier(fxMod);
        ParsePositionModifier(fxMod);
        ParseRotationModifier(fxMod);
        ParseScaleModifier(fxMod);
        ParseLightFloatModifier(fxMod);
        ParseLightColorModifier(fxMod);
      }
    }

    void ParseFloatModifier(EffectModifier fxMod)
    {
      try
      {
        EffectFloatModifier floatMod = (EffectFloatModifier)fxMod;
        if (floatMod != null)
        {
          bool needsNewIntegrator = true;
          EffectFloatIntegrator targetIntegrator = null;

          foreach (EffectFloatIntegrator floatInt in floatIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (floatInt.handledModifiers.Contains(floatMod))
              return;

            // if there's already an integrator that has the transform name and float name, don't need to add
            if (floatInt.floatName == floatMod.floatName && floatInt.transformName == floatMod.transformName)
            {
              targetIntegrator = floatInt;
              needsNewIntegrator = false;
            }

          }
          if (needsNewIntegrator && floatMod.floatName != "")
          {
            EffectFloatIntegrator newIntegrator = new EffectFloatIntegrator(this, floatMod);
            floatIntegrators.Add(newIntegrator);
          }
          else if (!needsNewIntegrator && floatMod.floatName != "")
          {
            if (targetIntegrator != null)
            {
              targetIntegrator.AddModifier(floatMod);
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void ParseColorModifier(EffectModifier fxMod)
    {
      try
      {
        EffectColorModifier colorMod = (EffectColorModifier)fxMod;
        if (colorMod != null)
        {
          bool needsNewIntegrator = true;
          EffectColorIntegrator targetIntegrator = null;

          foreach (EffectColorIntegrator integrator in colorIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(colorMod))
              return;

            // if there's already an integrator that has the transform name and float name, don't need to add
            if (integrator.colorName == colorMod.colorName && integrator.transformName == colorMod.transformName)
            {
              targetIntegrator = integrator;
              needsNewIntegrator = false;
            }

          }
          if (needsNewIntegrator && colorMod.colorName != "")
          {
            EffectColorIntegrator newIntegrator = new EffectColorIntegrator(this, colorMod);
            colorIntegrators.Add(newIntegrator);
          }
          else if (!needsNewIntegrator && colorMod.colorName != "")
          {
            if (targetIntegrator != null)
            {
              targetIntegrator.AddModifier(colorMod);
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void ParsePositionModifier(EffectModifier fxMod)
    {
      try
      {
        EffectPositionModifier posMod = (EffectPositionModifier)fxMod;
        if (posMod != null)
        {
          bool needsNewIntegrator = true;
          EffectPositionIntegrator targetIntegrator = null;

          foreach (EffectPositionIntegrator integrator in positionIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(posMod))
              return;

            // if there's already an integrator that has the transform name and float name, don't need to add
            if (integrator.transformName == posMod.transformName)
            {
              targetIntegrator = integrator;
              needsNewIntegrator = false;
            }

          }
          if (needsNewIntegrator)
          {
            EffectPositionIntegrator newIntegrator = new EffectPositionIntegrator(this, posMod);
            positionIntegrators.Add(newIntegrator);
          }
          else if (!needsNewIntegrator)
          {
            if (targetIntegrator != null)
            {
              targetIntegrator.AddModifier(posMod);
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void ParseScaleModifier(EffectModifier fxMod)
    {
      try
      {
        EffectScaleModifier scaleMod = (EffectScaleModifier)fxMod;
        if (scaleMod != null)
        {
          bool needsNewIntegrator = true;
          EffectScaleIntegrator targetIntegrator = null;

          foreach (EffectScaleIntegrator integrator in scaleIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(scaleMod))
              return;

            // if there's already an integrator that has the transform name and float name, don't need to add
            if (integrator.transformName == scaleMod.transformName)
            {
              targetIntegrator = integrator;
              needsNewIntegrator = false;
            }

          }
          if (needsNewIntegrator)
          {
            EffectScaleIntegrator newIntegrator = new EffectScaleIntegrator(this, scaleMod);
            scaleIntegrators.Add(newIntegrator);
          }
          else if (!needsNewIntegrator)
          {
            if (targetIntegrator != null)
            {
              targetIntegrator.AddModifier(scaleMod);
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }
    void ParseRotationModifier(EffectModifier fxMod)
    {
      try
      {
        EffectRotationModifier rotMod = (EffectRotationModifier)fxMod;
        if (rotMod != null)
        {
          bool needsNewIntegrator = true;
          EffectRotationIntegrator targetIntegrator = null;

          foreach (EffectRotationIntegrator integrator in rotationIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(rotMod))
              return;

            // if there's already an integrator that has the transform name and float name, don't need to add
            if (integrator.transformName == rotMod.transformName)
            {
              targetIntegrator = integrator;
              needsNewIntegrator = false;
            }

          }
          if (needsNewIntegrator)
          {
            EffectRotationIntegrator newIntegrator = new EffectRotationIntegrator(this, rotMod);
            rotationIntegrators.Add(newIntegrator);
          }
          else if (!needsNewIntegrator)
          {
            if (targetIntegrator != null)
            {
              targetIntegrator.AddModifier(rotMod);
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void ParseLightFloatModifier(EffectModifier fxMod)
    {
      try
      {
        EffectLightFloatModifier floatMod = (EffectLightFloatModifier)fxMod;
        if (floatMod != null)
        {
          bool needsNewIntegrator = true;
          EffectLightFloatIntegrator targetIntegrator = null;

          foreach (EffectLightFloatIntegrator floatInt in lightFloatIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (floatInt.handledModifiers.Contains(floatMod))
              return;

            // if there's already an integrator that has the transform name and float name, don't need to add
            if (floatInt.floatName == floatMod.floatName && floatInt.transformName == floatMod.transformName)
            {
              targetIntegrator = floatInt;
              needsNewIntegrator = false;
            }

          }
          if (needsNewIntegrator && floatMod.floatName != "")
          {
            EffectLightFloatIntegrator newIntegrator = new EffectLightFloatIntegrator(this, floatMod);
            lightFloatIntegrators.Add(newIntegrator);
          }
          else if (!needsNewIntegrator && floatMod.floatName != "")
          {
            if (targetIntegrator != null)
            {
              targetIntegrator.AddModifier(floatMod);
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void ParseLightColorModifier(EffectModifier fxMod)
    {
      try
      {
        EffectLightColorModifier colorMod = (EffectLightColorModifier)fxMod;
        if (colorMod != null)
        {
          bool needsNewIntegrator = true;
          EffectLightColorIntegrator targetIntegrator = null;

          foreach (EffectLightColorIntegrator integrator in lightColorIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(colorMod))
              return;

            // if there's already an integrator that has the transform name and float name, don't need to add
            if (integrator.colorName == colorMod.colorName && integrator.transformName == colorMod.transformName)
            {
              targetIntegrator = integrator;
              needsNewIntegrator = false;
            }

          }
          if (needsNewIntegrator && colorMod.colorName != "")
          {
            EffectLightColorIntegrator newIntegrator = new EffectLightColorIntegrator(this, colorMod);
            lightColorIntegrators.Add(newIntegrator);
          }
          else if (!needsNewIntegrator && colorMod.colorName != "")
          {
            if (targetIntegrator != null)
            {
              targetIntegrator.AddModifier(colorMod);
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void RemoveFloatModifier(EffectModifier fxMod)
    {
      try
      {
        EffectFloatModifier floatMod = (EffectFloatModifier)fxMod;
        if (floatMod != null)
        {
          foreach (EffectFloatIntegrator floatInt in floatIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (floatInt.handledModifiers.Contains(floatMod))
            {
              floatInt.RemoveModifier(floatMod);
              return;
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void RemoveColorModifier(EffectModifier fxMod)
    {
      try
      {
        EffectColorModifier mod = (EffectColorModifier)fxMod;
        if (mod != null)
        {
          foreach (EffectColorIntegrator integrator in colorIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(mod))
            {
              integrator.RemoveModifier(mod);
              return;
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void RemovePositionModifier(EffectModifier fxMod)
    {
      try
      {
        EffectPositionModifier posMod = (EffectPositionModifier)fxMod;
        if (posMod != null)
        {
          foreach (EffectPositionIntegrator integrator in positionIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(posMod))
            {
              integrator.RemoveModifier(posMod);
              return;
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }
    void RemoveScaleModifier(EffectModifier fxMod)
    {
      try
      {
        EffectScaleModifier mod = (EffectScaleModifier)fxMod;
        if (mod != null)
        {
          foreach (EffectScaleIntegrator integrator in scaleIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(mod))
            {
              integrator.RemoveModifier(mod);
              return;
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void RemoveRotationModifier(EffectModifier fxMod)
    {
      try
      {
        EffectRotationModifier mod = (EffectRotationModifier)fxMod;
        if (mod != null)
        {
          foreach (EffectRotationIntegrator integrator in rotationIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(mod))
            {
              integrator.RemoveModifier(mod);
              return;
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void RemoveLightFloatModifier(EffectModifier fxMod)
    {
      try
      {
        EffectLightFloatModifier floatMod = (EffectLightFloatModifier)fxMod;
        if (floatMod != null)
        {
          foreach (EffectLightFloatIntegrator floatInt in lightFloatIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (floatInt.handledModifiers.Contains(floatMod))
            {
              floatInt.RemoveModifier(floatMod);
              return;
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    void RemoveLightColorModifier(EffectModifier fxMod)
    {
      try
      {
        EffectLightColorModifier mod = (EffectLightColorModifier)fxMod;
        if (mod != null)
        {
          foreach (EffectLightColorIntegrator integrator in lightColorIntegrators)
          {
            // If already exists as a handled modifier, don't touch me
            if (integrator.handledModifiers.Contains(mod))
            {
              integrator.RemoveModifier(mod);
              return;
            }
          }
        }
      }
      catch (InvalidCastException e) { }
    }

    public void ApplyTemplateOffsets(Vector3 position, Vector3 rotation, Vector3 scale)
    {
      TemplatePositionOffset = position;
      TemplateRotationOffset = rotation;
      TemplateScaleOffset = scale;

      Utils.Log($"[WaterfallEffect] Applying template offsets from FN2 {position}, {rotation}, {scale}", LogType.Effects);


      for (int i = 0; i < effectTransforms.Count; i++)
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

        for (int i = 0; i < floatIntegrators.Count; i++)
        {
          floatIntegrators[i].Update();
        }
        for (int i = 0; i < colorIntegrators.Count; i++)
        {
          colorIntegrators[i].Update();
        }
        for (int i = 0; i < positionIntegrators.Count; i++)
        {
          positionIntegrators[i].Update();
        }
        for (int i = 0; i < scaleIntegrators.Count; i++)
        {
          scaleIntegrators[i].Update();
        }
        for (int i = 0; i < rotationIntegrators.Count; i++)
        { 
          rotationIntegrators[i].Update();
        }
        for (int i = 0; i < lightFloatIntegrators.Count; i++)
        {
          lightFloatIntegrators[i].Update();
        }
        for (int i = 0; i < lightColorIntegrators.Count; i++)
        {
          lightColorIntegrators[i].Update();
        }


        int transparentQueueBase = 3000;
        
        int queueDepth = 750;
        float sortedDepth = 1000f;
        int distortQueue= transparentQueueBase + 2;

        Transform c = FlightCamera.fetch.cameras[0].transform;
        for (int i = 0; i < effectRendererMaterials.Count; i++)
        {
          float camDistBounds = Vector3.Dot(effectRenderers[i].bounds.center - c.position, c.forward);
          float camDistTransform = Vector3.Dot(effectRenderers[i].transform.position - c.position, c.forward);

          int qDelta = queueDepth - (int)Mathf.Clamp((Mathf.Min(camDistBounds,camDistTransform) / sortedDepth) * queueDepth, 0, queueDepth);
          if (effectRendererMaterials[i].HasProperty("_Strength"))
            qDelta = distortQueue;
          if (effectRendererMaterials[i].HasProperty("_Intensity"))
            qDelta += 1;
          effectRendererMaterials[i].renderQueue = transparentQueueBase + qDelta;
        }
      }
    }

    public void SetHDR(bool isHDR)
    {
      for (int i = 0; i < effectRendererMaterials.Count; i++)
      {
        if (effectRendererMaterials[i].HasProperty("_DestMode"))
          if (isHDR)
          {
            effectRendererMaterials[i].SetFloat("_DestMode", 1f);
            effectRendererMaterials[i].SetFloat("_ClipBrightness", 50f);
          }
          else
          {
            effectRendererMaterials[i].SetFloat("_DestMode", 6f);
            effectRendererMaterials[i].SetFloat("_ClipBrightness", 1f);
          }
      }

    }
    public void RemoveModifier(EffectModifier mod)
    {
      fxModifiers.Remove(mod);

      RemoveFloatModifier(mod);
      RemoveColorModifier(mod);
      RemovePositionModifier(mod);
      RemoveRotationModifier(mod);
      RemoveScaleModifier(mod);
      RemoveLightFloatModifier(mod);
      RemoveLightColorModifier(mod);
    }

    public void ModifierParameterChange(EffectModifier mod)
    {
      RemoveModifier(mod);
      AddModifier(mod);
    }
    public void AddModifier(EffectModifier mod)
    {
      mod.Init(this);
      fxModifiers.Add(mod);
      ParseColorModifier(mod);
      ParseFloatModifier(mod);
      ParsePositionModifier(mod);
      ParseRotationModifier(mod);
      ParseScaleModifier(mod);
      ParseLightFloatModifier(mod);
      ParseLightColorModifier(mod);
    }

    public void MoveModifierUp(int index)
    {
      int newIndex = Mathf.Clamp(index - 1, 0, 5000);

      var item = fxModifiers[index];

      fxModifiers.RemoveAt(index);

      if (newIndex > index) newIndex--;
      // the actual index could have shifted due to the removal

      fxModifiers.Insert(newIndex, item);

      InitializeIntegrators();
    }

    public void MoveModifierDown(int index)
    {
      int newIndex = Mathf.Clamp(index + 1, 0, fxModifiers.Count - 1);

      var item = fxModifiers[index];

      fxModifiers.RemoveAt(index);

      //if (newIndex > i) newIndex--;
      // the actual index could have shifted due to the removal

      fxModifiers.Insert(newIndex, item);

      InitializeIntegrators();

    }


    public void SetEnabled(bool state)
    {
      if (effectTransforms != null)

        for (int i = 0; i < effectTransforms.Count; i++)
        {
          if (state)
          {

            effectTransforms[i].localScale = Vector3.Scale(baseScales[i], TemplateScaleOffset);
          }
          else
          {
            effectTransforms[i].localScale = Vector3.one * 0.00001f;
          }
        }
      effectVisible = state;

    }
  }

}
