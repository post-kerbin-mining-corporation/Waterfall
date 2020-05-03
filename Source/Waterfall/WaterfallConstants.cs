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
    public static string EffectNodeName = "EFFECT";
    public static string ModelNodeName = "MODEL";
    public static string MaterialNodeName = "MATERIAL";
    public static string FloatNodeName = "FLOAT";
    public static string ColorNodeName = "COLOR";
    public static string TextureNodeName = "TEXTURE";


    public static string ColorModifierNodeName = "COLORMODIFIER";
    public static string UVScrollModifierNodeName = "UVSCROLLMODIFIER";
    public static string ScaleModifierNodeName = "SCALEMODIFIER";

    public static Dictionary<string, WaterfallMaterialPropertyType> ShaderPropertyMap = new Dictionary<string, WaterfallMaterialPropertyType>
    {
      ["_MainColor"] = WaterfallMaterialPropertyType.Color,
      ["_EmissiveColor"] = WaterfallMaterialPropertyType.Color,
      ["_TintColor"] = WaterfallMaterialPropertyType.Color
    };




  }
}
