using System;
using System.Collections.Generic;
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
  public static class EffectControllersInfo
  {
    public class ControllerInfo
    {
      public readonly string Name;
      public readonly string DisplayName;

      private readonly Type controllerType;
      private readonly ConstructorInfo configNodeConstructor;
      private readonly ConstructorInfo parameterlessConstructor;

      public ControllerInfo(Type controllerType)
      {
        this.controllerType = controllerType;
        Name = controllerType.GetField("Name").GetValue(null).ToString();

        configNodeConstructor = controllerType.GetConstructor(new[] { typeof(ConfigNode) });
        parameterlessConstructor = controllerType.GetConstructor(Type.EmptyTypes);

        if (configNodeConstructor == null)
          throw new InvalidOperationException($"Unable to get ConfigNode constructor for controller of type {controllerType}");

        if (parameterlessConstructor == null)
          throw new InvalidOperationException($"Unable to get parameterless constructor for controller of type {controllerType}");

        DisplayName = CreateNew().DisplayName; // Create dummy controller and save display name which is expected to be constant
      }

      public WaterfallController CreateFromConfig(ConfigNode node)
      {
        var controller = (WaterfallController)configNodeConstructor.Invoke(new object[] { node });
        return controller;
      }

      public WaterfallController CreateNew()
      {
        var controller = (WaterfallController)parameterlessConstructor.Invoke(new object[] { });
        return controller;
      }

      /// <summary>
      ///     Find corresponding type implementing <see cref="IEffectControllerUIOptions"/> and create new instance.
      ///     Inject UIResources dependency if necessary.
      /// </summary>
      public IEffectControllerUIOptions CreateUIOptions(UIResources guiResources)
      {
        var waterfallAssembly = typeof(EffectControllersInfo).Assembly;
        var baseType = typeof(DefaultEffectControllerUIOptions<>);

        var optionsType = waterfallAssembly
          .GetTypes()
          .First(t => t.BaseType is { IsConstructedGenericType: true }
                      && t.BaseType.GetGenericTypeDefinition() == baseType
                      && t.BaseType.GenericTypeArguments.FirstOrDefault() == controllerType);

        object options = optionsType.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0])
                         ?? optionsType.GetConstructor(new[] { typeof(UIResources) })?.Invoke(new object[] { guiResources });

        if (options == null)
          throw new InvalidOperationException($"Unable to construct UI options for type {controllerType}");

        return (IEffectControllerUIOptions)options;
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