using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectScaleIntegrator : EffectIntegrator
  {
    protected List<Vector3> modifierData = new();
    protected List<Vector3> initialValues = new();
    protected List<Vector3> workingValues = new();
    
    public EffectScaleIntegrator(WaterfallEffect effect, EffectScaleModifier mod) : base(effect, mod)
    {
      foreach (var x in xforms)
        initialValues.Add(x.localScale);
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
        var modResult = (mod as EffectScaleModifier).Get(controllerData, modifierData);
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < xforms.Count; i++)
        xforms[i].localScale = workingValues[i];
    }
  }
}
