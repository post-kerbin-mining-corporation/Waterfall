using System;
using System.ComponentModel;
using System.Collections;
using UnityEngine;

namespace Waterfall
{
  [Serializable]
  [DisplayName("Remap")]
  public class RemapController : WaterfallController
  {
    [Persistent] public string sourceController = "";
    public FloatCurve mappingCurve = new();

    private WaterfallController source;

    public RemapController() : base() { }
    public RemapController(ConfigNode node) : base(node)
    {
      mappingCurve.Load(node.GetNode(nameof(mappingCurve)));
    }

    public override ConfigNode Save()
    {
      var c = base.Save();
      c.AddNode(Utils.SerializeFloatCurve(nameof(mappingCurve), mappingCurve));
      return c;
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);
      // Temporary value while the source controller gets grabbed.
      values = new float[1];
      // Delay search for source controller as it may be initialized after this controller.
      host.StartCoroutine(FindSourceDelayed());
    }

    private IEnumerator FindSourceDelayed()
    {
      yield return new WaitForEndOfFrame();
      source = parentModule.AllControllersDict[sourceController];
      values = new float[source.Get().Length];
    }

    protected override void UpdateInternal()
    {
      if (source == null) return;
      float[] sourceValues = source.Get();
      for (int i = 0; i < sourceValues.Length; ++i)
      {
        values[i] = mappingCurve.Evaluate(sourceValues[i]);
      }
    }
  }
}
