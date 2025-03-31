using System;
using System.ComponentModel;
using System.Linq;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls from RCS throttle
  /// </summary>
  [Serializable]
  [DisplayName("Scalar Module (AnimateGeneric)")]
  public class ScalarModuleController : WaterfallController
  {
    private IScalarModule scalarController;
    [Persistent] public string moduleID = String.Empty;

    public ScalarModuleController() : base() { }
    public ScalarModuleController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      values = new float[1];

      scalarController = host.part.FindModulesImplementing<IScalarModule>().FirstOrDefault(x => x.ScalarModuleID == moduleID);
      if (scalarController == null)
      {
        Utils.LogError($"[ScalarModuleController] Could not find a compatible module with ID {moduleID} on Initialize");
        values = new float[0];
        return;
      }
    }

    protected override float UpdateSingleValue()
    {
      if (scalarController == null)
      {
        Utils.LogWarning("[ScalarModuleController] Scalar module not assigned");
        return 0;
      }
      else 
      {
        return scalarController.GetScalar;
      }
    }
  }
}
