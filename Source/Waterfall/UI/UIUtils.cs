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
    public static FloatCurve CurveCopyBuffer;

    public static void CopyFloatCurve(FloatCurve curve)
    {
      CurveCopyBuffer = curve;
    }

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


    public static Vector3 Vector3InputField(Rect uiRect, Vector3 vec, string[] textFields, GUIStyle labelStyle, GUIStyle textAreaStyle)
    {

      //string xText = vec.x.ToString();
      //string yText = vec.y.ToString();
      //string zText = vec.z.ToString();
      
      float width = uiRect.width / 6f;
      GUI.BeginGroup(uiRect);
      Rect xRect = new Rect(0, 0, width, uiRect.height);
      Rect yRect = new Rect(2 * width, 0, width, uiRect.height);
      Rect zRect = new Rect(4 * width, 0, width, uiRect.height);
      Rect xFieldRect = new Rect(0.5f*width, 0, width*1.8f, uiRect.height);
      Rect yFieldRect = new Rect(2.5f * width, 0, width*1.8f, uiRect.height);
      Rect zFieldRect = new Rect(4.5f * width, 0, width*1.8f, uiRect.height);

      GUI.Label(xRect, "<b>X</b>", labelStyle);
      GUI.Label(yRect, "<b>Y</b>", labelStyle);
      GUI.Label(zRect, "<b>Z</b>", labelStyle);

      textFields[0] = GUI.TextField(xFieldRect, textFields[0], textAreaStyle);
      textFields[1] = GUI.TextField(yFieldRect, textFields[1], textAreaStyle);
      textFields[2] = GUI.TextField(zFieldRect, textFields[2], textAreaStyle);

      GUI.EndGroup();
      float parsedX = vec.x;
      float parsedY = vec.y;
      float parsedZ = vec.z;

      float.TryParse(textFields[0], out parsedX);
      float.TryParse(textFields[1], out parsedY);
      float.TryParse(textFields[2], out parsedZ);

      return new Vector3(parsedX, parsedY, parsedZ);
    }


    public static Vector2 Vector2InputField(Rect uiRect, Vector2 vec, string[] textFields, GUIStyle labelStyle, GUIStyle textAreaStyle, out bool changed)
    {
      changed = false;
      float width = uiRect.width / 4f;
      GUI.BeginGroup(uiRect);
      Rect xRect = new Rect(0, 0, width, uiRect.height);
      Rect yRect = new Rect(2 * width, 0, width, uiRect.height);
      Rect xFieldRect = new Rect(width - 5f, 0, width , uiRect.height);
      Rect yFieldRect = new Rect(3 * width - 5f, 0, width, uiRect.height);

      GUI.Label(xRect, "<b>X</b>", labelStyle);
      GUI.Label(yRect, "<b>Y</b>", labelStyle);

      textFields[0] = GUI.TextField(xFieldRect, textFields[0], textAreaStyle);
      textFields[1] = GUI.TextField(yFieldRect, textFields[1], textAreaStyle);

      GUI.EndGroup();
      float parsedX = vec.x;
      float parsedY = vec.y;

      float.TryParse(textFields[0], out parsedX);
      float.TryParse(textFields[1], out parsedY);
      Vector2 newVec = new Vector2(parsedX, parsedY);
      if (!Vector2.Equals(vec, newVec))
        changed = true;
      return newVec ;
    }
    public static Color ColorInputField(Rect uiRect, Color vec, string[] textFields, GUIStyle labelStyle, GUIStyle textAreaStyle, out bool changed)
    {
      changed = false;
      float width = uiRect.width / 8f;
      GUI.BeginGroup(uiRect);
      Rect rRect = new Rect(0, 0, width, uiRect.height);
      Rect gRect = new Rect(2 * width, 0, width, uiRect.height);
      Rect bRect = new Rect(4 * width, 0, width, uiRect.height);
      Rect aRect = new Rect(6 * width, 0, width, uiRect.height);
      Rect rFieldRect = new Rect(width - 5f, 0, width, uiRect.height);
      Rect gFieldRect = new Rect(3 * width-5f, 0, width, uiRect.height);
      Rect bFieldRect = new Rect(5 * width - 5f, 0, width, uiRect.height);
      Rect aFieldRect = new Rect(7 * width - 5f, 0, width, uiRect.height);

      GUI.Label(rRect, "R", labelStyle);
      GUI.Label(gRect, "G", labelStyle);
      GUI.Label(bRect, "B", labelStyle);
      GUI.Label(aRect, "A", labelStyle);

      textFields[0] = GUI.TextField(rFieldRect, textFields[0], textAreaStyle);
      textFields[1] = GUI.TextField(gFieldRect, textFields[1], textAreaStyle);
      textFields[2] = GUI.TextField(bFieldRect, textFields[2], textAreaStyle);
      textFields[3] = GUI.TextField(aFieldRect, textFields[3], textAreaStyle);

      GUI.EndGroup();
      float parsedR = vec.r;
      float parsedG = vec.g;
      float parsedB = vec.b;
      float parsedA = vec.a;

      float.TryParse(textFields[0], out parsedR);
      float.TryParse(textFields[1], out parsedG);
      float.TryParse(textFields[2], out parsedB);
      float.TryParse(textFields[3], out parsedA);

      Color newColor = new Color(parsedR, parsedG, parsedB, parsedA);
      if (!Color.Equals(newColor, vec))
        changed = true;
      return newColor;
    }
  }

 
}
