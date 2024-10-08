﻿using System;
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
    public float stagedValue = 0f;
    public CustomPushController() : base() { }
    public CustomPushController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      values = new float[1];
    }
    protected override float UpdateSingleValue()
    {
      return stagedValue;
    }
    public override void Set(float newValue)
    {
      stagedValue = newValue;
    }
  }
}
