using System.Collections.Generic;
using UnityEngine;
using Waterfall.UI;


namespace Waterfall
{
  public static class UICopy
  {
    public static FloatCurve        CurveCopyBuffer;
    public static List<ColorSwatch> ColorSwatches    = new List<ColorSwatch>();

    public static void CopyFloatCurve(FloatCurve curve)
    {
      CurveCopyBuffer = curve;
    }
  }

  public class ColorSwatch
  {
    public bool    assigned = false;
    public Color   swatchColor;
    public Texture swatchTexture;

    public ColorSwatch() { }

    public ColorSwatch(Color c)
    {
      swatchColor   = c;
      assigned      = true;
      swatchTexture = TextureUtils.GenerateColorTexture(60, 60, swatchColor);
    }
  }
}