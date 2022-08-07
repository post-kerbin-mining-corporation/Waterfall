using System.Collections.Generic;

namespace Waterfall
{
  public static class WaterfallConstants
  {

    public static string LegacyControllerNodeName = "CONTROLLER";
    public static string TemplateLibraryNodeName  = "EFFECTTEMPLATE";
    public static string TemplateNodeName         = "TEMPLATE";
    public static string EffectNodeName           = "EFFECT";
    public static string ModelNodeName            = "MODEL";
    public static string MaterialNodeName         = "MATERIAL";
    public static string LightNodeName            = "LIGHT";
    public static string FloatNodeName            = "FLOAT";
    public static string ColorNodeName            = "COLOR";
    public static string Vector4NodeName          = "VECTOR4";
    public static string TextureNodeName          = "TEXTURE";

    public static string ColorModifierNodeName    = "COLORMODIFIER";
    public static string FloatModifierNodeName    = "FLOATMODIFIER";
    public static string UVScrollModifierNodeName = "UVOFFSETMODIFIER";
    public static string ScaleModifierNodeName    = "SCALEMODIFIER";
    public static string PositionModifierNodeName = "POSITIONMODIFIER";
    public static string RotationModifierNodeName = "ROTATIONMODIFIER";

    public static string ColorFromLightNodeName     = "COLORLIGHTMODIFIER";
    public static string LightFloatModifierNodeName = "LIGHTFLOATMODIFIER";
    public static string LightColorModifierNodeName = "LIGHTCOLORMODIFIER";

    public static string ParticleSystemModifierNodeName = "PARTICLESYSTEMMODIFIER";
    public static string[] ShaderPropertyHideFloatNames = { "_Brightness", "_Intensity", "_Strength" };

    public static Dictionary<string, MaterialData> ShaderPropertyMapOlfd = new()
    {
      ["_MainColor"]     = new(WaterfallMaterialPropertyType.Color),
      ["_EmissiveColor"] = new(WaterfallMaterialPropertyType.Color),
      ["_TintColor"]     = new(WaterfallMaterialPropertyType.Color),
      ["_StartTint"]     = new(WaterfallMaterialPropertyType.Color),
      ["_EndTint"]       = new(WaterfallMaterialPropertyType.Color),
      ["_TintFalloff"]   = new(WaterfallMaterialPropertyType.Float, new(0f, 5f)),
      ["_Falloff"]       = new(WaterfallMaterialPropertyType.Float, new(0f, 10f)),
      ["_Fresnel"]       = new(WaterfallMaterialPropertyType.Float, new(0f, 10f)),
      ["_FresnelInvert"] = new(WaterfallMaterialPropertyType.Float, new(0f, 5)),
      ["_Intensity"]     = new(WaterfallMaterialPropertyType.Float, new(0f, 10f)),
      ["_Noise"]         = new(WaterfallMaterialPropertyType.Float, new(0f, 15)),
      ["_Brightness"]    = new(WaterfallMaterialPropertyType.Float, new(0f, 10f)),
      ["_SpeedX"]        = new(WaterfallMaterialPropertyType.Float, new(0f, 200f)),
      ["_SpeedY"]        = new(WaterfallMaterialPropertyType.Float, new(0f, 200f)),
      ["_TileX"]         = new(WaterfallMaterialPropertyType.Float, new(0f, 25f)),
      ["_TileY"]         = new(WaterfallMaterialPropertyType.Float, new(0f, 25f)),

      ["_MainTex"] = new(WaterfallMaterialPropertyType.Texture),

      ["_PlumeDir"]      = new(WaterfallMaterialPropertyType.Vector4),
      ["_DirAdjust"]     = new(WaterfallMaterialPropertyType.Float, new(0f, 1f)),
      ["_FadeIn"]        = new(WaterfallMaterialPropertyType.Float, new(0f, 1f)),
      ["_FadeOut"]       = new(WaterfallMaterialPropertyType.Float, new(0f, 1f)),
      ["_ExpandOffset"]  = new(WaterfallMaterialPropertyType.Float, new(-10f, 10f)),
      ["_ExpandBounded"] = new(WaterfallMaterialPropertyType.Float, new(-10f, 10f)),
      ["_ExpandLinear"]  = new(WaterfallMaterialPropertyType.Float, new(-10f, 10f)),
      ["_ExpandSquare"]  = new(WaterfallMaterialPropertyType.Float, new(-10f, 10f)),

      ["_FalloffStart"]     = new(WaterfallMaterialPropertyType.Float, new(0f, 1f)),
      ["_Symmetry"]         = new(WaterfallMaterialPropertyType.Float, new(0f, 24f)),
      ["_SymmetryStrength"] = new(WaterfallMaterialPropertyType.Float, new(0f, 1f)),
      ["_Direction"]        = new(WaterfallMaterialPropertyType.Vector4),
      ["_DirectionScale"]   = new(WaterfallMaterialPropertyType.Float, new(0f, 1f)),
      ["_Seed"]             = new(WaterfallMaterialPropertyType.Float, new(0f, 1500f)),
      ["_DistortionTex"]    = new(WaterfallMaterialPropertyType.Texture),
      ["_Strength"]         = new(WaterfallMaterialPropertyType.Float, new(0f, 5f)),
      ["_Highlight"]        = new(WaterfallMaterialPropertyType.Float, new(0f, 1f)),
      ["_Blur"]             = new(WaterfallMaterialPropertyType.Float, new(0f, 5f)),
      ["_Swirl"]            = new(WaterfallMaterialPropertyType.Float, new(0f, 5f))
    };

    public static Dictionary<string, ParticleParameterData> ParticleParameterMap = new Dictionary<string, ParticleParameterData>
    {
      ["StartSize"] = new ParticleParameterData("StartSize", ParticleParameterType.Range),
      ["StartLifetime"] = new ParticleParameterData("StartLifetime", ParticleParameterType.Range),
      ["StartSpeed"] = new ParticleParameterData("StartSpeed", ParticleParameterType.Range),
      ["EmissionRate"] = new ParticleParameterData("EmissionRate", ParticleParameterType.Range),
      ["EmissionVolumeLength"] = new ParticleParameterData("EmissionVolumeLength", ParticleParameterType.Value),
      ["EmissionVolumeRadius"] = new ParticleParameterData("EmissionVolumeRadius", ParticleParameterType.Value),
      ["MaxParticles"] = new ParticleParameterData("MaxParticles", ParticleParameterType.Value),
      
    };


  }
}