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

    public List<WaterfallMaterial> materials;


    public List<Transform> modelTransforms;
    public Vector3 modelPositionOffset = Vector3.zero;
    public Vector3 modelRotationOffset = Vector3.zero;
    public Vector3 modelScaleOffset = Vector3.one;

    public WaterfallModel()
    {
      modelTransforms = new List<Transform>();
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

    public void Initialize(Transform parent)
    {
      Utils.Log(String.Format("[WaterfallModel]: Instantiating model from {0} ", path));
      if (!GameDatabase.Instance.ExistsModel(path))
        Utils.LogError(String.Format("[WaterfallModel]: Unabled to find model {0} in GameDatabase", path));
      GameObject inst = GameObject.Instantiate(GameDatabase.Instance.GetModelPrefab(path), parent.position, parent.rotation);
      inst.SetActive(true);


      Transform modelTransform = inst.GetComponent<Transform>();
      modelTransform.localScale = modelScaleOffset;
      modelTransform.SetParent(parent, true);
      modelTransform.localPosition = modelPositionOffset;

      if (modelRotationOffset == Vector3.zero)
        modelTransform.localRotation = Quaternion.identity;
      else
        modelTransform.localRotation = Quaternion.LookRotation(modelRotationOffset);
      Utils.Log(String.Format("[WaterfallModel]: Instantiated model at {0} ", modelTransform.position));

      modelTransforms.Add(modelTransform);

      foreach (WaterfallMaterial m in materials)
      {
        m.Initialize(modelTransform);
      }

    }

    public void ApplyOffsets(Vector3 position, Vector3 rotation, Vector3 scale)
    {
      modelPositionOffset = position;
      modelRotationOffset = rotation;
      modelScaleOffset = scale;

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
        //modelTransform.gameObject.SetActive(state);
      }
    }
  }

}
