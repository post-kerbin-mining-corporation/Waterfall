using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectLightColorIntegrator : EffectIntegrator
  {
    public string                         colorName;
    protected readonly List<Color> modifierData = new();
    protected readonly List<Color> initialValues = new();
    protected readonly List<Color> workingValues = new();

    private readonly Light[]     l;

    public EffectLightColorIntegrator(WaterfallEffect effect, EffectLightColorModifier floatMod) : base(effect, floatMod)
    {
      // light-color specific
      colorName        = floatMod.colorName;
      l = new Light[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        l[i] = xforms[i].GetComponent<Light>();
        initialValues.Add(l[i].color);
      }
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
        var modResult = (mod as EffectLightColorModifier).Get(controllerData, modifierData);
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < l.Length; i++)
        l[i].color = workingValues[i];
    }
  }
}
