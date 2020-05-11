using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waterfall;

namespace Waterfall
{
  public static class WaterfallConstants
  {
    public static string ControllerNodeName = "CONTROLLER";
    public static string TemplateNodeName = "EFFECTTEMPLATE";
    public static string EffectNodeName = "EFFECT";
    public static string ModelNodeName = "MODEL";
    public static string MaterialNodeName = "MATERIAL";
    public static string FloatNodeName = "FLOAT";
    public static string ColorNodeName = "COLOR";
    public static string TextureNodeName = "TEXTURE";


    public static string ColorModifierNodeName = "COLORMODIFIER";
    public static string FloatModifierNodeName = "FLOATMODIFIER";
    public static string UVScrollModifierNodeName = "UVOFFSETMODIFIER";
    public static string ScaleModifierNodeName = "SCALEMODIFIER";
    public static string PositionModifierNodeName = "POSITIONMODIFIER";
    public static string RotationModifierNodeName = "ROTATIONMODIFIER";

    public static Dictionary<string, WaterfallMaterialPropertyType> ShaderPropertyMap = new Dictionary<string, WaterfallMaterialPropertyType>
    {
      ["_MainColor"] = WaterfallMaterialPropertyType.Color,
      ["_EmissiveColor"] = WaterfallMaterialPropertyType.Color,
      ["_TintColor"] = WaterfallMaterialPropertyType.Color,
      ["_StartTint"] = WaterfallMaterialPropertyType.Color,
      ["_EndTint"] = WaterfallMaterialPropertyType.Color,
      ["_TintFalloff"] = WaterfallMaterialPropertyType.Float,
      ["_Falloff"] = WaterfallMaterialPropertyType.Float,
      ["_Fresnel"] = WaterfallMaterialPropertyType.Float,
      ["_Noise"] = WaterfallMaterialPropertyType.Float,
      ["_Brightness"] = WaterfallMaterialPropertyType.Float,
      ["_SpeedX"] = WaterfallMaterialPropertyType.Float,
      ["_SpeedY"] = WaterfallMaterialPropertyType.Float,
      ["_TileX"] = WaterfallMaterialPropertyType.Float,
      ["_TileY"] = WaterfallMaterialPropertyType.Float,
      ["_MainTex"] = WaterfallMaterialPropertyType.Texture

    };




  }
}
