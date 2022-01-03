using System;

namespace Waterfall.UI.EffectControllersUI
{
  /// <summary>
  ///     Represents UI component responsible for drawing UI options for specified effect controller type.
  ///     Basically both View and ViewModel for effect controller.
  /// </summary>
  public interface IEffectControllerUIOptions
  {
    /// <summary>
    ///     Draw options according to UI state.
    /// </summary>
    void DrawOptions();

    /// <summary>
    ///     Update UI state from given controller.
    /// </summary>
    void LoadOptions(WaterfallController controller);

    /// <summary>
    ///     Create new effect controller with options from UI state.
    /// </summary>
    WaterfallController CreateController();
  }

  /// <summary>
  ///    Base type for effect controller UI options implementing <see cref="IEffectControllerUIOptions"/>.
  ///    Generic parameter used for both type safety and automatic matching with corresponding controller type.
  /// </summary>
  public class DefaultEffectControllerUIOptions<TController> : IEffectControllerUIOptions where TController : WaterfallController, new()
  {
    public virtual void DrawOptions()
    {
    }

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

    protected virtual void LoadOptions(TController controller)
    {
    }

    public WaterfallController CreateController()
    {
      return CreateControllerInternal();
    }

    protected virtual TController CreateControllerInternal()
    {
      return new TController();
    }
  }
}