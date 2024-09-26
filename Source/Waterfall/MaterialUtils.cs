using System.Collections.Generic;
using UnityEngine;
using Waterfall.UI;

namespace Waterfall
{
  static class Extension
  {
    public static bool IsEqualTo(this Color me, Color other)
    {
      return me.r == other.r && me.g == other.g && me.b == other.b && me.a == other.a;
    }
  }
  public static class MaterialUtils
  {
    public static string[] ColorToStringArray(Color c)
    {
      string[] newArr = new string[4];
      newArr[0] = c.r.ToString();
      newArr[1] = c.g.ToString();
      newArr[2] = c.b.ToString();
      newArr[3] = c.a.ToString();
      return newArr;
    }

    public static List<string> FindValidShaderProperties(Material m, WaterfallMaterialPropertyType propType)
    {
      var validProps = new List<string>();
      foreach (var mProp in ShaderLoader.GetShaderPropertyMap())
      {
        if (m.HasProperty(mProp.Key))
        {
          if (mProp.Value.type == propType)
          {
            validProps.Add(mProp.Key);
          }
        }
      }

      return validProps;
    }
  }

  public class ParsedMaterial
  {
    private Dictionary<string, Color>     colorValues         = new();
    private Dictionary<string, float>     floatValues         = new();
    private Dictionary<string, string>    textureValues       = new();
    private Dictionary<string, Vector2>   textureScaleValues  = new();
    private Dictionary<string, Vector2>   textureOffsetValues = new();
    private Dictionary<string, Texture2D> colorTextures       = new();

    private Dictionary<string, string[]> colorStrings         = new();
    private Dictionary<string, string>   floatStrings         = new();
    private Dictionary<string, string[]> textureOffsetStrings = new();
    private Dictionary<string, string[]> textureScaleStrings  = new();


    private Material matl;

    public ParsedMaterial(Material m)
    {
      matl = m;
      InitializeShaderProperties(m);
    }

    protected void InitializeShaderProperties(Material m)
    {
      colorValues          = new();
      floatValues          = new();
      textureValues        = new();
      textureScaleValues   = new();
      textureOffsetValues  = new();
      colorStrings         = new();
      floatStrings         = new();
      textureOffsetStrings = new();
      textureScaleStrings  = new();
      colorTextures        = new();

      foreach (var mProp in ShaderLoader.GetShaderPropertyMap())
      {
        if (m.HasProperty(mProp.Key))
        {
          if (mProp.Value.type == WaterfallMaterialPropertyType.Color)
          {
            colorValues.Add(mProp.Key, m.GetColor(mProp.Key));
            colorStrings.Add(mProp.Key, MaterialUtils.ColorToStringArray(m.GetColor(mProp.Key)));
            colorTextures.Add(mProp.Key, TextureUtils.GenerateColorTexture(64, 32, m.GetColor(mProp.Key)));
          }

          if (mProp.Value.type == WaterfallMaterialPropertyType.Float)
          {
            floatValues.Add(mProp.Key, m.GetFloat(mProp.Key));
            floatStrings.Add(mProp.Key, m.GetFloat(mProp.Key).ToString());
          }

          if (mProp.Value.type == WaterfallMaterialPropertyType.Texture)
          {
            textureValues.Add(mProp.Key, m.GetTexture(mProp.Key).name);
            textureScaleValues.Add(mProp.Key, m.GetTextureScale(mProp.Key));
            textureOffsetValues.Add(mProp.Key, m.GetTextureOffset(mProp.Key));
            textureOffsetStrings.Add(mProp.Key, new[] { $"{m.GetTextureOffset(mProp.Key).x}", $"{m.GetTextureOffset(mProp.Key).y}" });
            textureScaleStrings.Add(mProp.Key, new[] { $"{m.GetTextureScale(mProp.Key).x}", $"{m.GetTextureScale(mProp.Key).y}" });
          }
        }
      }
    }
  }

  public class MaterialData
  {
    public string name;
    public Vector2                       floatRange;
    public WaterfallMaterialPropertyType type;

    public MaterialData(WaterfallMaterialPropertyType theType, Vector2 range)
    {
      type       = theType;
      floatRange = range;
    }
    public MaterialData(string propertyName, WaterfallMaterialPropertyType theType, Vector2 range)
    {
      name = propertyName;
      type = theType;
      floatRange = range;
    }
    public MaterialData(WaterfallMaterialPropertyType theType)
    {
      type = theType;
    }
  }

}