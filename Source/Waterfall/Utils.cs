// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    Any
  }
  public static class Utils
  {
    public static string ModName = "Waterfall";

    /// <summary>
    /// Log a message with the mod name tag prefixed
    /// </summary>
    /// <param name="str">message string </param>
    public static void Log(string str)
    {
      Utils.Log(str, LogType.Any);
    }

    /// <summary>
    /// Log a message with the mod name tag prefixed
    /// </summary>
    /// <param name="str">message string </param>
    public static void Log(string str, LogType logType)
    {
      bool doLog = false;
      if (logType == LogType.Settings && Settings.DebugSettings) doLog = true;
      if (logType == LogType.UI && Settings.DebugUIMode) doLog = true;
      if (logType == LogType.Modules && Settings.DebugModules) doLog = true;
      if (logType == LogType.Effects && Settings.DebugEffects) doLog = true;
      if (logType == LogType.Modifiers && Settings.DebugModifiers) doLog = true;
      if (logType == LogType.Any) doLog = true;

      if (doLog)
        Debug.Log(String.Format("[{0}]{1}", ModName, str));
    }

    /// <summary>
    /// Log an error with the mod name tag prefixed
    /// </summary>
    /// <param name="str">Error string </param>
    public static void LogError(string str)
    {
      Debug.LogError(String.Format("[{0}]{1}", ModName, str));
    }

    /// <summary>
    /// Log a warning with the mod name tag prefixed
    /// </summary>
    /// <param name="str">warning string </param>
    public static void LogWarning(string str)
    {
      Debug.LogWarning(String.Format("[{0}]{1}", ModName, str));
    }

    public static ConfigNode SerializeFloatCurve(string name, FloatCurve curve)
    {
      ConfigNode node = new ConfigNode();
      curve.Save(node);
      node.name = name;
      return node;
    }
  }

  public static class TransformDeepChildExtension
  {
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
      Queue<Transform> queue = new Queue<Transform>();
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
