// Utils

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
              || (logType == LogType.Modules && Settings.DebugModules)
              || (logType == LogType.Effects && Settings.DebugEffects)
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

    public static ConfigNode SerializeFloatCurve(string name, FloatCurve curve)
    {
      var node = new ConfigNode();
      curve.Save(node);
      node.name = name;
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
  }

  public static class TransformDeepChildExtension
  {
    //Breadth-first search
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