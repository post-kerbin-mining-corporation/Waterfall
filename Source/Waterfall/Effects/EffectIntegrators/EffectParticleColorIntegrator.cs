using Waterfall.EffectModifiers;

namespace Waterfall
{

  public class EffectParticleColorIntegrator : EffectIntegrator_Color
  {
    private string _colorName;
    public string colorName
    {
      get { return _colorName; }
      set
      {
        _colorName = value;
      }
    }

    private readonly WaterfallParticleSystem[] emits;

    public EffectParticleColorIntegrator(WaterfallEffect effect, EffectParticleColorModifier colorMod) : base(effect, colorMod)
    {

      colorName = colorMod.colorName;

      emits = new WaterfallParticleSystem[xforms.Count];

      for (int i = 0; i < xforms.Count; i++)
      {
        emits[i] = xforms[i].GetComponent<WaterfallParticleSystem>();
        emits[i].Get(colorName, out initialValues[i]);
      }
    }

    protected override void Apply()
    {
      for (int i = 0; i < emits.Length; i++)
        emits[i].Set(colorName, workingValues[i]);
    }
  }
}
