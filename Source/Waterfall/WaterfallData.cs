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
  /// Class to load and hold the shader data for the mod
  /// </summary>
  [KSPAddon(KSPAddon.Startup.MainMenu, false)]
  public  class WaterfallLoader: MonoBehaviour
  { 

    public bool FirstLoad = true;

    public static WaterfallLoader Instance { get; private set; }

    protected void Awake()
    {
      Instance = this;
    }
    protected void Start()
    {
      ShaderLoader.LoadShaders();     
      
    }
    
  }
}
