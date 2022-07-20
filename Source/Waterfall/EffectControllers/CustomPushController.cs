using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Waterfall
{
  /// <summary>
  ///   Custom push-based controller.
  ///   Other mods can access it and use <see cref="WaterfallController.SetOverride" /> to provide value to this controller.
  /// </summary>
  [Serializable]
  [DisplayName("Custom (Push)")]
  public class CustomPushController : WaterfallController
  {
    public CustomPushController() : base()
    {
      values = new float[1];
    }
    public CustomPushController(ConfigNode node) : base(node) { }

    protected override void UpdateInternal() { }
  }
}
