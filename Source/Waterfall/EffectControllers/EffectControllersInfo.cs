using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Waterfall
{
  /// <summary>
  ///   Provides list of all effect controllers currently implemented in Waterfall as well as method to create them.
  /// </summary>
  public static class EffectControllersInfo
  {
    public class ControllerInfo
    {
      public readonly string Name;

      private readonly ConstructorInfo _constructor;

      public ControllerInfo(Type controllerType)
      {
        Name = controllerType.GetField("Name").GetValue(null).ToString();

        _constructor = controllerType.GetConstructor(new[] { typeof(ConfigNode) });
        if (_constructor == null)
          throw new InvalidOperationException($"Unable to get ConfigNode constructor for controller of type {controllerType}");
      }

      public WaterfallController CreateController(ConfigNode node)
      {
        var controller = (WaterfallController)_constructor.Invoke(new object[] { node });
        return controller;
      }
    }

    /// <summary>
    ///    Pairs of controller type and it's Name constant value.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, ControllerInfo> EffectControllers;

    static EffectControllersInfo()
    {
      var waterfallAssembly = typeof(EffectControllersInfo).Assembly;
      var baseType = typeof(WaterfallController);
      var controllerTypes = waterfallAssembly
        .GetTypes()
        .Where(t => t != baseType && baseType.IsAssignableFrom(t))
        .ToArray();

      EffectControllers = controllerTypes
        .Select(type => new ControllerInfo(type))
        .ToDictionary(type => type.Name);
    }
  }
}