using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectRotationIntegrator : EffectIntegrator
  {
    protected readonly List<Vector3> modifierData = new();
    protected readonly List<Vector3> initialValues = new();
    protected readonly List<Vector3> workingValues = new();

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
        mod.Controller?.Get(controllerData);
        var modResult = ((EffectRotationModifier)mod).Get(controllerData, modifierData);
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localEulerAngles = workingValues[i];
    }
  }
}
