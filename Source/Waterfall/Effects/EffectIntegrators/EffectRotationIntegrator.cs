using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectRotationIntegrator : EffectIntegrator
  {
    protected List<Vector3> initialValues = new();
    protected List<Vector3> workingValues = new();

    public EffectRotationIntegrator(WaterfallEffect effect, EffectRotationModifier mod) : base(effect, mod)
    {
      foreach (var x in xforms)
        initialValues.Add(x.localEulerAngles);
    }

    public override void Update()
    {
      if (handledModifiers.Count == 0)
        return;
      workingValues.Clear();
      workingValues.AddRange(initialValues);

      foreach (var mod in handledModifiers)
      {
        parentEffect.parentModule.GetControllerValue(mod.controllerName, controllerData);
        var modResult = (mod as EffectRotationModifier).Get(controllerData);
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localEulerAngles = workingValues[i];
    }
  }
}
