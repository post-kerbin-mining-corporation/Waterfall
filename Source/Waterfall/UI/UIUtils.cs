using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Waterfall.UI
{
  public static class UIUtils
  {
    public static void IconDataField(Rect uiRect, AtlasIcon icon, string value, GUIStyle dataStyle)
    {
      Color color = GUI.color;
      IconDataField(uiRect, icon, value, dataStyle, color);
      GUI.color = color;
    }
    public static void IconDataField(Rect uiRect, AtlasIcon icon, string value, GUIStyle dataStyle, Color color)
    {
      Color oldColor = GUI.color;
      GUI.color = color;
      GUI.BeginGroup(uiRect);
      Rect iconRect = new Rect(0, 0, uiRect.height, uiRect.height);
      Rect dataRect = new Rect(0, 0, uiRect.width, uiRect.height);

      GUI.DrawTextureWithTexCoords(iconRect, icon.iconAtlas, icon.iconRect);
      GUI.Label(dataRect, value, dataStyle);
      GUI.EndGroup();
      GUI.color = oldColor;
    }


    public static Vector3 Vector3InputField(Rect uiRect, Vector3 vec, GUIStyle labelStyle, GUIStyle textAreaStyle)
    {

      string xText = vec.x.ToString();
      string yText = vec.y.ToString();
      string zText = vec.z.ToString();
      
      float width = uiRect.width / 6f;
      GUI.BeginGroup(uiRect);
      Rect xRect = new Rect(0, 0, width, uiRect.height);
      Rect yRect = new Rect(2 * width, 0, width, uiRect.height);
      Rect zRect = new Rect(4 * width, 0, width, uiRect.height);
      Rect xFieldRect = new Rect(width-20f, 0, width*2, uiRect.height);
      Rect yFieldRect = new Rect(3 * width - 20f, 0, width*2, uiRect.height);
      Rect zFieldRect = new Rect(5 * width - 20f, 0, width*2, uiRect.height);

      GUI.Label(xRect, "X", labelStyle);
      GUI.Label(yRect, "Y", labelStyle);
      GUI.Label(zRect, "Z", labelStyle);
      xText = GUI.TextField(xFieldRect, xText, textAreaStyle);
      yText = GUI.TextField(yFieldRect, yText, textAreaStyle);
      zText = GUI.TextField(zFieldRect, zText, textAreaStyle);

      GUI.EndGroup();
      float parsedX = 0f;
      float parsedY = 0f;
      float parsedZ = 0f;

      float.TryParse(xText, out parsedX);
      float.TryParse(yText, out parsedY);
      float.TryParse(zText, out parsedZ);

      return new Vector3(parsedX, parsedY, parsedZ);
    }


    public static Vector2 Vector2InputField(Rect uiRect, Vector2 vec, GUIStyle labelStyle, GUIStyle textAreaStyle)
    {

      string xText = vec.x.ToString();
      string yText = vec.y.ToString();
      float width = uiRect.width / 4f;
      GUI.BeginGroup(uiRect);
      Rect xRect = new Rect(0, 0, uiRect.height, width);
      Rect yRect = new Rect(2 * width, 0, uiRect.height, width);
      Rect xFieldRect = new Rect(1 * width, 0, uiRect.height, width);
      Rect yFieldRect = new Rect(3 * width, 0, uiRect.height, width);

      GUI.Label(xRect, "X", labelStyle);
      GUI.Label(yRect, "Y", labelStyle);
      xText = GUI.TextField(xFieldRect, xText, textAreaStyle);
      yText = GUI.TextField(yFieldRect, yText, textAreaStyle);

      GUI.EndGroup();
      float parsedX = 0f;
      float parsedY = 0f;

      float.TryParse(xText, out parsedX);
      float.TryParse(yText, out parsedY);

      return new Vector2(parsedX, parsedY);
    }
  }
}
