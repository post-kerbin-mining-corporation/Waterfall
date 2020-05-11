using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall
{
  public static class MaterialUtils
  {
    public static Texture2D GenerateColorTexture(int texWidth, int texHeight, Color color)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, false);

      Color c = new Color(color.r, color.g, color.b, 1.0f);
      int xAlphaEnd = (int)Mathf.Round(color.a * (float)texWidth);
      for (int x = 0; x < texWidth; x++)
      {
        for (int y = 0; y < texHeight; y++)
        {
          tex.SetPixel(x, y, c);

          if (y <= 5)
          {
            if (x < xAlphaEnd)
            {
              tex.SetPixel(x, y, Color.white);
            }
            else
            {
              tex.SetPixel(x, y, Color.black);
            }

          }
        }
      }
      tex.Apply();
      return tex;
    }
    public static string[] ColorToStringArray(Color c)
    {
      string[] newArr = new string[4];
      newArr[0] = c.r.ToString();
      newArr[1] = c.g.ToString();
      newArr[2] = c.b.ToString();
      newArr[3] = c.a.ToString();
      return newArr;
    }
  }
  public class ParsedMaterial
  {

    Dictionary<string, Color> colorValues = new Dictionary<string, Color>();
    Dictionary<string, float> floatValues = new Dictionary<string, float>();
    Dictionary<string, string> textureValues = new Dictionary<string, string>();
    Dictionary<string, Vector2> textureScaleValues = new Dictionary<string, Vector2>();
    Dictionary<string, Vector2> textureOffsetValues = new Dictionary<string, Vector2>();
    Dictionary<string, Texture2D> colorTextures = new Dictionary<string, Texture2D>();

    Dictionary<string, string[]> colorStrings = new Dictionary<string, string[]>();
    Dictionary<string, string> floatStrings = new Dictionary<string, string>();
    Dictionary<string, string[]> textureOffsetStrings = new Dictionary<string, string[]>();
    Dictionary<string, string[]> textureScaleStrings = new Dictionary<string, string[]>();


    Material matl;

    public ParsedMaterial(Material m)
    {
      matl = m;
      InitializeShaderProperties(m);
    }

    protected void InitializeShaderProperties(Material m)
    {
      colorValues = new Dictionary<string, Color>();
      floatValues = new Dictionary<string, float>();
      textureValues = new Dictionary<string, string>();
      textureScaleValues = new Dictionary<string, Vector2>();
      textureOffsetValues = new Dictionary<string, Vector2>();
      colorStrings = new Dictionary<string, string[]>();
      floatStrings = new Dictionary<string, string>();
      textureOffsetStrings = new Dictionary<string, string[]>();
      textureScaleStrings = new Dictionary<string, string[]>();
      colorTextures = new Dictionary<string, Texture2D>();

      foreach (KeyValuePair<string, WaterfallMaterialPropertyType> mProp in WaterfallConstants.ShaderPropertyMap)
      {
        if (m.HasProperty(mProp.Key))
        {
          if (mProp.Value == WaterfallMaterialPropertyType.Color)
          {
            colorValues.Add(mProp.Key, m.GetColor(mProp.Key));
            colorStrings.Add(mProp.Key, MaterialUtils.ColorToStringArray(m.GetColor(mProp.Key)));
            colorTextures.Add(mProp.Key, MaterialUtils.GenerateColorTexture(64, 32, m.GetColor(mProp.Key)));
          }
          if (mProp.Value == WaterfallMaterialPropertyType.Float)
          {
            floatValues.Add(mProp.Key, m.GetFloat(mProp.Key));
            floatStrings.Add(mProp.Key, m.GetFloat(mProp.Key).ToString());
          }
          if (mProp.Value == WaterfallMaterialPropertyType.Texture)
          {
            textureValues.Add(mProp.Key, m.GetTexture(mProp.Key).name);
            textureScaleValues.Add(mProp.Key, m.GetTextureScale(mProp.Key));
            textureOffsetValues.Add(mProp.Key, m.GetTextureOffset(mProp.Key));
            textureOffsetStrings.Add(mProp.Key, new string[] { $"{m.GetTextureOffset(mProp.Key).x}", $"{m.GetTextureOffset(mProp.Key).y}" });
            textureScaleStrings.Add(mProp.Key, new string[] { $"{m.GetTextureScale(mProp.Key).x}", $"{m.GetTextureScale(mProp.Key).y}" });
          }
        }

      }



    }
  }
}
