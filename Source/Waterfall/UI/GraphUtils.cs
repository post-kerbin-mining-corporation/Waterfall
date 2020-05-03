using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall.UI
{
  public static class GraphUtils
  {


    /// <summary>
    /// Convertes a FloatCurve into a list of FloatString points
    /// </summary>
    /// <param name="inCurve"></param>
    /// <returns></returns>
    public static List<FloatString4> FloatCurveToPoints(FloatCurve inCurve)
    {
      List<FloatString4> outPoints = new List<FloatString4>();
      foreach (Keyframe kf in inCurve.Curve.keys)
      {
        outPoints.Add(new FloatString4(kf.time, kf.value, kf.inTangent, kf.outTangent));
      }
      return outPoints;
    }

    /// <summary>
    /// Converts a list of FloatString points into their string representation 
    /// </summary>
    /// <param name="inPoints"></param>
    /// <returns></returns>
    public static string PointsToString(List<FloatString4> inPoints)
    {
      string buff = "";
      foreach (FloatString4 p in inPoints)
      {
        buff += $"key = {p.floats.x} {p.floats.y} {p.floats.z} {p.floats.w}";
      }
      return buff;
    }

    /// <summary>
    /// Parses a string into a list of FloatString points
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static List<FloatString4> StringToPoints(string data)
    {
      List<FloatString4> newPoints = new List<FloatString4>();

      string[] lines = data.Split('\n');
      foreach (string line in lines)
      {
        string[] pcs = line.Trim().Split(new char[] { '=', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if ((pcs.Length >= 3) && (pcs[0] == "key"))
        {
          FloatString4 nv = new FloatString4();
          if (pcs.Length >= 5)
          {
            nv.strings = new string[] { pcs[1], pcs[2], pcs[3], pcs[4] };
          }
          else
          {

            nv.strings = new string[] { pcs[1], pcs[2], "0", "0" };
          }
          nv.UpdateFloats();
          newPoints.Add(nv);

        }
      }
      return newPoints;
    }
    
    public static Texture2D GenerateCurveTexture(int texWidth, int texHeight, FloatCurve curve, Color curveColor)
    {
      Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, true);
      float minY = float.MaxValue;
      float maxY = float.MinValue;

      //textVersion = CurveToString();
      Color bgColor = new Color(.39f, .39f, .39f, 1f);
      Color lineColor = new Color(.33f, .33f, .33f, 1f);
      Color outsideColor = new Color(.16f, .16f, .16f, 1f);
      for (int x = 0; x < texWidth; x++)
      {
        for (int y = 0; y < texHeight; y++)
        {
          tex.SetPixel(x, y, bgColor);
          if (y==0 || y == (texHeight - 1) )
          {
            tex.SetPixel(x, y, outsideColor);
          }
        }

        if (x == 0 || x == texWidth - 1)
        {
          for (int y = 0; y < texHeight; y++)
          {
            tex.SetPixel(x, y, outsideColor);
          }
        }

        float fY = curve.Evaluate(curve.minTime + curve.maxTime * x / (texWidth - 1));
        minY = Mathf.Min(minY, fY);
        maxY = Mathf.Max(maxY, fY);
      }

      for (int x = 0; x < texWidth; x++)
      {
        float step = texHeight / (float)4;
        for (int y = 0; y < 4; y++)
        {
          tex.SetPixel(x, Mathf.RoundToInt(y * step), lineColor); 
        }

        
      }

      for (int x = 1; x < texWidth -1; x++)
      {
        float fY = curve.Evaluate(curve.minTime + curve.maxTime * x / (texWidth - 1));
        tex.SetPixel(x, Mathf.RoundToInt((fY - minY) / (maxY - minY) * (texHeight - 1)), curveColor);
      }
      tex.Apply();
      return tex;
    }
    
  }

  public class FloatString4 : IComparable<FloatString4>
  {
    public Vector4 floats;
    public string[] strings;

    public int CompareTo(FloatString4 other)
    {
      if (other == null)
      {
        return 1;
      }
      return floats.x.CompareTo(other.floats.x);
    }

    public FloatString4()
    {
      floats = new Vector4();
      strings = new string[] { "0", "0", "0", "0" };
    }

    public FloatString4(float x, float y, float z, float w)
    {
      floats = new Vector4(x, y, z, w);
      UpdateStrings();
    }

    public void UpdateFloats()
    {
      float x, y, z, w;
      float.TryParse(strings[0], out x);
      float.TryParse(strings[1], out y);
      float.TryParse(strings[2], out z);
      float.TryParse(strings[3], out w);
      floats = new Vector4(x, y, z, w);
    }

    public void UpdateStrings()
    {
      strings = new string[] { floats.x.ToString(), floats.y.ToString(), floats.z.ToString(), floats.w.ToString() };
    }
  }
}
