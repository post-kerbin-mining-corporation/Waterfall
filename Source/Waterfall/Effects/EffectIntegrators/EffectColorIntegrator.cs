using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectColorIntegrator : EffectIntegrator
  {
    public string                    colorName;
    protected List<Color> modifierData = new();
    protected List<Color> initialValues = new();
    protected List<Color> workingValues = new();

    private readonly Material[]  m;

    public EffectColorIntegrator(WaterfallEffect effect, EffectColorModifier colorMod) : base(effect, colorMod)
    {
      // color specific
      colorName        = colorMod.colorName;

      m                  = new Material[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
        initialValues.Add(m[i].GetColor(colorName));
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
        parentEffect.parentModule.GetControllerValue(mod.controllerName, controllerData);
        var modResult = (mod as EffectColorModifier).Get(controllerData, modifierData);
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < m.Length; i++)
        m[i].SetColor(colorName, workingValues[i]);
    }
  }
}
