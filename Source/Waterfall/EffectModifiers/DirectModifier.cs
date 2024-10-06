using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waterfall
{
  public abstract class DirectModifier : EffectModifier
  {
    protected DirectModifier() { }

    protected DirectModifier(ConfigNode node) : base(node) { }

    /// <summary>
    ///   Apply the effect with the various combine modes
    /// </summary>
    /// <param name="strength"></param>
    public virtual void Apply(float[] strength)
    {
      UpdateRandomValue();

      switch (effectMode)
      {
        case EffectModifierMode.REPLACE:
          ApplyReplace(strength);
          break;
        case EffectModifierMode.ADD:
          ApplyAdd(strength);
          break;
        case EffectModifierMode.MULTIPLY:
          ApplyMultiply(strength);
          break;
        case EffectModifierMode.SUBTRACT:
          ApplySubtract(strength);
          break;
      }
    }

    public override EffectIntegrator CreateIntegrator()
    {
      // hmm, perhaps there should be a different base class for modifiers that use integrators?
      Utils.LogError($"DirectModifier.CreateIntegrator() called but this has no corresponding integrator!");
      return null;
    }

    protected virtual void ApplyReplace(float[] strength) { }

    protected virtual void ApplyAdd(float[] strength) { }

    protected virtual void ApplyMultiply(float[] strength) { }

    protected virtual void ApplySubtract(float[] strength) { }
  }
}
