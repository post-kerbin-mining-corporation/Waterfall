using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// Defines a material managed by Waterfall
  /// </summary>
  /// 
  public class WaterfallSkinnedMesh
  {
    public SkinnedMeshRenderer SMR;
    public Mesh SkinedMesh;
    public WaterfallSkinnedMesh(SkinnedMeshRenderer skin, MeshFilter filter)
    {
      SMR = skin;
      SkinedMesh = skin.sharedMesh;
    }

    public void Recalculate()
    {
      SMR.BakeMesh(SkinedMesh);
      if (SkinedMesh)
      {
        SkinedMesh.RecalculateNormals();
      }
    }
  }
  public class WaterfallMaterial
  {
    public string shaderName;
    public string transformName = "";
    public string baseTransformName = "";
    public bool useAutoRandomization = true;
    public List<WaterfallMaterialProperty> matProperties;

    public List<Material> materials;
    protected Renderer targetMeshRenderer;
    protected List<WaterfallSkinnedMesh> skinnedMeshes;
    protected List<MeshFilter> targetFilter;
    protected List<Mesh> bakedMesh;
    public WaterfallMaterial()
    {

      matProperties = new List<WaterfallMaterialProperty>();
    }
    public WaterfallMaterial(ConfigNode node)
    {
      Load(node);
    }

    /// <summary>
    /// Load from a confignode
    /// </summary>
    /// <param name="node"></param>
    public void Load(ConfigNode node)
    {

      node.TryGetValue("transform", ref transformName);
      node.TryGetValue("baseTransform", ref baseTransformName);
      node.TryGetValue("shader", ref shaderName);
      node.TryGetValue("randomizeSeed", ref useAutoRandomization);

      materials = new List<Material>();
      Utils.Log(String.Format("[WaterfallMaterial]: Loading new material for {0} ", transformName), LogType.Effects);

      matProperties = new List<WaterfallMaterialProperty>();
      foreach (ConfigNode subnode in node.GetNodes(WaterfallConstants.TextureNodeName))
      {
        matProperties.Add(new WaterfallMaterialTextureProperty(subnode));
      }
      foreach (ConfigNode subnode in node.GetNodes(WaterfallConstants.ColorNodeName))
      {
        matProperties.Add(new WaterfallMaterialColorProperty(subnode));
      }
      foreach (ConfigNode subnode in node.GetNodes(WaterfallConstants.FloatNodeName))
      {
        matProperties.Add(new WaterfallMaterialFloatProperty(subnode));
      }
    }

    /// <summary>
    /// Save to a config node
    /// </summary>
    /// <returns></returns>
    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.MaterialNodeName;
      if (baseTransformName != "")
        node.AddValue("baseTransform", baseTransformName);
      if (transformName != "")
        node.AddValue("transform", transformName);
      node.AddValue("shader", shaderName);
      node.AddValue("randomizeSeed", useAutoRandomization);
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        node.AddNode(p.Save());
      }
      return node;
    }

    /// <summary>
    /// Initialize the material by finding its target transform, the applying shader properties loaded in configuration to it
    /// </summary>
    /// <param name="parentTransform"></param>
    public void Initialize(Transform parentTransform)
    {
      materials = new List<Material>();
      skinnedMeshes = new List<WaterfallSkinnedMesh>();
      if (shaderName != null)
      {
        if (baseTransformName != "")
        {
          Transform[] candidates = parentTransform.GetComponentsInChildren<Transform>();
          foreach (Transform t in candidates)
          {

            Renderer r = t.GetComponent<Renderer>();
            if (r != null && r.material != null)
            {
              if (t.GetComponent<SkinnedMeshRenderer>() != null)
              {
                skinnedMeshes.Add(new WaterfallSkinnedMesh(
                  t.GetComponent<SkinnedMeshRenderer>(),
                  t.GetComponent<MeshFilter>()));
              }

              Utils.Log($"Added rendered material from {t.name}", LogType.Effects);
              materials.Add(r.material);
            }
          }
        }
        else
        {
          Transform materialTarget = parentTransform.FindDeepChild(transformName);
          targetMeshRenderer = materialTarget.GetComponent<Renderer>();
          if (materialTarget.GetComponent<SkinnedMeshRenderer>() != null)
          {
            skinnedMeshes.Add(new WaterfallSkinnedMesh(
              materialTarget.GetComponent<SkinnedMeshRenderer>(),
              materialTarget.GetComponent<MeshFilter>()));
          }

          materials.Add(targetMeshRenderer.material);
        }


        foreach (Material mat in materials)
        {
          mat.shader = ShaderLoader.GetShader(shaderName);


          foreach (WaterfallMaterialProperty p in matProperties)
          {
            p.Initialize(mat);
          }
          if (useAutoRandomization && mat.HasProperty("_Seed"))
          {
            mat.SetFloat("_Seed", UnityEngine.Random.Range(-10, 10));
          }

          Utils.Log(String.Format("[WaterfallMaterial]: Assigned new shader {0} ", mat.shader), LogType.Effects);
        }
      }
    }

    public void Update()
    {
      if (skinnedMeshes != null)
      {
        foreach (WaterfallSkinnedMesh smr in skinnedMeshes)
        {
          smr.Recalculate();
        }
      }

    }
    /// <summary>
    /// Sets a shader float on this material. If it doesn't exist as a material property object, create it.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    public void SetFloat(string propertyName, float value)
    {
      bool existsSavedProperty = false;
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        if (p.propertyName == propertyName)
          existsSavedProperty = true;
      }
      if (!existsSavedProperty)
      {
        WaterfallMaterialFloatProperty newProp = new WaterfallMaterialFloatProperty();
        newProp.propertyName = propertyName;
        newProp.propertyValue = value;
        matProperties.Add(newProp);
      }
      else
      {
        foreach (WaterfallMaterialProperty p in matProperties)
        {
          if (p.propertyName == propertyName)
          {
            WaterfallMaterialFloatProperty t = (WaterfallMaterialFloatProperty)p;
            t.propertyValue = value;
          }
        }
      }
      foreach (Material mat in materials)
      {
        if (propertyName == "_Seed" && useAutoRandomization) { }
        else 
          mat.SetFloat(propertyName, value);
      }
    }

    /// <summary>
    /// Sets a shader Vec4 on this material. If it doesn't exist as a material property object, create it.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    public void SetVector4(string propertyName, Vector4 value)
    {
      bool existsSavedProperty = false;
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        if (p.propertyName == propertyName)
          existsSavedProperty = true;
      }
      if (!existsSavedProperty)
      {
        WaterfallMaterialVector4Property newProp = new WaterfallMaterialVector4Property();
        newProp.propertyName = propertyName;
        newProp.propertyValue = value;
        matProperties.Add(newProp);
      }
      else
      {
        foreach (WaterfallMaterialProperty p in matProperties)
        {
          if (p.propertyName == propertyName)
          {
            WaterfallMaterialVector4Property t = (WaterfallMaterialVector4Property)p;
            t.propertyValue = value;
          }
        }
      }
      foreach (Material mat in materials)
      {
        mat.SetFloatArray(propertyName, new float[] { value.x, value.y, value.z, value.w });
      }
    }


    /// <summary>
    /// Sets the texture scale for this material. If it doesn't exist as a material property object, create it.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    public void SetTextureScale(string propertyName, Vector2 value)
    {
      bool existsSavedProperty = false;
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        if (p.propertyName == propertyName)
          existsSavedProperty = true;
      }
      if (!existsSavedProperty)
      {
        WaterfallMaterialTextureProperty newProp = new WaterfallMaterialTextureProperty();
        newProp.propertyName = propertyName;
        newProp.textureOffset = materials[0].GetTextureOffset(propertyName);
        newProp.textureScale = value;
        matProperties.Add(newProp);
      }
      else
      {
        foreach (WaterfallMaterialProperty p in matProperties)
        {
          if (p.propertyName == propertyName)
          {
            WaterfallMaterialTextureProperty t = (WaterfallMaterialTextureProperty)p;
            t.textureScale = value;
          }
        }
      }
      foreach (Material mat in materials)
      {
        mat.SetTextureScale(propertyName, value);
      }
    }

    /// <summary>
    /// Sets the texture for this material. If it doesn't exist as a material property object, create it.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    public void SetTexture(string propertyName, string value)
    {
      Utils.Log($"[WaterfallMaterial] Changing {propertyName} to {value}", LogType.Effects);
      bool existsSavedProperty = false;
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        if (p.propertyName == propertyName)
          existsSavedProperty = true;
      }
      if (!existsSavedProperty)
      {
        WaterfallMaterialTextureProperty newProp = new WaterfallMaterialTextureProperty();
        newProp.propertyName = propertyName;
        newProp.texturePath = value;
        newProp.textureScale = materials[0].GetTextureScale(propertyName);
        newProp.textureOffset = materials[0].GetTextureOffset(propertyName);
        matProperties.Add(newProp);
      }
      else
      {
        foreach (WaterfallMaterialProperty p in matProperties)
        {
          if (p.propertyName == propertyName)
          {
            WaterfallMaterialTextureProperty t = (WaterfallMaterialTextureProperty)p;
            t.texturePath = value;
          }
        }
      }
      foreach (Material mat in materials)
      {
        Utils.Log($"[WaterfallMaterial] Changing {propertyName} to {value} on {mat}", LogType.Effects);
        mat.SetTexture(propertyName, GameDatabase.Instance.GetTexture(value, false));
      }
    }

    /// <summary>
    /// Sets the texture offset for this material. If it doesn't exist as a material property object, create it.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    public void SetTextureOffset(string propertyName, Vector2 value)
    {
      bool existsSavedProperty = false;
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        if (p.propertyName == propertyName)
          existsSavedProperty = true;
      }
      if (!existsSavedProperty)
      {
        WaterfallMaterialTextureProperty newProp = new WaterfallMaterialTextureProperty();
        newProp.propertyName = propertyName;
        newProp.textureScale = materials[0].GetTextureScale(propertyName);
        newProp.textureOffset = value;
        matProperties.Add(newProp);
      }
      else
      {
        foreach (WaterfallMaterialProperty p in matProperties)
        {
          if (p.propertyName == propertyName)
          {
            WaterfallMaterialTextureProperty t = (WaterfallMaterialTextureProperty)p;
            t.textureOffset = value;
          }
        }
      }
      foreach (Material mat in materials)
      {
        mat.SetTextureOffset(propertyName, value);
      }
    }

    /// <summary>
    /// Sets the material colour. If it doesn't exist as a material property object, create it.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    public void SetColor(string propertyName, Color value)
    {
      bool existsSavedProperty = false;
      foreach (WaterfallMaterialProperty p in matProperties)
      {
        if (p.propertyName == propertyName)
          existsSavedProperty = true;
      }
      if (!existsSavedProperty)
      {
        WaterfallMaterialColorProperty newProp = new WaterfallMaterialColorProperty();
        newProp.propertyName = propertyName;
        newProp.propertyValue = value;
        matProperties.Add(newProp);
      }
      else
      {
        foreach (WaterfallMaterialProperty p in matProperties)
        {
          if (p.propertyName == propertyName)
          {
            WaterfallMaterialColorProperty t = (WaterfallMaterialColorProperty)p;
            t.propertyValue = value;
          }
        }
      }
      foreach (Material mat in materials)
      {
        mat.SetColor(propertyName, value);
      }
    }
  }

}
