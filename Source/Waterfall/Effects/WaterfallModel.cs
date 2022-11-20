using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Waterfall
{
  public class WaterfallModel
  {
    public string path;
    public string positionOffsetString;
    public string rotationOffestString;
    public string scaleOffsetString;

    public string                  overrideShader = "";
    public List<WaterfallMaterial> materials;
    public List<WaterfallLight>    lights;


    public List<Transform> modelTransforms;
    public Vector3         modelPositionOffset = Vector3.zero;
    public Vector3         modelRotationOffset = Vector3.zero;
    public Vector3         modelScaleOffset    = Vector3.one;

    protected bool randomized = true;

    public WaterfallModel()
    {
      modelTransforms = new();
    }

    public WaterfallModel(string modelPath, string shader, bool randomizeSeed)
    {
      modelTransforms = new();
      path            = modelPath;
      overrideShader  = shader;
      randomized      = randomizeSeed;
    }

    public WaterfallModel(WaterfallAsset modelAsset, WaterfallAsset shaderAsset, bool randomizeSeed)
    {
      modelTransforms = new();
      path            = modelAsset.Path;
      if (shaderAsset == null)
        overrideShader = null;
      else
        overrideShader = shaderAsset.Name;
      randomized = randomizeSeed;
    }

    public WaterfallModel(ConfigNode node) : this()
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      if (!node.TryGetValue("path", ref path))
        Utils.LogError("[WaterfallModel]: Unabled to find required path string in MODEL node");
      node.TryGetValue("positionOffset", ref modelPositionOffset);
      node.TryGetValue("rotationOffset", ref modelRotationOffset);
      node.TryGetValue("scaleOffset",    ref modelScaleOffset);

      materials = new();
      foreach (var materialNode in node.GetNodes(WaterfallConstants.MaterialNodeName))
      {
        materials.Add(new(materialNode));
      }

      lights = new();
      foreach (var lightNode in node.GetNodes(WaterfallConstants.LightNodeName))
      {
        lights.Add(new(lightNode));
      }
    }

    public ConfigNode Save()
    {
      var node = new ConfigNode();
      node.name = WaterfallConstants.ModelNodeName;
      node.AddValue("path",           path);
      node.AddValue("positionOffset", modelPositionOffset);
      node.AddValue("rotationOffset", modelRotationOffset);
      node.AddValue("scaleOffset",    modelScaleOffset);
      foreach (var m in materials)
      {
        node.AddNode(m.Save());
      }

      foreach (var l in lights)
      {
        node.AddNode(l.Save());
      }

      return node;
    }


    public void Initialize(Transform parent, bool fromNothing)
    {
      Utils.Log(String.Format("[WaterfallModel]: Instantiating model from {0} ", path), LogType.Effects);
      if (!GameDatabase.Instance.ExistsModel(path))
        Utils.LogError(String.Format("[WaterfallModel]: Unabled to find model {0} in GameDatabase", path));
      var inst = Object.Instantiate(GameDatabase.Instance.GetModelPrefab(path), parent.position, parent.rotation);
      inst.SetLayerRecursive(1);
      inst.SetActive(true);

      var modelTransform = inst.GetComponent<Transform>();

      modelTransform.SetParent(parent, true);

      //modelTransform.localScale = modelScaleOffset;
      //modelTransform.localPosition = modelPositionOffset;

      //if (modelRotationOffset == Vector3.zero)
      //  modelTransform.localRotation = Quaternion.identity;
      //else
      //  modelTransform.localEulerAngles = modelRotationOffset;
      //Utils.Log(String.Format("[WaterfallModel]: Instantiated model at {0} with {1}, {2}", modelPositionOffset, modelRotationOffset, modelScaleOffset));

      modelTransforms.Add(modelTransform);

      var renderers = modelTransform.GetComponentsInChildren<Renderer>();
      var lightObjs = modelTransform.GetComponentsInChildren<Light>();

      if (fromNothing)
      {
        materials = new();
        lights    = new();

        if (lightObjs.Length > 0)
        {
          var l = new WaterfallLight();
          l.lights        = new();
          l.transformName = lightObjs[0].name;
          lights.Add(l);
        }

        foreach (var r in renderers)
        {
          var m = new WaterfallMaterial();
          if (overrideShader != null)
            m.shaderName = overrideShader;
          m.useAutoRandomization = randomized;
          m.materials            = new();
          m.transformName        = r.transform.name;
          materials.Add(m);
        }
      }

      foreach (var m in materials)
      {
        m.useAutoRandomization = randomized;
        m.Initialize(modelTransform);
      }

      foreach (var l in lights)
      {
        l.Initialize(modelTransform);
      }

      ApplyOffsets(modelPositionOffset, modelRotationOffset, modelScaleOffset);
    }

    public void Update()
    {
      for (int i = 0; i < materials.Count; i++)
      {
        var m = materials[i];
        m.Update();
      }
    }

    public void ApplyOffsets(Vector3 position, Vector3 rotation, Vector3 scale)
    {
      modelPositionOffset = position;
      modelRotationOffset = rotation;
      modelScaleOffset    = scale;

      Utils.Log($"[WaterfallModel] Applying model offsets {position}, {rotation}, {scale}", LogType.Effects);

      positionOffsetString = $"{position.x}, {position.y}, {position.z}";
      rotationOffestString = $"{rotation.x}, {rotation.y}, {rotation.z}";
      scaleOffsetString    = $"{scale.x}, {scale.y}, {scale.z}";

      for (int i = 0; i < modelTransforms.Count; i++)
      {
        var modelTransform = modelTransforms[i];
        modelTransform.localPosition = modelPositionOffset;
        modelTransform.localScale    = modelScaleOffset;

        if (modelRotationOffset == Vector3.zero)
        {
          modelTransform.localRotation = Quaternion.identity;
        }
        else
        {
          modelTransform.localEulerAngles = modelRotationOffset;
        }
      }
    }

    public void SetEnabled(bool state)
    {
      for (int i = 0; i < modelTransforms.Count; i++)
      {
        var modelTransform = modelTransforms[i];
        var skinRenderers  = modelTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int j = 0; j < skinRenderers.Length; j++)
        {
          var renderer = skinRenderers[j];
          renderer.enabled = state;
        }

        var renderers = modelTransform.GetComponentsInChildren<Renderer>();
      }
    }

    public void SetTexture(WaterfallMaterial targetMat, string propertyName, string value)
    {
      for (int i = 0; i < materials.Count; i++)
      {
        var m = materials[i];
        if (m == targetMat)
        {
          m.SetTexture(propertyName, value);
          if (modelTransforms.Count > 1)
          {
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t  = modelTransforms[transformIndex];
              var renderers = t.GetComponentsInChildren<Renderer>();
              for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
              {
                var renderer = renderers[rendererIndex];
                if (renderer.name == m.transformName)
                {
                  renderer.material.SetTexture(propertyName, GameDatabase.Instance.GetTexture(value, false));
                }
              }
            }
          }
        }
      }
    }

    public void SetFloat(WaterfallMaterial targetMat, string propertyName, float value)
    {
      for (int matIndex = 0; matIndex < materials.Count; matIndex++)
      {
        var m = materials[matIndex];
        if (m == targetMat)
        {
          m.SetFloat(propertyName, value);

          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t  = modelTransforms[transformIndex];
              var rs = t.GetComponentsInChildren<Renderer>();
              for (int rendererIndex = 0; rendererIndex < rs.Length; rendererIndex++)
              {
                var r = rs[rendererIndex];
                if (r.name == m.transformName)
                {
                  if (propertyName == "_Seed" && m.useAutoRandomization) { }
                  else
                  {
                    r.material.SetFloat(propertyName, value);
                  }
                }
              }
            }
        }
      }
    }

    public void SetVector4(WaterfallMaterial targetMat, string propertyName, Vector4 value)
    {
      for (int matIndex = 0; matIndex < materials.Count; matIndex++)
      {
        var m = materials[matIndex];
        if (m == targetMat)
        {
          m.SetVector4(propertyName, value);
          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t  = modelTransforms[transformIndex];
              var rs = t.GetComponentsInChildren<Renderer>();
              for (int rendererIndex = 0; rendererIndex < rs.Length; rendererIndex++)
              {
                var r = rs[rendererIndex];
                if (r.name == m.transformName)
                {
                  r.material.SetVector(propertyName, value);
                }
              }
            }
        }
      }
    }

    public void SetTextureScale(WaterfallMaterial targetMat, string propertyName, Vector2 value)
    {
      for (int matIndex = 0; matIndex < materials.Count; matIndex++)
      {
        var m = materials[matIndex];
        if (m == targetMat)
        {
          m.SetTextureScale(propertyName, value);
          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t  = modelTransforms[transformIndex];
              var rs = t.GetComponentsInChildren<Renderer>();
              for (int rendererIndex = 0; rendererIndex < rs.Length; rendererIndex++)
              {
                var r = rs[rendererIndex];
                if (r.name == m.transformName)
                {
                  r.material.SetTextureScale(propertyName, value);
                }
              }
            }
        }
      }
    }

    public void SetTextureOffset(WaterfallMaterial targetMat, string propertyName, Vector2 value)
    {
      for (int maxIndex = 0; maxIndex < materials.Count; maxIndex++)
      {
        var m = materials[maxIndex];
        if (m == targetMat)
        {
          m.SetTextureOffset(propertyName, value);
          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t  = modelTransforms[transformIndex];
              var rs = t.GetComponentsInChildren<Renderer>();
              for (int rendererIndex = 0; rendererIndex < rs.Length; rendererIndex++)
              {
                var r = rs[rendererIndex];
                if (r.name == m.transformName)
                {
                  r.material.SetTextureOffset(propertyName, value);
                }
              }
            }
        }
      }
    }

    public void SetColor(WaterfallMaterial targetMat, string propertyName, Color value)
    {
      for (int matIndex = 0; matIndex < materials.Count; matIndex++)
      {
        var m = materials[matIndex];
        if (m == targetMat)
        {
          m.SetColor(propertyName, value);

          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t  = modelTransforms[transformIndex];
              var rs = t.GetComponentsInChildren<Renderer>();
              for (int rendererIndex = 0; rendererIndex < rs.Length; rendererIndex++)
              {
                var r = rs[rendererIndex];
                if (r.name == m.transformName)
                {
                  r.material.SetColor(propertyName, value);
                }
              }
            }
        }
      }
    }

    public void SetLightRange(WaterfallLight targetLight, float value)
    {
      for (int lightIndex = 0; lightIndex < lights.Count; lightIndex++)
      {
        var l = lights[lightIndex];
        if (l == targetLight)
        {
          l.SetRange(value);

          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t   = modelTransforms[transformIndex];
              var lis = t.GetComponentsInChildren<Light>();
              for (int lightComponentIndex = 0; lightComponentIndex < lis.Length; lightComponentIndex++)
              {
                var li = lis[lightComponentIndex];
                li.range = value;
              }
            }
        }
      }
    }

    public void SetLightIntensity(WaterfallLight targetLight, float value)
    {
      for (int lightIndex = 0; lightIndex < lights.Count; lightIndex++)
      {
        var l = lights[lightIndex];
        if (l == targetLight)
        {
          l.SetIntensity(value);

          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t   = modelTransforms[transformIndex];
              var lis = t.GetComponentsInChildren<Light>();
              for (int lightComponentIndex = 0; lightComponentIndex < lis.Length; lightComponentIndex++)
              {
                var li = lis[lightComponentIndex];
                li.intensity = value;
              }
            }
        }
      }
    }

    public void SetLightAngle(WaterfallLight targetLight, float value)
    {
      for (int lightIndex = 0; lightIndex < lights.Count; lightIndex++)
      {
        var l = lights[lightIndex];
        if (l == targetLight)
        {
          l.SetAngle(value);

          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t   = modelTransforms[transformIndex];
              var lis = t.GetComponentsInChildren<Light>();
              for (int lightComponentIndex = 0; lightComponentIndex < lis.Length; lightComponentIndex++)
              {
                var li = lis[lightComponentIndex];
                li.spotAngle = value;
              }
            }
        }
      }
    }

    public void SetLightColor(WaterfallLight targetLight, Color value)
    {
      for (int lightIndex = 0; lightIndex < lights.Count; lightIndex++)
      {
        var l = lights[lightIndex];
        if (l == targetLight)
        {
          l.SetColor(value);

          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t   = modelTransforms[transformIndex];
              var lis = t.GetComponentsInChildren<Light>();
              for (int lightComponentIndex = 0; lightComponentIndex < lis.Length; lightComponentIndex++)
              {
                var li = lis[lightComponentIndex];
                li.color = value;
              }
            }
        }
      }
    }

    public void SetLightType(WaterfallLight targetLight, LightType value)
    {
      for (int lightIndex = 0; lightIndex < lights.Count; lightIndex++)
      {
        var l = lights[lightIndex];
        if (l == targetLight)
        {
          l.SetLightType(value);

          if (modelTransforms.Count > 1)
            for (int transformIndex = 0; transformIndex < modelTransforms.Count; transformIndex++)
            {
              var t   = modelTransforms[transformIndex];
              var lis = t.GetComponentsInChildren<Light>();
              for (int lightComponentIndex = 0; lightComponentIndex < lis.Length; lightComponentIndex++)
              {
                var li = lis[lightComponentIndex];
                li.type = value;
              }
            }
        }
      }
    }
  }
}