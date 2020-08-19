using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{

  public class WaterfallModel
  {
    public string path;
    public string positionOffsetString;
    public string rotationOffestString;
    public string scaleOffsetString;

    public string overrideShader= "";
    public List<WaterfallMaterial> materials;


    public List<Transform> modelTransforms;
    public Vector3 modelPositionOffset = Vector3.zero;
    public Vector3 modelRotationOffset = Vector3.zero;
    public Vector3 modelScaleOffset = Vector3.one;

    public WaterfallModel()
    {
      modelTransforms = new List<Transform>();
    }

    public WaterfallModel(string modelPath, string shader)
    {
      modelTransforms = new List<Transform>();
      path = modelPath;
      overrideShader = shader;
    }

    public WaterfallModel(ConfigNode node) : this()
    {
      Load(node);
    }
    public void Load(ConfigNode node)
    {
      if (!node.TryGetValue("path", ref path))
        Utils.LogError(String.Format("[WaterfallModel]: Unabled to find required path string in MODEL node"));
      node.TryGetValue("positionOffset", ref modelPositionOffset);
      node.TryGetValue("rotationOffset", ref modelRotationOffset);
      node.TryGetValue("scaleOffset", ref modelScaleOffset);

      materials = new List<WaterfallMaterial>();
      foreach (ConfigNode materialNode in node.GetNodes(WaterfallConstants.MaterialNodeName))
      {
        materials.Add(new WaterfallMaterial(materialNode));
      }
    }
    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.ModelNodeName;
      node.AddValue("path", path);
      node.AddValue("positionOffset", modelPositionOffset);
      node.AddValue("rotationOffset", modelRotationOffset);
      node.AddValue("scaleOffset", modelScaleOffset);
      foreach (WaterfallMaterial m in materials)
      {
        node.AddNode(m.Save());
      }

      return node;
    }


    public void Initialize(Transform parent, bool fromNothing)
    {
      Utils.Log(String.Format("[WaterfallModel]: Instantiating model from {0} ", path));
      if (!GameDatabase.Instance.ExistsModel(path))
        Utils.LogError(String.Format("[WaterfallModel]: Unabled to find model {0} in GameDatabase", path));
      GameObject inst = GameObject.Instantiate(GameDatabase.Instance.GetModelPrefab(path), parent.position, parent.rotation);
      inst.SetActive(true);

      Transform modelTransform = inst.GetComponent<Transform>();
      
      modelTransform.SetParent(parent, true);
      modelTransform.localScale = modelScaleOffset;
      modelTransform.localPosition = modelPositionOffset;

      if (modelRotationOffset == Vector3.zero)
        modelTransform.localRotation = Quaternion.identity;
      else
        modelTransform.localRotation = Quaternion.LookRotation(modelRotationOffset);
      Utils.Log(String.Format("[WaterfallModel]: Instantiated model at {0} with {1}, {2}", modelTransform.position, modelRotationOffset, modelScaleOffset));

      modelTransforms.Add(modelTransform);

      Renderer[] renderers = modelTransform.GetComponentsInChildren<Renderer>();

      if (fromNothing)
      {
        materials = new List<WaterfallMaterial>();
        foreach (Renderer r in renderers)
        {
          WaterfallMaterial m = new WaterfallMaterial();
          m.shaderName = overrideShader;
          m.materials = new List<Material>();
          m.transformName = r.transform.name;
          materials.Add(m);
        }
      }

      foreach (WaterfallMaterial m in materials)
      {
        m.Initialize(modelTransform);
      }
      ApplyOffsets(modelPositionOffset, modelRotationOffset, modelScaleOffset);
    }

    public void ApplyOffsets(Vector3 position, Vector3 rotation, Vector3 scale)
    {
      modelPositionOffset = position;
      modelRotationOffset = rotation;
      modelScaleOffset = scale;

      Utils.Log($"[WaterfallModel] Applying geometric offsets {position}, {rotation}, {scale}");

      positionOffsetString = $"{position.x}, {position.y}, {position.z}";
      rotationOffestString = $"{rotation.x}, {rotation.y}, {rotation.z}";
      scaleOffsetString = $"{scale.x}, {scale.y}, {scale.z}";

      foreach (Transform modelTransform in modelTransforms)
      {
        modelTransform.localPosition = modelPositionOffset;
        modelTransform.localScale = modelScaleOffset;

        if (modelRotationOffset == Vector3.zero)
          modelTransform.localRotation = Quaternion.identity;
        else
          modelTransform.localRotation = Quaternion.LookRotation(modelRotationOffset);
      }

    }
    public void SetEnabled(bool state)
    {
      foreach (Transform modelTransform in modelTransforms)
      {
        SkinnedMeshRenderer[] skinRenderers = modelTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinRenderers)
        {
          renderer.enabled = state;
        }
        Renderer[] renderers = modelTransform.GetComponentsInChildren<Renderer>();
       
      }
    }

    public void SetTexture(WaterfallMaterial targetMat, string propertyName, string value)
    {
      foreach (WaterfallMaterial m in materials)
      {
        if (m == targetMat)
        {
          m.SetTexture(propertyName, value);
          if (modelTransforms.Count > 1)
          {
            foreach (Transform t in modelTransforms)
            {
              foreach(Renderer r in t.GetComponentsInChildren<Renderer>())
              {
                if (r.name == m.transformName)
                {
                  r.material.SetTexture(propertyName, GameDatabase.Instance.GetTexture(value, false));
                }
              }
            }
          }
        }
      }
    }
    public void SetFloat(WaterfallMaterial targetMat, string propertyName, float value)
    {
      foreach (WaterfallMaterial m in materials)
      {
        if (m == targetMat)
        {
          m.SetFloat(propertyName, value);

          if (modelTransforms.Count > 1)
            foreach (Transform t in modelTransforms)
            {
              foreach (Renderer r in t.GetComponentsInChildren<Renderer>())
              {
                if (r.name == m.transformName)
                {
                  r.material.SetFloat(propertyName,value);
                }
              }
            }
        }
      }
    }
    public void SetVector4(WaterfallMaterial targetMat, string propertyName, Vector4 value)
    {
      foreach (WaterfallMaterial m in materials)
      {
        if (m == targetMat)
          m.SetVector4(propertyName, value);
      }
    }
    public void SetTextureScale(WaterfallMaterial targetMat, string propertyName, Vector2 value)
    {
      foreach (WaterfallMaterial m in materials)
      {
        if (m == targetMat)
          m.SetTextureScale(propertyName, value);
      }
    }
    public void SetTextureOffset(WaterfallMaterial targetMat, string propertyName, Vector2 value)
    {
      foreach (WaterfallMaterial m in materials)
      {
        if (m == targetMat)
          m.SetTextureOffset(propertyName, value);
      }
    }
    public void SetColor(WaterfallMaterial targetMat, string propertyName, Color value)
    {
      foreach (WaterfallMaterial m in materials)
      {
        if (m == targetMat)
        {
          m.SetColor(propertyName, value);

          if (modelTransforms.Count > 1)
            foreach (Transform t in modelTransforms)
            {
              foreach (Renderer r in t.GetComponentsInChildren<Renderer>())
              {
                if (r.name == m.transformName)
                {
                  r.material.SetColor(propertyName, value);
                }
              }
            }
        }
      }
    }
  }
}
