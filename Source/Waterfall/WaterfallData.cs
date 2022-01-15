using UnityEngine;
using Waterfall.UI;

namespace Waterfall
{
  /// <summary>
  ///   Class to load shaders and config level data for the mod.
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Instantly, false)]
  public class WaterfallData : MonoBehaviour
  {
    public        bool          FirstLoad = true;
    public static WaterfallData Instance { get; private set; }

    protected void Awake()
    {
      Instance = this;
    }

    public static void ModuleManagerPostLoad()
    {
      WaterfallParticleLoader.LoadParticles();
      ShaderLoader.LoadShaders();
      ShaderLoader.LoadShaderProperties();
      WaterfallTemplates.LoadTemplates();
      UIResources.InitalizeUIResources();
    }
  }
}