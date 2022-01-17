using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// Stores a Color as a Hue/Saturation/Value model with components 0-1
  /// </summary>
  [System.Serializable]
  public class ColorHSV
  {
    public float h = 0.5f;
    public float s = 0.5f;
    public float v = 0.5f;
    public float a = 1f;
    public ColorHSV() { }

    /// <summary>
    /// New HSV color from components
    /// </summary>
    /// <param name="hue"></param>
    /// <param name="sat"></param>
    /// <param name="value"></param>
    public ColorHSV(float hue, float sat, float value)
    {
      h = hue; s = sat; v = value; a = 1f;
    }
    /// <summary>
    /// New HSV color from components
    /// </summary>
    /// <param name="hue"></param>
    /// <param name="sat"></param>
    /// <param name="value"></param>
    /// <param name="alpha"></param>
    public ColorHSV(float hue, float sat, float value, float alpha)
    {
      h = hue; s = sat; v = value; a = alpha;
    }

    /// <summary>
    /// New HSV color from RGBA
    /// </summary>
    /// <param name="colorRGB"></param>
    public ColorHSV(Color colorRGB)
    {
      Color.RGBToHSV(colorRGB, out h, out s, out v);
      a = colorRGB.a;
    }

    /// <summary>
    /// Tests color equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ColorHSV other)
    {
      return this.h == other.h && this.s == other.s && this.v == other.v && this.a == other.a;
    }

    public override string ToString()
    {
      return $"ColorHSV({h}, {s}, {v}, {a})";
    }

    /// <summary>
    /// Returns this color as RGBA
    /// </summary>
    /// <returns></returns>
    public Color ToRGB()
    {
      Color newC = Color.HSVToRGB(h, s, v);
      newC = new Color(newC.r, newC.g, newC.b, a);
      return newC;
    }
  }
}