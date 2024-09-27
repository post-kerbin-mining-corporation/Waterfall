using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public enum LogType
  {
    UI,
    Settings,
    Modules,
    Effects,
    Modifiers,
    Particles,
    Loading,
    Any
  }

  public static class Utils
  {
    public static string ModName = "Waterfall";

    /// <summary>
    ///   Log a message with the mod name tag prefixed
    /// </summary>
    /// <param name="str">message string </param>
    public static void Log(string str)
    {
      Log(str, LogType.Any);
    }

    /// <summary>
    /// Is logging enabled?
    /// </summary>
    /// <param name="logType">Logging Type</param>
    /// <returns>True if logging is enabled</returns>
    public static bool IsLogging(LogType logType = LogType.Any)
    {
      return logType == LogType.Any
              || (logType == LogType.Settings && Settings.DebugSettings)
              || (logType == LogType.UI && Settings.DebugUIMode)
              || (logType == LogType.Loading && Settings.DebugLoading)
              || (logType == LogType.Modules && Settings.DebugModules)
              || (logType == LogType.Effects && Settings.DebugEffects)
              || (logType == LogType.Effects && Settings.DebugParticles)
              || (logType == LogType.Modifiers && Settings.DebugModifiers);
    }

    /// <summary>
    ///   Log a message with the mod name tag prefixed
    /// </summary>
    /// <param name="str">message string </param>
    public static void Log(string str, LogType logType)
    {
      if (IsLogging(logType))
        Debug.Log($"[{ModName}]{str}");
    }

    /// <summary>
    ///   Log an error with the mod name tag prefixed
    /// </summary>
    /// <param name="str">Error string </param>
    public static void LogError(string str)
    {
      Debug.LogError(String.Format("[{0}]{1}", ModName, str));
    }

    /// <summary>
    ///   Log a warning with the mod name tag prefixed
    /// </summary>
    /// <param name="str">warning string </param>
    public static void LogWarning(string str)
    {
      Debug.LogWarning(String.Format("[{0}]{1}", ModName, str));
    }


    public static Gradient CreateGradientFromCurves(FloatCurve r, FloatCurve g, FloatCurve b, FloatCurve a, out float lower, out float upper, float timeMergeTolerance = 0.01f)
    {

      // Need to convert real space keys into gradient space keys (0-1)
      Keyframe[] redKeys = r.Curve.keys;
      Keyframe[] greenKeys = g.Curve.keys;
      Keyframe[] blueKeys = b.Curve.keys;
      Keyframe[] alphaKeys = a.Curve.keys;

      float rCurveMax = 1f;
      float rCurveMin = 0f;
      float gCurveMax = 1f;
      float gCurveMin = 0f;
      float bCurveMax = 1f;
      float bCurveMin = 0f;
      float aCurveMax = 1f;
      float aCurveMin = 0f;

      if (r.Curve.length > 0)
      {
        rCurveMax = redKeys[redKeys.Length - 1].time;
        rCurveMin = redKeys[0].time;
      }
      if (g.Curve.length > 0)
      {
        gCurveMax = greenKeys[greenKeys.Length - 1].time;
        gCurveMin = greenKeys[0].time;
      }
      if (b.Curve.length > 0)
      {
        bCurveMax = blueKeys[blueKeys.Length - 1].time;
        bCurveMin = blueKeys[0].time;
      }
      if (a.Curve.length > 0)
      {
        aCurveMax = alphaKeys[alphaKeys.Length - 1].time;
        aCurveMin = alphaKeys[0].time;
      }

      lower = Mathf.Min(rCurveMin, gCurveMin, bCurveMin, aCurveMin);
      upper = Mathf.Max(rCurveMax, gCurveMax, bCurveMax, aCurveMax);

      float offset = 0 - lower;
      float gain = 1f / (upper - offset);

      Gradient grad = new();
      List<GradientColorKey> gradientColorKeys = new List<GradientColorKey>();
      GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[alphaKeys.Length];

      for (int i = 0; i < redKeys.Length; i++)
      {
        float adjTime = (redKeys[i].time - offset) * gain;
        GradientColorKey newKey = new(
          new Color(redKeys[i].value, g.Evaluate(redKeys[i].time), b.Evaluate(redKeys[i].time)),
          adjTime
          );
        gradientColorKeys.Add(newKey);
      }

      for (int i = 0; i < greenKeys.Length; i++)
      {
        float adjTime = (greenKeys[i].time - offset) * gain;

        bool skip = false;
        foreach (GradientColorKey checkKey in gradientColorKeys)
        {
          if (Mathf.Abs(adjTime - checkKey.time) < timeMergeTolerance)
            skip = true;
        }

        if (!skip)
        {
          GradientColorKey newKey = new(
            new Color(r.Evaluate(greenKeys[i].time), greenKeys[i].value, b.Evaluate(greenKeys[i].time)),
            adjTime
            );
          gradientColorKeys.Add(newKey);
        }
      }
      for (int i = 0; i < blueKeys.Length; i++)
      {
        float adjTime = (blueKeys[i].time - offset) * gain;

        bool skip = false;
        foreach (GradientColorKey checkKey in gradientColorKeys)
        {
          if (Mathf.Abs(adjTime - checkKey.time) < timeMergeTolerance)
            skip = true;
        }

        if (!skip)
        {
          GradientColorKey newKey = new(
            new Color(r.Evaluate(blueKeys[i].time), g.Evaluate(blueKeys[i].time), blueKeys[i].value),
            adjTime
            );
          gradientColorKeys.Add(newKey);
        }
      }
      for (int i = 0; i < alphaKeys.Length; i++)
      {
        gradientAlphaKeys[i] = new(alphaKeys[i].value, (alphaKeys[i].time - offset) * gain);
      }
      grad.SetKeys(gradientColorKeys.ToArray(), gradientAlphaKeys);
      return grad;
    }
    public static void CreateCurvesFromGradient(Gradient gradient, out FloatCurve r, out FloatCurve g, out FloatCurve b, out FloatCurve a, float lower = 0f, float upper = 1f)
    {
      /// Put 0-1 gradient keys into real space
      float offset = 0 - lower;
      float gain = 1f / (upper - offset);
      r = new();
      g = new();
      b = new();
      a = new();
      foreach (GradientColorKey key in gradient.colorKeys)
      {
        r.Add(key.time * gain + offset, key.color.r);
        g.Add(key.time * gain + offset, key.color.g);
        b.Add(key.time * gain + offset, key.color.b);
      }
      foreach (GradientAlphaKey key in gradient.alphaKeys)
      {
        a.Add(key.time * gain + offset, key.alpha);
      }
    }
    public static ConfigNode SerializeFloatCurve(string name, FloatCurve curve)
    {
      ConfigNode node = new();
      curve.Save(node);
      node.name = name;
      return node;
    }
    public static ConfigNode SerializeGradient(string name, Gradient gradient, float lower = 0f, float upper = 1f)
    {
      CreateCurvesFromGradient(gradient, out FloatCurve r, out FloatCurve g, out FloatCurve b, out FloatCurve a, lower, upper);

      ConfigNode node = new();
      node.name = name;

      node.AddNode(SerializeFloatCurve("red", r));
      node.AddNode(SerializeFloatCurve("green", g));
      node.AddNode(SerializeFloatCurve("blue", b));
      node.AddNode(SerializeFloatCurve("alpha", a));
      return node;
    }


  }

  public static class ConfigNodeParseExtension
  {
    public static bool TryParseVector3(this ConfigNode theNode, string valueName, ref Vector3 result)
    {
      if (!theNode.HasValue(valueName))
        return false;

      result = ConfigNode.ParseVector3(theNode.GetValue(valueName));
      return true;
    }
    public static bool TryParseGradient(this ConfigNode node, ref Gradient result)
    {
      FloatCurve rCurve = new();
      FloatCurve gCurve = new();
      FloatCurve bCurve = new();
      FloatCurve aCurve = new();
      ConfigNode curveNode = new();

      bool failed = false;
      if (node.TryGetNode("red", ref curveNode))
      {
        rCurve.Load(curveNode);
      }
      else
      {
        failed = true;
      }
      if (node.TryGetNode("green", ref curveNode))
      {
        gCurve.Load(curveNode);
      }
      else
      {
        failed = true;
      }
      if (node.TryGetNode("blue", ref curveNode))
      {
        bCurve.Load(curveNode);
      }
      else
      {
        failed = true;
      }
      if (node.TryGetNode("alpha", ref curveNode))
      {
        aCurve.Load(curveNode);
      }
      else
      {
        failed = true;
      }
      if (!failed)
      {
        result = Utils.CreateGradientFromCurves(rCurve, gCurve, bCurve, aCurve, out float l, out float u); ;
      }

      return failed;
    }
  }

  public static class TransformDeepChildExtension
  {
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
      var queue = new Queue<Transform>();
      queue.Enqueue(aParent);
      while (queue.Count > 0)
      {
        var c = queue.Dequeue();
        if (c.name == aName)
          return c;
        foreach (Transform t in c)
          queue.Enqueue(t);
      }
      return null;
    }

  }
}