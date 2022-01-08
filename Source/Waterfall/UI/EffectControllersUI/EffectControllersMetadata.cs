using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Waterfall.UI.EffectControllersUI
{
  /// <summary>
  ///   Provides list of all effect controllers currently implemented in Waterfall as well as method to create them.
  /// </summary>
  /// <remarks>
  ///   Unfortunately had to use a little reflection magic to piece together controller name, controller type and ui options type.
  /// </remarks>
  public static class EffectControllersMetadata
  {
    /// <summary>
    ///   Pairs of controller type and it's Name constant value.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, ControllerInfo> EffectControllers;

    /// <summary>
    ///     These were used before field "linkedTo" was replaced by using controller type names for serialization.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> LegacyControllerTypeIds = new Dictionary<string, string>
    {
      ["atmosphere_density"] = nameof(AtmosphereDensityController),
      ["custom"]             = nameof(CustomController),
      ["gimbal"]             = nameof(GimbalController),
      ["light"]              = nameof(LightController),
      ["mach"]               = nameof(MachController),
      ["random"]             = nameof(RandomnessController),
      ["rcs"]                = nameof(RCSController),
      ["throttle"]           = nameof(ThrottleController),
      ["thrust"]             = nameof(ThrustController),
    };

    static EffectControllersMetadata()
    {
      var waterfallAssembly = typeof(EffectControllersMetadata).Assembly;
      var baseType          = typeof(WaterfallController);
      var controllerTypes = waterfallAssembly
        .GetTypes()
        .Where(t => t != baseType && baseType.IsAssignableFrom(t))
        .ToArray();

      EffectControllers = controllerTypes
        .Select(type => new ControllerInfo(type))
        .ToDictionary(type => type.ControllerType.Name);
    }

    public class ControllerInfo
    {
      private readonly ConstructorInfo deserializeConstructor;

      public readonly Type   ControllerType;
      public readonly string DisplayName;

      public ControllerInfo(Type controllerType)
      {
        ControllerType = controllerType;
        DisplayName    = controllerType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? ControllerType.Name;

        deserializeConstructor = controllerType.GetConstructor(new[] { typeof(ConfigNode) });

        if (deserializeConstructor == null)
          throw new InvalidOperationException($"Unable to get ConfigNode constructor for controller of type {controllerType}");

        Utils.Log($"[{nameof(EffectControllersMetadata)}]: Registered controller type {ControllerType}", LogType.Modules);
      }

      public WaterfallController CreateFromConfig(ConfigNode node)
      {
        var controller = (WaterfallController)deserializeConstructor.Invoke(new object[] { node });
        return controller;
      }

      /// <summary>
      ///   Find corresponding type implementing <see cref="IEffectControllerUIOptions" /> and create new instance.
      ///   Inject UIResources dependency if necessary.
      /// </summary>
      public IEffectControllerUIOptions CreateUIOptions(UIResources guiResources)
      {
        var waterfallAssembly = typeof(EffectControllersMetadata).Assembly;
        var baseType          = typeof(DefaultEffectControllerUIOptions<>);

        var optionsType = waterfallAssembly
          .GetTypes()
          .First(t => t.BaseType is { IsConstructedGenericType: true }
                      && t.BaseType.GetGenericTypeDefinition()            == baseType
                      && t.BaseType.GenericTypeArguments.FirstOrDefault() == ControllerType);

        object options = optionsType.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0])
                         ?? optionsType.GetConstructor(new[] { typeof(UIResources) })?.Invoke(new object[] { guiResources });

        if (options == null)
          throw new InvalidOperationException($"Unable to construct UI options for type {ControllerType}");

        return (IEffectControllerUIOptions)options;
      }
    }
  }
}