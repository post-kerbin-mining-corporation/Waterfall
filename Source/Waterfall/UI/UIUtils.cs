using System;
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
      var color = GUI.color;
      IconDataField(uiRect, icon, value, dataStyle, color);
      GUI.color = color;
    }

    public static void IconDataField(Rect uiRect, AtlasIcon icon, string value, GUIStyle dataStyle, Color color)
    {
      var oldColor = GUI.color;
      GUI.color = color;
      GUI.BeginGroup(uiRect);
      var iconRect = new Rect(0, 0, uiRect.height, uiRect.height);
      var dataRect = new Rect(0, 0, uiRect.width,  uiRect.height);

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
      var xRect      = new Rect(0,            0, width,        uiRect.height);
      var yRect      = new Rect(2    * width, 0, width,        uiRect.height);
      var zRect      = new Rect(4    * width, 0, width,        uiRect.height);
      var xFieldRect = new Rect(0.5f * width, 0, width * 1.8f, uiRect.height);
      var yFieldRect = new Rect(2.5f * width, 0, width * 1.8f, uiRect.height);
      var zFieldRect = new Rect(4.5f * width, 0, width * 1.8f, uiRect.height);

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

      Single.TryParse(textFields[0], out parsedX);
      Single.TryParse(textFields[1], out parsedY);
      Single.TryParse(textFields[2], out parsedZ);

      return new(parsedX, parsedY, parsedZ);
    }


    public static Vector2 Vector2InputField(Rect uiRect, Vector2 vec, string[] textFields, GUIStyle labelStyle, GUIStyle textAreaStyle, out bool changed)
    {
      changed = false;
      float width = uiRect.width / 4f;
      GUI.BeginGroup(uiRect);
      var xRect      = new Rect(0,              0, width, uiRect.height);
      var yRect      = new Rect(2 * width,      0, width, uiRect.height);
      var xFieldRect = new Rect(width     - 5f, 0, width, uiRect.height);
      var yFieldRect = new Rect(3 * width - 5f, 0, width, uiRect.height);

      GUI.Label(xRect, "<b>X</b>", labelStyle);
      GUI.Label(yRect, "<b>Y</b>", labelStyle);

      textFields[0] = GUI.TextField(xFieldRect, textFields[0], textAreaStyle);
      textFields[1] = GUI.TextField(yFieldRect, textFields[1], textAreaStyle);

      GUI.EndGroup();
      float parsedX = vec.x;
      float parsedY = vec.y;

      Single.TryParse(textFields[0], out parsedX);
      Single.TryParse(textFields[1], out parsedY);
      var newVec = new Vector2(parsedX, parsedY);
      if (!Equals(vec, newVec))
        changed = true;
      return newVec;
    }

    public static Color ColorInputField(Rect uiRect, Color vec, string[] textFields, GUIStyle labelStyle, GUIStyle textAreaStyle, out bool changed)
    {
      changed = false;
      float width = uiRect.width / 8f;
      GUI.BeginGroup(uiRect);
      var rRect      = new Rect(0,              0, width, uiRect.height);
      var gRect      = new Rect(2 * width,      0, width, uiRect.height);
      var bRect      = new Rect(4 * width,      0, width, uiRect.height);
      var aRect      = new Rect(6 * width,      0, width, uiRect.height);
      var rFieldRect = new Rect(width     - 5f, 0, width, uiRect.height);
      var gFieldRect = new Rect(3 * width - 5f, 0, width, uiRect.height);
      var bFieldRect = new Rect(5 * width - 5f, 0, width, uiRect.height);
      var aFieldRect = new Rect(7 * width - 5f, 0, width, uiRect.height);

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

      Single.TryParse(textFields[0], out parsedR);
      Single.TryParse(textFields[1], out parsedG);
      Single.TryParse(textFields[2], out parsedB);
      Single.TryParse(textFields[3], out parsedA);

      var newColor = new Color(parsedR, parsedG, parsedB, parsedA);
      if (!Equals(newColor, vec))
        changed = true;
      return newColor;
    }
  }
}