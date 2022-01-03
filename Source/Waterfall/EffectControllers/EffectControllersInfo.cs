using System;
using System.Collections.Generic;
using System.Linq;

namespace Waterfall
{
  /// <summary>
  ///     Provides list of all effect controllers currently implemented in Waterfall.
  /// </summary>
  public static class EffectControllersInfo
  {
    /// <summary>
    ///    Pairs of controller type and it's Name constant value.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Type> EffectControllers;

    static EffectControllersInfo()
    {
      var waterfallAssembly = typeof(EffectControllersInfo).Assembly;
      var baseType = typeof(WaterfallController);
      var controllerTypes = waterfallAssembly
        .GetTypes()
        .Where(t => t != baseType && baseType.IsAssignableFrom(t))
        .ToArray();

      EffectControllers = controllerTypes
        .ToDictionary(t => t.GetField("Name").GetValue(null).ToString());

      foreach (var pair in EffectControllers)
      {
        Utils.Log($"CONTROLLER TYPE {pair.Key} loaded {pair.Value.Name}");
      }
    }
  }
}