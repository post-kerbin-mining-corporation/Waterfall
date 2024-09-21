using System;

namespace Waterfall.UI.EffectControllersUI
{
  /// <summary>
  ///   Represents UI component responsible for drawing UI options for specified effect controller type.
  ///   Basically both View and ViewModel for effect controller.
  /// </summary>
  public interface IEffectControllerUIOptions
  {
    /// <summary>
    ///   Draw options according to UI state.
    /// </summary>
    void DrawOptions();

    /// <summary>
    ///   Update UI state from given controller.
    /// </summary>
    void LoadOptions(WaterfallController controller);

    /// <summary>
    /// Can be used to initialized default options from the base module
    /// </summary>
    /// <param name="fx"></param>
    void DefaultOptions(ModuleWaterfallFX fx);

    /// <summary>
    ///   Create new effect controller with options from UI state.
    /// </summary>
    WaterfallController CreateController();
  }

  /// <summary>
  ///   Base type for effect controller UI options implementing <see cref="IEffectControllerUIOptions" />.
  ///   Generic parameter used for both type safety and automatic matching with corresponding controller type.
  /// </summary>
  public class DefaultEffectControllerUIOptions<TController> : IEffectControllerUIOptions where TController : WaterfallController, new()
  {
    public virtual void DrawOptions() { }

    public void LoadOptions(WaterfallController controller)
    {
      if (controller is TController concreteController)
      {
        LoadOptions(concreteController);
      }
      else
      {
        throw new ArgumentException(nameof(controller));
      }
    }
    public virtual void DefaultOptions(ModuleWaterfallFX fx) { }
    
    public WaterfallController CreateController() => CreateControllerInternal();

    /// <summary>
    ///   Must be overridden if controller type have UI options that have to be copied from controller instance.
    /// </summary>
    protected virtual void LoadOptions(TController controller) { }

    /// <summary>
    ///   Must be overridden if controller type have UI options that have to be copied into newly created controller instance.
    /// </summary>
    protected virtual TController CreateControllerInternal() => new();
  }
}