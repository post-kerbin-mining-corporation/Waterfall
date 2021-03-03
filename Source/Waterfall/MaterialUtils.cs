using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

    public static Texture2D GenerateGradientTexture(int texWidth, int texHeight, Color startColor, Color endColor)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, false);

      Color c;
      //int xAlphaEnd = (int)Mathf.Round(color.a * (float)texWidth);
      for (int x = 0; x < texWidth; x++)
      {
        c = Color.Lerp(startColor, endColor, (float)x / (float)texWidth);
        for (int y = 0; y < texHeight; y++)
        {
          tex.SetPixel(x, y, c);
        }
      }
      tex.Apply();
      return tex;
    }
    public static Texture2D GenerateRainbowGradient(int texWidth, int texHeight)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, false);

      Color c;
      //int xAlphaEnd = (int)Mathf.Round(color.a * (float)texWidth);
      for (int x = 0; x < texWidth; x++)
      {

        c = HSBColor.ToColor(new HSBColor((float)x / (float)texWidth, 1f, 1f));
        for (int y = 0; y < texHeight; y++)
        {
          tex.SetPixel(x, y, c);
        }
      }
      tex.Apply();
      return tex;
    }
    public static Texture2D GenerateColorField(int texWidth, int texHeight, Color targetColor)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, false);
      HSBColor hue = new HSBColor(targetColor);
      Color c;
      //int xAlphaEnd = (int)Mathf.Round(color.a * (float)texWidth);
      for (int x = 0; x < texWidth; x++)
      {
        for (int y = 0; y < texHeight; y++)
        {
          c = new HSBColor(hue.h, (float)x / (float)texWidth, (float)y / (float)texHeight).ToColor();
          tex.SetPixel(x, y, c);
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
    public static List<string> FindValidShaderProperties(Material m, WaterfallMaterialPropertyType propType)
    {
      List<string> validProps = new List<string>();
      foreach (KeyValuePair<string, MaterialData> mProp in ShaderLoader.GetShaderPropertyMap())
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


  [System.Serializable]
  public struct HSBColor
  {
    public float h;
    public float s;
    public float b;
    public float a;

    public HSBColor(float h, float s, float b, float a)
    {
      this.h = h;
      this.s = s;
      this.b = b;
      this.a = a;
    }

    public HSBColor(float h, float s, float b)
    {
      this.h = h;
      this.s = s;
      this.b = b;
      this.a = 1f;
    }

    public HSBColor(Color col)
    {
      HSBColor temp = FromColor(col);
      h = temp.h;
      s = temp.s;
      b = temp.b;
      a = temp.a;
    }

    public static HSBColor FromColor(Color color)
    {
      HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);

      float r = color.r;
      float g = color.g;
      float b = color.b;

      float max = Mathf.Max(r, Mathf.Max(g, b));

      if (max <= 0)
      {
        return ret;
      }

      float min = Mathf.Min(r, Mathf.Min(g, b));
      float dif = max - min;

      if (max > min)
      {
        if (g == max)
        {
          ret.h = (b - r) / dif * 60f + 120f;
        }
        else if (b == max)
        {
          ret.h = (r - g) / dif * 60f + 240f;
        }
        else if (b > g)
        {
          ret.h = (g - b) / dif * 60f + 360f;
        }
        else
        {
          ret.h = (g - b) / dif * 60f;
        }
        if (ret.h < 0)
        {
          ret.h = ret.h + 360f;
        }
      }
      else
      {
        ret.h = 0;
      }

      ret.h *= 1f / 360f;
      ret.s = (dif / max) * 1f;
      ret.b = max;

      return ret;
    }

    public static Color ToColor(HSBColor hsbColor)
    {
      float r = hsbColor.b;
      float g = hsbColor.b;
      float b = hsbColor.b;
      if (hsbColor.s != 0)
      {
        float max = hsbColor.b;
        float dif = hsbColor.b * hsbColor.s;
        float min = hsbColor.b - dif;

        float h = hsbColor.h * 360f;

        if (h < 60f)
        {
          r = max;
          g = h * dif / 60f + min;
          b = min;
        }
        else if (h < 120f)
        {
          r = -(h - 120f) * dif / 60f + min;
          g = max;
          b = min;
        }
        else if (h < 180f)
        {
          r = min;
          g = max;
          b = (h - 120f) * dif / 60f + min;
        }
        else if (h < 240f)
        {
          r = min;
          g = -(h - 240f) * dif / 60f + min;
          b = max;
        }
        else if (h < 300f)
        {
          r = (h - 240f) * dif / 60f + min;
          g = min;
          b = max;
        }
        else if (h <= 360f)
        {
          r = max;
          g = min;
          b = -(h - 360f) * dif / 60 + min;
        }
        else
        {
          r = 0;
          g = 0;
          b = 0;
        }
      }

      return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
    }

    public Color ToColor()
    {
      return ToColor(this);
    }

    public override string ToString()
    {
      return "H:" + h + " S:" + s + " B:" + b;
    }

    public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
    {
      float h, s;

      //check special case black (color.b==0): interpolate neither hue nor saturation!
      //check special case grey (color.s==0): don't interpolate hue!
      if (a.b == 0)
      {
        h = b.h;
        s = b.s;
      }
      else if (b.b == 0)
      {
        h = a.h;
        s = a.s;
      }
      else
      {
        if (a.s == 0)
        {
          h = b.h;
        }
        else if (b.s == 0)
        {
          h = a.h;
        }
        else
        {
          // works around bug with LerpAngle
          float angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
          while (angle < 0f)
            angle += 360f;
          while (angle > 360f)
            angle -= 360f;
          h = angle / 360f;
        }
        s = Mathf.Lerp(a.s, b.s, t);
      }
      return new HSBColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
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

      foreach (KeyValuePair<string, MaterialData> mProp in ShaderLoader.GetShaderPropertyMap())
      {
        if (m.HasProperty(mProp.Key))
        {
          if (mProp.Value.type == WaterfallMaterialPropertyType.Color)
          {
            colorValues.Add(mProp.Key, m.GetColor(mProp.Key));
            colorStrings.Add(mProp.Key, MaterialUtils.ColorToStringArray(m.GetColor(mProp.Key)));
            colorTextures.Add(mProp.Key, MaterialUtils.GenerateColorTexture(64, 32, m.GetColor(mProp.Key)));
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
            textureOffsetStrings.Add(mProp.Key, new string[] { $"{m.GetTextureOffset(mProp.Key).x}", $"{m.GetTextureOffset(mProp.Key).y}" });
            textureScaleStrings.Add(mProp.Key, new string[] { $"{m.GetTextureScale(mProp.Key).x}", $"{m.GetTextureScale(mProp.Key).y}" });
          }
        }
      }
    }
  }
  public class MaterialData
  {
    public Vector2 floatRange;
    public WaterfallMaterialPropertyType type;
    public MaterialData (WaterfallMaterialPropertyType theType, Vector2 range)
    {
      type = theType;
      floatRange = range;
    }
    public MaterialData(WaterfallMaterialPropertyType theType)
    {
      type = theType;
    }
  }
}