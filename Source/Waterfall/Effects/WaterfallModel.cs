using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall
{

  public class WaterfallModel
  {
    public string path;
    public string positionOffsetString;
    public string rotationOffestString;

    public List<WaterfallMaterial> materials;


    public List<Transform> modelTransforms;
    public Vector3 modelPositionOffset = Vector3.zero;
    public Vector3 modelRotationOffset = Vector3.zero;

    public WaterfallModel()
    {
      modelTransforms = new List<Transform>();
    }


    public WaterfallModel(ConfigNode node):this()
    {
      Load(node);
    }
    public void Load(ConfigNode node)
    {
      if (!node.TryGetValue("path", ref path))
        Utils.LogError(String.Format("[WaterfallModel]: Unabled to find required path string in MODEL node"));
      node.TryGetValue("positionOffset", ref modelPositionOffset);
      node.TryGetValue("rotationOffset", ref modelRotationOffset);

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
      node.AddValue("positionOffset", positionOffsetString);
      node.AddValue("rotationOffset", rotationOffestString);
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
      modelTransform.localScale = Vector3.one;
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

    public void ApplyOffsets(Vector3 position, Vector3 rotation)
    {
      modelPositionOffset = position;
      modelRotationOffset = rotation;

      positionOffsetString = $"{position.x}, {position.y}, {position.z}";
      rotationOffestString = $"{rotation.x}, {rotation.y}, {rotation.z}";

      foreach (Transform modelTransform in modelTransforms)
      {

        modelTransform.localPosition = modelPositionOffset;
        if (modelRotationOffset == Vector3.zero)
          modelTransform.localRotation = Quaternion.identity;
        else
          modelTransform.localRotation = Quaternion.LookRotation(modelRotationOffset);
      }
    
    }
  }

}
