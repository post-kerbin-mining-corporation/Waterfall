using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public static class WaterfallConstants
  {
    public static string ControllerNodeName = "CONTROLLER";
    public static string TemplateLibraryNodeName = "EFFECTTEMPLATE";
    public static string TemplateNodeName = "TEMPLATE";
    public static string EffectNodeName = "EFFECT";
    public static string ModelNodeName = "MODEL";
    public static string MaterialNodeName = "MATERIAL";
    public static string LightNodeName = "LIGHT";
    public static string FloatNodeName = "FLOAT";
    public static string ColorNodeName = "COLOR";
    public static string Vector4NodeName = "VECTOR4";
    public static string TextureNodeName = "TEXTURE";


    public static string ColorModifierNodeName = "COLORMODIFIER";
    public static string FloatModifierNodeName = "FLOATMODIFIER";
    public static string UVScrollModifierNodeName = "UVOFFSETMODIFIER";
    public static string ScaleModifierNodeName = "SCALEMODIFIER";
    public static string PositionModifierNodeName = "POSITIONMODIFIER";
    public static string RotationModifierNodeName = "ROTATIONMODIFIER";

    public static string ColorFromLightNodeName = "COLORLIGHTMODIFIER";
    public static string LightFloatModifierNodeName = "LIGHTFLOATMODIFIER";
    public static string LightColorModifierNodeName = "LIGHTCOLORMODIFIER";

    public static string[] ShaderPropertyHideFloatNames = new string[] { "_Brightness", "_Intensity", "_Strength"};

    public static Dictionary<string, MaterialData> ShaderPropertyMapOlfd= new Dictionary<string, MaterialData>
    {
      ["_MainColor"] = new MaterialData(WaterfallMaterialPropertyType.Color),
      ["_EmissiveColor"] = new MaterialData(WaterfallMaterialPropertyType.Color),
      ["_TintColor"] = new MaterialData(WaterfallMaterialPropertyType.Color),
      ["_StartTint"] = new MaterialData(WaterfallMaterialPropertyType.Color),
      ["_EndTint"] = new MaterialData(WaterfallMaterialPropertyType.Color),
      ["_TintFalloff"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 5f)),
      ["_Falloff"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 10f)),
      ["_Fresnel"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 10f)),
      ["_FresnelInvert"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 5)),
      ["_Intensity"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 10f)),
      ["_Noise"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 15)),
      ["_Brightness"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 10f)),
      ["_SpeedX"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 200f)),
      ["_SpeedY"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 200f)),
      ["_TileX"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 25f)),
      ["_TileY"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 25f)),

      ["_MainTex"] = new MaterialData(WaterfallMaterialPropertyType.Texture),
      
      ["_PlumeDir"] = new MaterialData(WaterfallMaterialPropertyType.Vector4),
      ["_DirAdjust"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1f)),
      ["_FadeIn"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1f)),
      ["_FadeOut"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1f)),
      ["_ExpandOffset"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(-10f, 10f)),
      ["_ExpandBounded"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(-10f, 10f)),
      ["_ExpandLinear"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(-10f, 10f)),
      ["_ExpandSquare"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(-10f, 10f)),
      
      ["_FalloffStart"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1f)),
      ["_Symmetry"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 24f)),
      ["_SymmetryStrength"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1f)),
      ["_Direction"] = new MaterialData(WaterfallMaterialPropertyType.Vector4),
      ["_DirectionScale"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1f)),
      ["_Seed"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1500f)),
      ["_DistortionTex"] = new MaterialData(WaterfallMaterialPropertyType.Texture),
      ["_Strength"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 5f)),
      ["_Highlight"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 1f)),
      ["_Blur"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 5f)),
      ["_Swirl"] = new MaterialData(WaterfallMaterialPropertyType.Float, new Vector2(0f, 5f))

    };




  }
}
