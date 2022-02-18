using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectPositionIntegrator : EffectIntegrator
  {
    protected List<Vector3> initialValues = new();
    protected List<Vector3> workingValues = new();

    public EffectPositionIntegrator(WaterfallEffect effect, EffectPositionModifier posMod) : base(effect, posMod)
    {
      foreach (var x in xforms)
        initialValues.Add(x.localPosition);
    }

    public override void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      workingValues.Clear();
      workingValues.AddRange(initialValues);

      foreach (var mod in handledModifiers)
      {
        var modResult = (mod as EffectPositionModifier).Get(parentEffect.parentModule.GetControllerValue(mod.controllerName));
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localPosition = workingValues[i];
    }
  }
}
