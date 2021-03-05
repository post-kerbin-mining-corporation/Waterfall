using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace Waterfall
{
  /// <summary>
  /// Class to load shaders and config level data for the mod.
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Instantly, false)]
  public  class WaterfallData: MonoBehaviour
  { 

    public bool FirstLoad = true;
    public static WaterfallData Instance { get; private set; }

    protected void Awake()
    {
      Instance = this;
    }

    protected void Start()
    {
      WaterfallParticleLoader.LoadParticles();
      ShaderLoader.LoadShaders();
      ShaderLoader.LoadShaderProperties();
      WaterfallTemplates.LoadTemplates();
    }
    
  }
}
