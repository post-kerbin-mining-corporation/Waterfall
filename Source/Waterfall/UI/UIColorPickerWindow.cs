using System;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{

  public class UIColorPickerWindow : UIPopupWindow
  {
    internal const int   colorFieldSize = 330;
    internal const float sliderWidth = 270;
    internal const float bitScale = 255f;

    protected string windowTitle = "";

    internal Texture   swatch;
    internal Texture   rainbow;
    internal Texture   colorField;
    internal Texture2D barCarat;
    internal Texture2D fieldCarat;

    internal string  slider1Text = "";
    internal string  slider2Text = "";
    internal string  slider3Text = "";
    internal string  slider4Text = "";
    internal Texture slider1Texture;
    internal Texture slider2Texture;
    internal Texture slider3Texture;
    internal Texture slider4Texture;

    internal float hueValue = 0f;
    internal float prevHue = 0f;

    internal ColorHSV currentHSVColor;
    internal ColorHSV prevHSVColor;
    internal Color    currentRGBColor;
    internal Color    prevRGBColor;

    internal Rect colorFieldRect;
    internal Rect hueBarRect;
    internal Rect slider1BarRect;
    internal Rect slider2BarRect;
    internal Rect slider3BarRect;
    internal Rect slider4BarRect;

    internal Vector4 rgbScaled;
    internal Vector4 hsvScaled;

    public UIColorPickerWindow(Color colorToEdit, bool show) : base(show)
    {
      ChangeColor(colorToEdit, show);
      WindowPosition = new Rect(Screen.width / 2 - 100, Screen.height / 2f, 300, 100);
    }
    public void ChangeColor(Color colorToEdit, bool show)
    {
      currentRGBColor = colorToEdit;
      prevRGBColor = currentRGBColor;


      currentHSVColor = new ColorHSV(currentRGBColor);
      prevHSVColor = new ColorHSV(currentRGBColor);

      ScaleColors(currentRGBColor, currentHSVColor);
      GenerateTextures();

      showWindow = true;
    }

    protected void ScaleColors(Color c, ColorHSV h)
    {
      rgbScaled = new Vector4(c.r * bitScale, c.g * bitScale, c.b * bitScale, c.a * bitScale);
      hsvScaled = new Vector4(h.h * bitScale, h.s * bitScale, h.v * bitScale, h.a * bitScale);

      if (sliderMode == 0)
      {
        slider4Text = rgbScaled.w.ToString("F0");
        slider1Text = rgbScaled.x.ToString("F0");
        slider2Text = rgbScaled.y.ToString("F0");
        slider3Text = rgbScaled.z.ToString("F0");
      }
      else
      {
        slider4Text = hsvScaled.w.ToString("F0");
        slider1Text = hsvScaled.x.ToString("F0");
        slider2Text = hsvScaled.y.ToString("F0");
        slider3Text = hsvScaled.z.ToString("F0");
      }
    }
    protected override void InitUI()
    {
      windowTitle = "Color Picker";

      base.InitUI();
    }

    public Color GetColor()
    {
      return currentRGBColor;
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header
      DrawTitle();

      DrawColorField();
      DrawSliders();
      GUI.DragWindow();
      HandleClicks();
    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, UIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = UIResources.GetColor("cancel_color");

      UIUtils.IconButton(buttonRect, "cancel", "button_cancel");

      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected void DrawSwatch(ColorSwatch swatchSlot)
    {
      int slot = UICopy.ColorSwatches.IndexOf(swatchSlot);
      int row = slot / 4;
      int col = row < 1 ? slot : slot - row * 4;
      Rect swatchAreaRect = new Rect(col * 60, row * 30, 60, 30);
      GUI.BeginGroup(swatchAreaRect);

      Rect btnRect = new Rect(0f, 0f, 30f, 30f);
      Rect swatchRect = new Rect(btnRect.x + 2, btnRect.y + 2, btnRect.width - 4, btnRect.height - 4);
      if (GUI.Button(btnRect, ""))
      {
        currentRGBColor = UICopy.ColorSwatches[slot].swatchColor;
        Utils.Log($"Set color to {currentRGBColor}");
      }

      if (UICopy.ColorSwatches[slot].swatchTexture != null)
        GUI.DrawTexture(swatchRect, UICopy.ColorSwatches[slot].swatchTexture);

      if (GUI.Button(new Rect(31f, 7f, 18f, 16f), "x"))
      {
        UICopy.ColorSwatches.RemoveAt(slot);
      }


      GUI.EndGroup();


    }
    protected void DrawColorField()
    {


      GUILayout.BeginVertical();
      GUILayout.Space(10);

      GUILayout.BeginHorizontal();
      // Main Swatch
      Rect tempRect = GUILayoutUtility.GetRect(120, 30, GUILayout.Width(40));
      GUI.DrawTexture(tempRect, swatch);


      GUILayout.Label("Current Colour");
      GUILayout.EndHorizontal();
      GUILayout.Space(10);

      /// Swatches
      /// 
      GUILayout.BeginHorizontal();
      GUILayout.BeginVertical();
      GUILayout.Label("<b>Swatches</b>", GUILayout.Width(70));
      if (GUILayout.Button("Save"))
      {
        UICopy.ColorSwatches.Add(new ColorSwatch(currentRGBColor));
      }
      GUILayout.Space(5);
      GUILayout.EndVertical();
      int usedSwatches = UICopy.ColorSwatches.Count;

      int rows = usedSwatches >= 4 ? 2 : 1;
      Rect swatchAreaRect = GUILayoutUtility.GetRect(230, rows * 30);
      GUI.BeginGroup(swatchAreaRect);
      /// don't try this at home
      try
      {
        foreach (ColorSwatch swatchSlot in UICopy.ColorSwatches)
        {

          DrawSwatch(swatchSlot);
        }

      }
      catch (InvalidOperationException)
      { }
      GUI.EndGroup();
      GUILayout.EndHorizontal();

      GUILayout.Space(15);
      /// Rainbow hue selector
      tempRect = GUILayoutUtility.GetRect(150, 20, GUILayout.Width(colorFieldSize));
      GUI.DrawTexture(tempRect, rainbow);
      hueBarRect = GUIUtility.GUIToScreenRect(tempRect);

      // Position the bar carat
      Rect caratRect = new Rect(tempRect.x + hueValue * tempRect.width, tempRect.y - 2f, 4f, tempRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);

      GUILayout.Space(5);

      /// Color Field
      tempRect = GUILayoutUtility.GetRect(colorFieldSize, colorFieldSize, GUILayout.Width(colorFieldSize));
      GUI.DrawTexture(tempRect, colorField);
      colorFieldRect = GUIUtility.GUIToScreenRect(tempRect);

      // Position the field carat
      caratRect = new Rect(tempRect.x + currentHSVColor.s * tempRect.width, tempRect.y + (1f - currentHSVColor.v) * tempRect.height, 10f, 10f);
      GUI.DrawTexture(caratRect, fieldCarat);

      GUILayout.EndVertical();
      GUILayout.Space(4);

    }

    int sliderMode = 0;
    protected void DrawSliders()
    {
      // float sliderValue = 0f;
      int sliderModeNew = GUILayout.SelectionGrid(sliderMode, new string[] { "RGBA", "HSVA" }, 2);
      if (sliderModeNew != sliderMode)
      {
        sliderMode = sliderModeNew;
        GenerateTextures();
      }
      if (sliderMode == 0)
        DrawSlidersRGB();
      else
        DrawSlidersHSV();


    }
    protected void DrawSlidersRGB()
    {
      bool inputChanged = false;
      string fieldText;
      // Bar 1
      GUILayout.BeginHorizontal();
      GUILayout.Label("R", GUILayout.Width(15));
      GUILayout.BeginVertical();
      Rect tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider1Texture);
      Rect caratRect = new Rect(tRect.x + currentRGBColor.r * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider1BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();


      fieldText = GUILayout.TextArea(slider1Text);

      if (fieldText != slider1Text)
      {
        if (float.TryParse(fieldText, out rgbScaled.x))
        {
          rgbScaled.x = Mathf.Clamp(rgbScaled.x, 0f, 255f);
          slider1Text = $"{rgbScaled.x.ToString("F0")}";
          inputChanged = true;
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(2);

      // Bar 2
      GUILayout.BeginHorizontal();
      GUILayout.Label("G", GUILayout.Width(15));
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider2Texture);
      caratRect = new Rect(tRect.x + currentRGBColor.g * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider2BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();

      fieldText = GUILayout.TextArea(slider2Text);

      if (fieldText != slider2Text)
      {
        if (float.TryParse(fieldText, out rgbScaled.y))
        {
          rgbScaled.y = Mathf.Clamp(rgbScaled.y, 0f, 255f);
          slider2Text = $"{rgbScaled.y.ToString("F0")}";
          inputChanged = true;
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(2);

      // Bar 3
      GUILayout.BeginHorizontal();
      GUILayout.Label("B", GUILayout.Width(15));
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider3Texture);
      caratRect = new Rect(tRect.x + currentRGBColor.b * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider3BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();


      fieldText = GUILayout.TextArea(slider3Text);
      if (fieldText != slider3Text)
      {
        if (float.TryParse(fieldText, out rgbScaled.z))
        {
          rgbScaled.z = Mathf.Clamp(rgbScaled.z, 0f, 255f);
          slider3Text = $"{rgbScaled.z.ToString("F0")}";
          inputChanged = true;
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(2);

      // Bar 4
      GUILayout.BeginHorizontal();
      GUILayout.Label("A", GUILayout.Width(15));
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider4Texture);
      caratRect = new Rect(tRect.x + currentRGBColor.a * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider4BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();

      fieldText = GUILayout.TextArea(slider4Text);
      if (fieldText != slider4Text)
      {
        if (float.TryParse(fieldText, out rgbScaled.w))
        {
          rgbScaled.w = Mathf.Clamp(rgbScaled.w, 0f, 255f);
          slider4Text = $"{rgbScaled.w.ToString("F0")}";
          inputChanged = true;
        }

      }

      GUILayout.EndHorizontal();
      if (inputChanged)
      {
        currentRGBColor = new Color(rgbScaled.x / 255f, rgbScaled.y / 255f, rgbScaled.z / 255f, rgbScaled.w / 255f);
        currentHSVColor = new ColorHSV(currentRGBColor);
      }
      //

    }
    protected void DrawSlidersHSV()
    {
      bool inputChanged = false;
      string fieldText;
      // Bar 1
      GUILayout.BeginHorizontal();
      GUILayout.Label("H", GUILayout.Width(15));
      GUILayout.BeginVertical();
      Rect tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider1Texture);
      Rect caratRect = new Rect(tRect.x + currentHSVColor.h * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider1BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();


      fieldText = GUILayout.TextArea(slider1Text);

      if (fieldText != slider1Text)
      {
        if (float.TryParse(fieldText, out hsvScaled.x))
        {
          hsvScaled.x = Mathf.Clamp(hsvScaled.x, 0f, 255f);
          slider1Text = $"{hsvScaled.x.ToString("F0")}";
          inputChanged = true;
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(2);

      // Bar 2
      GUILayout.BeginHorizontal();
      GUILayout.Label("S", GUILayout.Width(15));
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider2Texture);
      caratRect = new Rect(tRect.x + currentHSVColor.s * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider2BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();

      fieldText = GUILayout.TextArea(slider2Text);

      if (fieldText != slider2Text)
      {
        if (float.TryParse(fieldText, out hsvScaled.y))
        {
          hsvScaled.y = Mathf.Clamp(hsvScaled.y, 0f, 255f);
          slider2Text = $"{hsvScaled.y.ToString("F0")}";
          inputChanged = true;
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(2);

      // Bar 3
      GUILayout.BeginHorizontal();
      GUILayout.Label("V", GUILayout.Width(15));
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider3Texture);
      caratRect = new Rect(tRect.x + currentHSVColor.v * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider3BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();


      fieldText = GUILayout.TextArea(slider3Text);
      if (fieldText != slider3Text)
      {
        if (float.TryParse(fieldText, out hsvScaled.z))
        {
          hsvScaled.z = Mathf.Clamp(hsvScaled.z, 0f, 255f);
          slider3Text = $"{hsvScaled.z.ToString("F0")}";
          inputChanged = true;
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(2);

      // Bar 4
      GUILayout.BeginHorizontal();
      GUILayout.Label("A", GUILayout.Width(15));
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, slider4Texture);
      caratRect = new Rect(tRect.x + currentHSVColor.a * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
      GUI.DrawTexture(caratRect, barCarat);
      slider4BarRect = GUIUtility.GUIToScreenRect(tRect);
      GUILayout.EndVertical();

      fieldText = GUILayout.TextArea(slider4Text);
      if (fieldText != slider4Text)
      {
        if (float.TryParse(fieldText, out hsvScaled.w))
        {
          hsvScaled.w = Mathf.Clamp(hsvScaled.w, 0f, 255f);
          slider4Text = $"{hsvScaled.w.ToString("F0")}";
          inputChanged = true;
        }

      }

      GUILayout.EndHorizontal();
      if (inputChanged)
      {
        currentRGBColor = new Color(rgbScaled.x / 255f, rgbScaled.y / 255f, rgbScaled.z / 255f, rgbScaled.w / 255f);
        currentHSVColor = new ColorHSV(currentRGBColor);
      }
      //

    }
    protected bool TryGetRectClickPos(Vector2 mPos, Rect screenRect, out Vector2 result)
    {

      result = Vector2.zero;
      if (screenRect.Contains(mPos))
      {

        result = new Vector2(Mathf.Clamp(mPos.x - screenRect.xMin, 0, screenRect.width),
         (int)screenRect.width - (int)Mathf.Clamp(mPos.y - screenRect.yMin, 0, screenRect.width));
        return true;
      }
      return false;
    }
    protected bool TryGetRectTextureColor(Vector2 mPos, Rect screenRect, Texture2D texture, out Color result)
    {
      result = new Color();
      Vector2 clickPos = Vector2.zero;
      if (TryGetRectClickPos(mPos, screenRect, out clickPos))
      {

        result = texture.GetPixel((int)clickPos.x, (int)clickPos.y);
        return true;
      }
      return false;
    }

    protected void HandleColorFieldClick(Vector2 mPos)
    {
      Color colorFieldSample = new Color();
      if (TryGetRectTextureColor(mPos, colorFieldRect, (Texture2D)colorField, out colorFieldSample))
      {
        currentRGBColor = new Color(colorFieldSample.r, colorFieldSample.g, colorFieldSample.b, currentRGBColor.a);
        currentHSVColor = new ColorHSV(currentRGBColor);
        ScaleColors(currentRGBColor, currentHSVColor);
      }
    }
    protected void HandleHueBarClick(Vector2 mPos)
    {
      Vector2 clickPos = Vector2.zero;
      if (TryGetRectClickPos(mPos, hueBarRect, out clickPos))
      {

        hueValue = ((float)clickPos.x / (float)hueBarRect.width);
        currentHSVColor = new ColorHSV(hueValue, currentHSVColor.s, currentHSVColor.v);
        ScaleColors(currentRGBColor, currentHSVColor);
      }
    }
    protected void HandleColorBarClick(Vector2 mPos, Rect barRect, int barID)
    {
      Vector2 clickPos = Vector2.zero;
      if (TryGetRectClickPos(mPos, barRect, out clickPos))
      {
        if (sliderMode == 0)
        {
          if (barID == 0)
            currentRGBColor = new Color(clickPos.x / (float)barRect.width, currentRGBColor.g, currentRGBColor.b, currentRGBColor.a);
          if (barID == 1)
            currentRGBColor = new Color(currentRGBColor.r, clickPos.x / (float)barRect.width, currentRGBColor.b, currentRGBColor.a);
          if (barID == 2)
            currentRGBColor = new Color(currentRGBColor.r, currentRGBColor.g, clickPos.x / (float)barRect.width, currentRGBColor.a);
          if (barID == 3)
            currentRGBColor = new Color(currentRGBColor.r, currentRGBColor.g, currentRGBColor.b, clickPos.x / (float)barRect.width);

          currentHSVColor = new ColorHSV(currentRGBColor);
        }
        else
        {
          if (barID == 0)
            currentHSVColor = new ColorHSV(clickPos.x / (float)barRect.width, currentHSVColor.s, currentHSVColor.v, currentRGBColor.a);
          if (barID == 1)
            currentHSVColor = new ColorHSV(currentHSVColor.h, clickPos.x / (float)barRect.width, currentHSVColor.v, currentRGBColor.a);
          if (barID == 2)
            currentHSVColor = new ColorHSV(currentHSVColor.h, currentHSVColor.s, clickPos.x / (float)barRect.width, currentRGBColor.a);
          if (barID == 3)
            currentHSVColor = new ColorHSV(currentHSVColor.h, currentHSVColor.s, currentHSVColor.v, clickPos.x / (float)barRect.width);



        }
        ScaleColors(currentRGBColor, currentHSVColor);
      }
    }

    protected void HandleClicks()
    {

      var eventType = Event.current.type;
      if (eventType == EventType.Repaint)
        if (Input.GetMouseButton(0))
        {

          Vector2 mPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

          HandleHueBarClick(mPos);
          HandleColorFieldClick(mPos);
          HandleColorBarClick(mPos, slider1BarRect, 0);
          HandleColorBarClick(mPos, slider2BarRect, 1);
          HandleColorBarClick(mPos, slider3BarRect, 2);
          HandleColorBarClick(mPos, slider4BarRect, 3);
        }
    }

    public void Update()
    {
      if (!currentHSVColor.Equals(prevHSVColor))
      {
        Debug.Log($"hsv change to {currentHSVColor} ,{currentHSVColor.ToRGB()}");
        colorField = TextureUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor);
        prevHSVColor = currentHSVColor;
        currentRGBColor = currentHSVColor.ToRGB();

      }

      if (!currentRGBColor.Equals(prevRGBColor))
      {
        GenerateTextures();
        currentHSVColor = new ColorHSV(currentRGBColor);

        //  currentHSVColor = new HSBColor(currentRGBColor);
        //  hueValue = currentHSVColor.h;
        //  colorField = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());
        prevRGBColor = currentRGBColor;
        hueValue = currentHSVColor.h;
      }
    }

    public void GenerateTextures()
    {
      rainbow    = TextureUtils.GenerateRainbowGradient(120, 20);
      colorField = TextureUtils.GenerateColorField(colorFieldSize, colorFieldSize,
        currentRGBColor);

      if (sliderMode == 0)
      {
        slider1Texture = TextureUtils.GenerateGradientTexture(100, 20,
          new Color(0.0f, currentRGBColor.g, currentRGBColor.b, 1.0f),
          new Color(1.0f, currentRGBColor.g, currentRGBColor.b, 1.0f));
        slider2Texture = TextureUtils.GenerateGradientTexture(100, 20,
          new Color(currentRGBColor.r, 0f, currentRGBColor.b, 1.0f),
          new Color(currentRGBColor.r, 1f, currentRGBColor.b, 1.0f));
        slider3Texture = TextureUtils.GenerateGradientTexture(100, 20,
          new Color(currentRGBColor.r, currentRGBColor.g, 0f, 1.0f),
          new Color(currentRGBColor.r, currentRGBColor.g, 1f, 1.0f));
        slider4Texture = TextureUtils.GenerateGradientTexture(100, 20,
          Color.black,
          Color.white);
      }
      else
      {
        slider1Texture = TextureUtils.GenerateRainbowGradient(120, 20);
        slider2Texture = TextureUtils.GenerateGradientTexture(100, 20,
          new ColorHSV(currentHSVColor.h, 0f, currentHSVColor.v).ToRGB(),
          new ColorHSV(currentHSVColor.h, 1f, currentHSVColor.v).ToRGB());
        slider3Texture = TextureUtils.GenerateGradientTexture(100, 20,
          new ColorHSV(currentHSVColor.h, currentHSVColor.s, 0f).ToRGB(),
          new ColorHSV(currentHSVColor.h, currentHSVColor.s, 1f).ToRGB());
        slider4Texture = TextureUtils.GenerateGradientTexture(100, 20,
          Color.black,
          Color.white);
      }

      swatch = TextureUtils.GenerateColorTexture(40, 20,
        currentRGBColor);


      barCarat = TextureUtils.GenerateSliderCarat(5, 20,
        new Color(1f, 1f, 1f, 0.75f),
        Color.black);

      fieldCarat = TextureUtils.GenerateRoundCarat(20, 20, Color.white, Color.black);
    }
  }
}

