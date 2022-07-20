using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public class EffectColorIntegrator : EffectIntegrator
  {
    private int colorPropertyID;
    private string _colorName;
    public string colorName
    {
      get { return _colorName; }
      set
      {
        _colorName = value;
        colorPropertyID = Shader.PropertyToID(_colorName);
      }
    }
    protected readonly List<Color> modifierData = new();
    protected readonly List<Color> initialValues = new();
    protected readonly List<Color> workingValues = new();

    private readonly Material[]  m;

    public EffectColorIntegrator(WaterfallEffect effect, EffectColorModifier colorMod) : base(effect, colorMod)
    {
      // color specific
      colorName        = colorMod.colorName;

      m                  = new Material[xforms.Count];
      for (int i = 0; i < xforms.Count; i++)
      {
        m[i] = xforms[i].GetComponent<Renderer>().material;
        initialValues.Add(m[i].GetColor(colorPropertyID));
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
        var modResult = ((EffectColorModifier)mod).Get(controllerData, modifierData);
        Integrate(mod.effectMode, workingValues, modResult);
      }

      for (int i = 0; i < m.Length; i++)
        m[i].SetColor(colorPropertyID, workingValues[i]);
    }
  }
}
