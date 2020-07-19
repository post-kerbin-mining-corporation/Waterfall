using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waterfall
{
  /// <summary>
  /// Class for loading and retrieving effect templates
  /// </summary>
  public static class WaterfallTemplates
  {

    public static Dictionary<string, WaterfallEffectTemplate> Library = new Dictionary<string, WaterfallEffectTemplate>();

    /// <summary>
    /// Get an effect template by name
    /// </summary>
    /// <param name="name">The config name of the template</param>
    /// <returns>The template, if it exists</returns>
    public static WaterfallEffectTemplate GetTemplate(string name)
    {
      if (WaterfallTemplates.Library.ContainsKey(name))
      {
        return WaterfallTemplates.Library[name];
      } else
      {
        Utils.LogError($"[Template Libary]: Can't find effect template {name}");
        return null;
      }
    }

    /// <summary>
    /// Load all templates from config
    /// </summary>
    public static void LoadTemplates()
    {
      Utils.Log($"[Template Libary]: Loading effect templates", LogType.Settings);
      ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes(WaterfallConstants.TemplateLibraryNodeName);

      foreach (ConfigNode node in nodes)
      {
        if (!Library.ContainsKey(node.GetValue("templateName")))
        {
          Library.Add(node.GetValue("templateName"), new WaterfallEffectTemplate(node));
          Utils.Log($"[Template Libary]: Added template {node.GetValue("templateName")}", LogType.Settings);
        }
      }
    }

  }
}
