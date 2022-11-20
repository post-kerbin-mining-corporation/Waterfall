using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Waterfall
{
  /// <summary>
  ///   This class represents a single effect. An effect contains a MODEL with various PROPERTIES, modified by EFFECTS
  /// </summary>
  public class WaterfallEffect
  {
    public          string                           name       = "";
    public          string                           parentName = "";
    public readonly List<Vector3>                    baseScales = new();
    public          ModuleWaterfallFX                parentModule;
    public          WaterfallEffectTemplate          parentTemplate;
    public readonly List<EffectFloatIntegrator>      floatIntegrators = new();
    public readonly List<EffectLightFloatIntegrator> lightFloatIntegrators = new();
    public readonly List<EffectLightColorIntegrator> lightColorIntegrators = new();
    public readonly List<EffectPositionIntegrator>   positionIntegrators = new();
    public readonly List<EffectRotationIntegrator>   rotationIntegrators = new();
    public readonly List<EffectScaleIntegrator>      scaleIntegrators = new();
    public readonly List<EffectColorIntegrator>      colorIntegrators = new();

    protected           WaterfallModel       model;
    protected readonly  List<EffectModifier> fxModifiers = new ();
    protected           Transform            parentTransform;
    protected           ConfigNode           savedNode;
    protected           bool                 effectVisible = true;
    protected           Vector3              savedScale;
    protected readonly  List<Transform>      effectTransforms = new();
    private   readonly  List<Material>       effectRendererMaterials = new();
    private   readonly  List<Transform>      effectRendererTransforms = new();
    internal  readonly  List<Renderer>       effectRenderers = new();

    public WaterfallEffect(string parent, WaterfallModel mdl, WaterfallEffectTemplate templateOwner = null)
    {
      if (templateOwner != null)
        parentTemplate = templateOwner;
      parentName             = parent;
      model                  = mdl;
      TemplatePositionOffset = templateOwner != null ? parentTemplate.position : Vector3.zero;
      TemplateRotationOffset = templateOwner != null ? parentTemplate.rotation : Vector3.zero;
      TemplateScaleOffset    = templateOwner != null ? parentTemplate.scale : Vector3.one;
    }

    public WaterfallEffect(ConfigNode node, WaterfallEffectTemplate templateOwner = null)
    {
      if (templateOwner != null)
        parentTemplate = templateOwner;
      TemplatePositionOffset = Vector3.zero;
      TemplateRotationOffset = Vector3.zero;
      TemplateScaleOffset    = Vector3.one;
      Load(node);
    }

    public WaterfallEffect(WaterfallEffect fx, WaterfallEffectTemplate templateOwner)
    {
      parentTemplate         = templateOwner;
      TemplatePositionOffset = parentTemplate.position;
      TemplateRotationOffset = parentTemplate.rotation;
      TemplateScaleOffset    = parentTemplate.scale;

      if (parentTemplate.overrideParentTransform != "" && parentTemplate.overrideParentTransform != null)
        fx.savedNode.SetValue("parentName", parentTemplate.overrideParentTransform, true);

      Load(fx.savedNode);
    }

    public WaterfallEffect(WaterfallEffect fx)
    {
      parentTemplate         = fx.parentTemplate;
      TemplatePositionOffset = fx.TemplatePositionOffset;
      TemplateRotationOffset = fx.TemplateRotationOffset;
      TemplateScaleOffset    = fx.TemplateScaleOffset;
      Load(fx.Save());
    }

    public Vector3 TemplatePositionOffset { get; set; }
    public Vector3 TemplateRotationOffset { get; set; }
    public Vector3 TemplateScaleOffset    { get; set; }

    public List<EffectModifier> FXModifiers => fxModifiers;

    public WaterfallModel FXModel => model;

    public ConfigNode SavedNode => savedNode;

    /// <summary>
    ///   Load the node from config
    /// </summary>
    /// <param name="node"></param>
    public void Load(ConfigNode node)
    {
      savedNode = node;
      node.TryGetValue(nameof(name), ref name);

      if (!node.TryGetValue(nameof(parentName), ref parentName))
      {
        Utils.LogError(String.Format("[WaterfallEffect]: EFFECT with name {0} does not define parentName, which is required", name));
        return;
      }

      model       = new(node.GetNode(WaterfallConstants.ModelNodeName));
      fxModifiers.Clear();

      // types
      var positionNodes = node.GetNodes(WaterfallConstants.PositionModifierNodeName);
      var rotationNodes = node.GetNodes(WaterfallConstants.RotationModifierNodeName);
      var scalingNodes  = node.GetNodes(WaterfallConstants.ScaleModifierNodeName);
      var colorNodes    = node.GetNodes(WaterfallConstants.ColorModifierNodeName);
      var uvOffsetNodes = node.GetNodes(WaterfallConstants.UVScrollModifierNodeName);
      var floatNodes    = node.GetNodes(WaterfallConstants.FloatModifierNodeName);

      var colorLightNodes = node.GetNodes(WaterfallConstants.ColorFromLightNodeName);

      var lightFloatNodes = node.GetNodes(WaterfallConstants.LightFloatModifierNodeName);
      var lightColorNodes = node.GetNodes(WaterfallConstants.LightColorModifierNodeName);

      for (int i = 0; i < positionNodes.Length; i++)
      {
        var subNode = positionNodes[i];
        fxModifiers.Add(new EffectPositionModifier(subNode));
      }

      for (int i = 0; i < rotationNodes.Length; i++)
      {
        var subNode = rotationNodes[i];
        fxModifiers.Add(new EffectRotationModifier(subNode));
      }

      for (int i = 0; i < scalingNodes.Length; i++)
      {
        var subNode = scalingNodes[i];
        fxModifiers.Add(new EffectScaleModifier(subNode));
      }

      for (int i = 0; i < colorNodes.Length; i++)
      {
        var subNode = colorNodes[i];
        fxModifiers.Add(new EffectColorModifier(subNode));
      }

      for (int i = 0; i < uvOffsetNodes.Length; i++)
      {
        var subNode = uvOffsetNodes[i];
        fxModifiers.Add(new EffectUVScrollModifier(subNode));
      }

      for (int i = 0; i < floatNodes.Length; i++)
      {
        var subNode = floatNodes[i];
        fxModifiers.Add(new EffectFloatModifier(subNode));
      }

      for (int i = 0; i < colorLightNodes.Length; i++)
      {
        var subNode = colorLightNodes[i];
        fxModifiers.Add(new EffectColorFromLightModifier(subNode));
      }

      for (int i = 0; i < lightFloatNodes.Length; i++)
      {
        var subNode = lightFloatNodes[i];
        fxModifiers.Add(new EffectLightFloatModifier(subNode));
      }

      for (int i = 0; i < lightColorNodes.Length; i++)
      {
        var subNode = lightColorNodes[i];
        fxModifiers.Add(new EffectLightColorModifier(subNode));
      }
    }

    public ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.EffectNodeName;
      node.AddValue("name",       name);
      node.AddValue("parentName", parentName);
      node.AddNode(model.Save());
      foreach (var fx in fxModifiers)
      {
        node.AddNode(fx.Save());
      }

      return node;
    }

    public void CleanupEffect()
    {
      Utils.Log($"[WaterfallEffect]: Deleting effect {name}", LogType.Effects);
      for (int i = model.modelTransforms.Count - 1; i >= 0; i--)
      {
        Object.Destroy(model.modelTransforms[i].gameObject);
      }
    }

    public void InitializeEffect(ModuleWaterfallFX host, bool fromNothing, bool useRelativeScaling)
    {
      parentModule = host;
      var parents = parentModule.part.FindModelTransforms(parentName);
      Utils.Log($"[WaterfallEffect]: Initializing effect {name} at {parentName} [{parents.Length} instances]; relative scaling: {useRelativeScaling}", LogType.Effects);

      effectTransforms.Clear();
      baseScales.Clear();

      for (int i = 0; i < parents.Length; i++)
      {
        var effect          = new GameObject($"Waterfall_FX_{name}_{i}");
        var effectTransform = effect.transform;

        if (parents[i] == null)
        {
          Utils.LogError($"[WaterfallEffect]: Trying to attach effect to null parent transform {parentName} on model");
          continue;
        }

        effectTransform.SetParent(parents[i], true);
        effectTransform.localPosition    = Vector3.zero;
        effectTransform.localEulerAngles = Vector3.zero;
        if (useRelativeScaling)
          effectTransform.localScale = Vector3.one;

        model.Initialize(effectTransform, fromNothing);

        baseScales.Add(effectTransform.localScale);
        Utils.Log($"[WaterfallEffect] Scale: {effectTransform.localScale}", LogType.Effects);

        effectTransform.localPosition    = TemplatePositionOffset;
        effectTransform.localEulerAngles = TemplateRotationOffset;
        effectTransform.localScale       = Vector3.Scale(baseScales[i], TemplateScaleOffset);

        Utils.Log($"[WaterfallEffect] local Scale {effectTransform.localScale}, baseScale, {baseScales[i]}, {Vector3.Scale(baseScales[i], TemplateScaleOffset)}", LogType.Effects);

        Utils.Log($"[WaterfallEffect] Applied template offsets {TemplatePositionOffset}, {TemplateRotationOffset}, {TemplateScaleOffset}", LogType.Effects);

        effectTransforms.Add(effectTransform);
      }

      for (int j = 0; j < fxModifiers.Count; j++)
      {
        var fx = fxModifiers[j];
        fx.Init(this);
      }

      effectRenderers.Clear();
      effectRendererMaterials.Clear();
      effectRendererTransforms.Clear();
      for (int transformIndex = 0; transformIndex < model.modelTransforms.Count; transformIndex++)
      {
        var t  = model.modelTransforms[transformIndex];
        var rs = t.GetComponentsInChildren<Renderer>();
        for (int rendererIndex = 0; rendererIndex < rs.Length; rendererIndex++)
        {
          var r = rs[rendererIndex];
          effectRenderers.Add(r);
          effectRendererMaterials.Add(r.material);
          effectRendererTransforms.Add(r.transform);
        }
      }

      InitializeIntegrators();
    }

    public void InitializeIntegrators()
    {
      floatIntegrators.Clear();
      positionIntegrators.Clear();
      colorIntegrators.Clear();
      rotationIntegrators.Clear();
      scaleIntegrators.Clear();
      lightFloatIntegrators.Clear();
      lightColorIntegrators.Clear();

      for (int i = 0; i < fxModifiers.Count; i++)
      {
        var mod = fxModifiers[i];
        if (mod is EffectFloatModifier)
          mod.CreateOrAttachToIntegrator(floatIntegrators);
        else if (mod is EffectColorModifier)
          mod.CreateOrAttachToIntegrator(colorIntegrators);
        else if (mod is EffectPositionModifier)
          mod.CreateOrAttachToIntegrator(positionIntegrators);
        else if (mod is EffectRotationModifier)
          mod.CreateOrAttachToIntegrator(rotationIntegrators);
        else if (mod is EffectScaleModifier)
          mod.CreateOrAttachToIntegrator(scaleIntegrators);
        else if (mod is EffectLightFloatModifier)
          mod.CreateOrAttachToIntegrator(lightFloatIntegrators);
        else if (mod is EffectLightColorModifier)
          mod.CreateOrAttachToIntegrator(lightColorIntegrators);
      }
    }

    public void ApplyTemplateOffsets(Vector3 position, Vector3 rotation, Vector3 scale)
    {
      TemplatePositionOffset = position;
      TemplateRotationOffset = rotation;
      TemplateScaleOffset    = scale;

      Utils.Log($"[WaterfallEffect] Applying template offsets from FN2 {position}, {rotation}, {scale}", LogType.Effects);


      for (int i = 0; i < effectTransforms.Count; i++)
      {
        effectTransforms[i].localPosition = TemplatePositionOffset;
        effectTransforms[i].localScale    = Vector3.Scale(baseScales[i], TemplateScaleOffset);

        if (TemplateRotationOffset == Vector3.zero)
        {
          effectTransforms[i].localRotation = Quaternion.identity;
        }
        else
        {
          effectTransforms[i].localEulerAngles = TemplateRotationOffset;
        }
      }
    }

    public List<Transform> GetModelTransforms() => model.modelTransforms;

    private static readonly ProfilerMarker s_Update = new ProfilerMarker("Waterfall.Effect.Update");
    private static readonly ProfilerMarker s_fxApply = new ProfilerMarker("Waterfall.Effect.Update.FxApply");
    private static readonly ProfilerMarker s_Integrators = new ProfilerMarker("Waterfall.Effect.Update.Integrators");

    private static readonly float[] EmptyControllerValues = new float[1];

    public void Update()
    {
      s_Update.Begin();
      if (effectVisible)
      {
        s_fxApply.Begin();
        for (int i = 0; i < fxModifiers.Count; i++)
        {
          var     fx             = fxModifiers[i];
          float[] controllerData = fx.Controller == null ? EmptyControllerValues : fx.Controller.Get();
          fx.Apply(controllerData);
        }

        s_fxApply.End();

        s_Integrators.Begin();
        for (int i = 0; i < floatIntegrators.Count; i++)
        {
          var integrator = floatIntegrators[i];
          integrator.Update();
        }

        for (int i = 0; i < colorIntegrators.Count; i++)
        {
          var integrator = colorIntegrators[i];
          integrator.Update();
        }

        for (int i = 0; i < positionIntegrators.Count; i++)
        {
          var integrator = positionIntegrators[i];
          integrator.Update();
        }

        for (int i = 0; i < scaleIntegrators.Count; i++)
        {
          var integrator = scaleIntegrators[i];
          integrator.Update();
        }

        for (int i = 0; i < rotationIntegrators.Count; i++)
        {
          var integrator = rotationIntegrators[i];
          integrator.Update();
        }

        for (int i = 0; i < lightFloatIntegrators.Count; i++)
        {
          var integrator = lightFloatIntegrators[i];
          integrator.Update();
        }

        for (int i = 0; i < lightColorIntegrators.Count; i++)
        {
          var integrator = lightColorIntegrators[i];
          integrator.Update();
        }

        s_Integrators.End();
      }
      s_Update.End();
    }

    private static readonly ProfilerMarker camerasProf = new("Waterfall.Effect.Update.Cameras");
    public static void SetupRenderersForCamera(Camera camera, List<Renderer> renderers)
    {
      camerasProf.Begin();
      var c = camera.transform;
      for (int i = 0; i < renderers.Count; i++)
      {
        var renderer = renderers[i];
        if (!renderer.enabled)
          continue;
        Material mat = renderer.material;

        int qDelta;
        if (mat.HasProperty("_Strength"))
          qDelta = Settings.DistortQueue;
        else
        {
          float camDistBounds    = Vector3.Dot(renderer.bounds.center      - c.position, c.forward);
          float camDistTransform = Vector3.Dot(renderer.transform.position - c.position, c.forward);
          qDelta = Settings.QueueDepth - (int) Mathf.Clamp(Mathf.Min(camDistBounds, camDistTransform) / Settings.SortedDepth * Settings.QueueDepth, 0, Settings.QueueDepth);
        }

        if (mat.HasProperty("_Intensity"))
          qDelta += 1;
        mat.renderQueue = Settings.TransparentQueueBase + qDelta;
      }

      camerasProf.End();
    }

    public void SetHDR(bool isHDR)
    {
      float destMode = Settings.EnableLegacyBlendModes ? 6 : 1;

      for (int i = 0; i < effectRendererMaterials.Count; i++)
      {
        var mat = effectRendererMaterials[i];
        if (mat.HasProperty("_DestMode"))
        {
          mat.SetFloat("_DestMode",       isHDR ? 1 : destMode);
          mat.SetFloat("_ClipBrightness", isHDR ? 50 : 1);
        }
      }
    }

    public void RemoveModifier(EffectModifier mod)
    {
      fxModifiers.Remove(mod);
      if (mod is EffectFloatModifier) mod.RemoveFromIntegrator(floatIntegrators);
      else if (mod is EffectColorModifier) mod.RemoveFromIntegrator(colorIntegrators);
      else if (mod is EffectPositionModifier) mod.RemoveFromIntegrator(positionIntegrators);
      else if (mod is EffectRotationModifier) mod.RemoveFromIntegrator(rotationIntegrators);
      else if (mod is EffectScaleModifier) mod.RemoveFromIntegrator(scaleIntegrators);
      else if (mod is EffectLightFloatModifier) mod.RemoveFromIntegrator(lightFloatIntegrators);
      else if (mod is EffectLightColorModifier) mod.RemoveFromIntegrator(lightColorIntegrators);
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
      if (mod is EffectFloatModifier) mod.CreateOrAttachToIntegrator(floatIntegrators);
      else if (mod is EffectColorModifier) mod.CreateOrAttachToIntegrator(colorIntegrators);
      else if (mod is EffectPositionModifier) mod.CreateOrAttachToIntegrator(positionIntegrators);
      else if (mod is EffectRotationModifier) mod.CreateOrAttachToIntegrator(rotationIntegrators);
      else if (mod is EffectScaleModifier) mod.CreateOrAttachToIntegrator(scaleIntegrators);
      else if (mod is EffectLightFloatModifier) mod.CreateOrAttachToIntegrator(lightFloatIntegrators);
      else if (mod is EffectLightColorModifier) mod.CreateOrAttachToIntegrator(lightColorIntegrators);
    }

    public void MoveModifierFromTo(int oldIndex, int newIndex)
    {
      oldIndex = Mathf.Clamp(oldIndex, 0, fxModifiers.Count - 1);
      newIndex = Mathf.Clamp(newIndex, 0, fxModifiers.Count - 1);

      var item = fxModifiers[oldIndex];
      fxModifiers.RemoveAt(oldIndex);
      fxModifiers.Insert(newIndex, item);

      InitializeIntegrators();
    }

    public void MoveModifierUp(int index) => MoveModifierFromTo(index, index - 1);
    public void MoveModifierDown(int index) => MoveModifierFromTo(index, index + 1);

    public void SetEnabled(bool state)
    {
      for (int i = 0; i < effectTransforms.Count; i++)
        effectTransforms[i].localScale = state ? Vector3.Scale(baseScales[i], TemplateScaleOffset) : Vector3.one * 0.00001f;
      effectVisible = state;

      for (int i = 0; i < effectRenderers.Count; i++)
      {
        var r = effectRenderers[i];
        r.enabled = state;
      }

      for (int i = 0; i < model.lights.Count; i++)
      {
        var waterfallLight = model.lights[i];
        for (int j = 0; j < waterfallLight.lights.Count; j++)
        {
          var light = waterfallLight.lights[j];
          light.enabled = state;
        }
      }
    }
  }
}
