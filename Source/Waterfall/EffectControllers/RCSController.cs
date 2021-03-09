using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waterfall
{

  /// <summary>
  /// A controller that pulls from throttle settings
  /// </summary>
  [Serializable]
  public class RCSController : WaterfallController
  {
    public float currentThrottle = 1;
    public string thrusterTransformName = "";
    ModuleRCSFX rcsController;
    public RCSController() { }
    public RCSController(ConfigNode node)
    {
      name = "rcs";
      linkedTo = "rcs";
      node.TryGetValue("name", ref name);
      node.TryGetValue("thrusterTransformName", ref thrusterTransformName);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);


      rcsController = host.GetComponents<ModuleRCSFX>().ToList().Find(x => x.thrusterTransformName == thrusterTransformName);
      if (rcsController == null)
        rcsController = host.GetComponent<ModuleRCSFX>();

      if (rcsController == null)
        Utils.LogError("[RCSController] Could not find ModuleRCSFX on Initialize");

    }

    public override ConfigNode Save()
    {
      ConfigNode c =  base.Save();

      c.AddValue("thrusterTransformName", thrusterTransformName);
      return c;
    }
    public override List<float> Get()
    {
      if (rcsController == null)
      {
        Utils.LogWarning("[RCSController] RCS controller not assigned");
        return new List<float>() { 0f };
      }
      if (overridden)
      {
        List<float> overrideValues = new List<float>();
        for (int i=0; i< rcsController.thrusterTransforms.Count; i++)
        {
          overrideValues.Add(overrideValue);
        }
        return overrideValues;
      }

    
      return (rcsController.thrustForces).ToList().Select(x => x/rcsController.thrusterPower).ToList();
      
    }
  }

}
