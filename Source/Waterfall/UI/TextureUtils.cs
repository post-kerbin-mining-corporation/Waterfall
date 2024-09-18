using UnityEngine;

namespace Waterfall.UI
{
  /// <summary>
  /// Utilities to generate useful UI textures
  /// TODO: ALL OF THESE FUNCTIONS COULD BE OPTIMIZED
  /// </summary>
  public static class TextureUtils
  {
    internal static TextureFormat texFormat = TextureFormat.ARGB32;

    public static Texture2D GenerateFlatColorTexture(int texWidth, int texHeight, Color color)
    {
      Texture2D tex = new (texWidth, texHeight, texFormat, false, false);
      Color c = new (color.r, color.g, color.b, 1.0f);

      for (int x = 0; x < texWidth; x++)
      {
        for (int y = 0; y < texHeight; y++)
        {
          tex.SetPixel(x, y, c);
        }
      }
      tex.Apply();
      return tex;
    }
    public static Texture2D GenerateSliderCarat(int texWidth,
      int   texHeight,
      Color caratColor,
      Color caratBorderColor,
      int   borderWidth = 1)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, texFormat, false, false);

      for (int x = 0; x < texWidth; x++)
      {

        for (int y = 0; y < texHeight; y++)
        {
          if (x < borderWidth || x > texWidth - 1 - borderWidth)
            tex.SetPixel(x, y, caratBorderColor);
          else if (y < borderWidth || y > texHeight - 1 - borderWidth)
            tex.SetPixel(x, y, caratBorderColor);
          else
            tex.SetPixel(x, y, caratColor);

        }
      }
      tex.Apply();
      return tex;
    }
    public static Texture2D GenerateRoundCarat(int texWidth,
      int   texHeight,
      Color caratColor,
      Color caratBorderColor)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, texFormat, false, false);

      for (int x = 0; x < texWidth; x++)
      {

        for (int y = 0; y < texHeight; y++)
        {

          tex.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));

        }
      }
      tex.Apply();
      int centerX = texWidth / 2;
      int centerY = texHeight / 2;
      int radius = texHeight / 2;

      TextureUtils.DrawCircle(tex, centerX, centerY, radius - 4, caratBorderColor);
      TextureUtils.DrawCircle(tex, centerX, centerY, radius, caratBorderColor);
      TextureUtils.DrawCircle(tex, centerX, centerY, radius - 2, caratColor);

      tex.SetPixel(centerX, centerY, caratBorderColor);

      tex.Apply();
      return tex;
    }

    public static Texture2D GenerateCheckerboard(int texWidth, int texHeight, Color background, Color foreground, int gridSize)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, texFormat, false, false);
      for (int x = 0; x < texWidth; x++)
      {
        int xID = x / gridSize;
        for (int y = 0; y < texHeight; y++)
        {
          int yID = y / gridSize;

          if (yID % 2 == 0)
          {
            if (xID % 2 == 1)
            {
              tex.SetPixel(x, y, background);
            }

            else
            {
              tex.SetPixel(x, y, foreground);
            }
          }
          if (yID % 2 == 1)
          {
            if (xID % 2 == 0)
            {
              tex.SetPixel(x, y, background);
            }

            else
            {
              tex.SetPixel(x, y, foreground);
            }
          }
        }
      }
      tex.Apply();
      return tex;
    }
    public static Texture2D DrawCircle(Texture2D tex, int centerX, int centerY, int radius, Color circleColor)
    {
      int d = 3 - (2 * radius);
      int x = 0;
      int y = radius;

      while (x < y)
      {
        tex.SetPixel(centerX + x, centerY + y, circleColor);
        tex.SetPixel(centerX + x, centerY - y, circleColor);
        tex.SetPixel(centerX - x, centerY + y, circleColor);
        tex.SetPixel(centerX - x, centerY - y, circleColor);
        tex.SetPixel(centerX + y, centerY + x, circleColor);
        tex.SetPixel(centerX + y, centerY - x, circleColor);
        tex.SetPixel(centerX - y, centerY + x, circleColor);
        tex.SetPixel(centerX - y, centerY - x, circleColor);
        if (d < 0)
        {
          d = d + (4 * x) + 6;
        }
        else
        {
          d = d + 4 * (x - y) + 10;
          y--;
        }
        x++;
      }
      return tex;
    }
    public static Texture2D GenerateColorTexture(int texWidth, int texHeight, Color color)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, texFormat, false, false);
      Color c       = new Color(color.r, color.g, color.b, 1.0f);
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
      Texture2D tex = new Texture2D(texWidth, texHeight, texFormat, false, false);

      Color c;
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


    public static Texture2D GenerateGradientTexture(int texWidth, int texHeight, Gradient g)
    {
      Texture2D tex = GenerateCheckerboard(texWidth, texHeight, Color.white, Color.grey, 8);

      float mTime = g.colorKeys[g.colorKeys.Length - 1].time;
      Color c;

      for (int x = 0; x < texWidth; x++)
      {
        c = g.Evaluate(x / (float)texWidth * mTime);
        for (int y = 0; y < texHeight; y++)
        {

          Color baseCol = tex.GetPixel(x, y);
          Color cN = new Color(
            baseCol.r * (1f - c.a) + c.r * c.a,
            baseCol.g * (1f - c.a) + c.g * c.a,
            baseCol.b * (1f - c.a) + c.b * c.a,
            1f);
          tex.SetPixel(x, y, cN);
        }
      }
      tex.Apply();
      return tex;
    }

    public static Texture2D GenerateRainbowGradient(int texWidth, int texHeight)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, texFormat, false, false);

      Color c;
      for (int x = 0; x < texWidth; x++)
      {

        c = new ColorHSV((float)x / (float)texWidth, 1f, 1f).ToRGB();
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
      return GenerateColorField(texWidth, texHeight, new ColorHSV(targetColor));
    }
    public static Texture2D GenerateColorField(int texWidth, int texHeight, ColorHSV hsv)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, texFormat, false, false);

      Color c;

      for (int x = 0; x < texWidth; x++)
      {
        for (int y = 0; y < texHeight; y++)
        {
          c = new ColorHSV(hsv.h, (float)x / (float)texWidth, (float)y / (float)texHeight).ToRGB();
          tex.SetPixel(x, y, c);
        }
      }
      tex.Apply();
      return tex;
    }
  }
}