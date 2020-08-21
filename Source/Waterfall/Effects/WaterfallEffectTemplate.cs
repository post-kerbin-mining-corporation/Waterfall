using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// Defines an Effect Template, which is a set of WaterfallEffects defined in a separate config
  /// </summary>
  public class WaterfallEffectTemplate
  {
    public string templateName;

    public List<WaterfallEffect> allFX;

    public WaterfallEffectTemplate() { }
    public WaterfallEffectTemplate(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue("templateName", ref templateName);
      // Load up all the effects
      ConfigNode[] effectNodes = node.GetNodes(WaterfallConstants.EffectNodeName);
      
      Utils.Log($"[WaterfallEffectTemplate]: Loading effects on {templateName}");
      allFX = new List<WaterfallEffect>();
      foreach (ConfigNode fxDataNode in effectNodes)
      {
        WaterfallEffect newFX = ScriptableObject.CreateInstance<WaterfallEffect>();
        newFX.SetupEffect(fxDataNode);
        allFX.Add(newFX);
      }
    }
    
  }
}
