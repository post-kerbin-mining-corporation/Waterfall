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
    public string overrideParentTransform;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public WaterfallTemplate template;

    public List<WaterfallEffect> allFX;

    public WaterfallEffectTemplate() { }
    public WaterfallEffectTemplate(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      allFX = new List<WaterfallEffect>();
      position = Vector3.one;
      rotation = Vector3.zero;
      scale = Vector3.zero;


      node.TryGetValue("templateName", ref templateName);
      node.TryGetValue("overrideParentTransform", ref overrideParentTransform);
      node.TryParseVector3("position", ref position);
      node.TryParseVector3("rotation", ref rotation);
      node.TryParseVector3("scale", ref scale);

      template = WaterfallTemplates.GetTemplate(templateName);

      foreach (WaterfallEffect fx in template.allFX)
      {
        allFX.Add(new WaterfallEffect(fx, this));
      }
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode();
      node.name = WaterfallConstants.TemplateNodeName;
      node.AddValue("templateName", templateName);
      node.AddValue("overrideParentTransform", overrideParentTransform);
      node.AddValue("scale", scale);
      node.AddValue("rotation", rotation);
      node.AddValue("position", position);


      return node;
    }

  }
}
