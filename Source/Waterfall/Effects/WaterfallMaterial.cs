using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall
{

  public class WaterfallMaterial
  {
    public string shaderName;
    public string transformName;


    public List<WaterfallMaterialProperty> matProperties;
    //public List<WaterfallMaterialTextureProperty> textures;
    //public List<WaterfallMaterialProperty> matProperties;
    //public List<WaterfallMaterialColorProperty> matColors;

    public Material material;

    public WaterfallMaterial() { }
    public WaterfallMaterial(ConfigNode node)
    {
      Load(node);
    }
    public void Load(ConfigNode node)
    {

      node.TryGetValue("transform", ref transformName);
      node.TryGetValue("shader", ref shaderName);
      Utils.Log(String.Format("[WaterfallMaterial]: Loading new material for {0} ", transformName));

      matProperties = new List<WaterfallMaterialProperty>();
      foreach (ConfigNode subnode in node.GetNodes(WaterfallConstants.TextureNodeName))
      {
        matProperties.Add(new WaterfallMaterialTextureProperty(subnode));
      }
     
      foreach (ConfigNode subnode in node.GetNodes("COLOR"))
      {
        matProperties.Add(new WaterfallMaterialColorProperty(subnode));
      }
      foreach (ConfigNode subnode in node.GetNodes("FLOAT"))
      {
        matProperties.Add(new WaterfallMaterialFloatProperty(subnode));
      }
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.MaterialNodeName;
      node.AddValue("shader", shaderName);
      node.AddValue("transform", transformName);
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        node.AddNode(p.Save());
      }

      return node;
    }

    public void Initialize(Transform parentTransform)
    {
      Transform materialTarget = parentTransform.FindDeepChild(transformName);
      material = materialTarget.GetComponent<Renderer>().material;
      material.shader = ShaderLoader.GetShader(shaderName);

      foreach (WaterfallMaterialProperty p in matProperties)
      {
        p.Initialize(material);
      }

      Utils.Log(String.Format("[WaterfallMaterial]: New shader is {0} ", material.shader));
    }
  }
  
}
