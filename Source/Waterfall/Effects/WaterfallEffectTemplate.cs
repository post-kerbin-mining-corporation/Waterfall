using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   Defines an Effect Template, which is a set of WaterfallEffects defined in a separate config
  /// </summary>
  public class WaterfallEffectTemplate
  {
    [Persistent] public string  templateName;
    [Persistent] public string  overrideParentTransform;
    [Persistent] public Vector3 position;
    [Persistent] public Vector3 rotation;
    [Persistent] public Vector3 scale;

    public WaterfallTemplate template;

    public List<WaterfallEffect> allFX = new();

    public WaterfallEffectTemplate() { }

    public WaterfallEffectTemplate(ConfigNode node) : this()
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      position = Vector3.one;
      rotation = Vector3.zero;
      scale    = Vector3.zero;
      ConfigNode.LoadObjectFromConfig(this, node);

      template = WaterfallTemplates.GetTemplate(templateName);
      allFX.Clear();
      for (int i = 0; i < template.allFX.Count; i++)
      {
        var fx = template.allFX[i];
        allFX.Add(new(fx, this));
      }
    }

    public ConfigNode Save()
    {
      var node = ConfigNode.CreateConfigFromObject(this);
      node.name = WaterfallConstants.TemplateNodeName;
      return node;
    }
  }
}
