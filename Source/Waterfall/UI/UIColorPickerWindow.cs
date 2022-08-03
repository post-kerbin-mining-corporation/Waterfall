using UnityEngine;

namespace Waterfall.UI
{
  public class UIColorPickerWindow : UIPopupWindow
  {
    internal const int colorFieldSize = 330;
    internal const float sliderWidth = 270;
    internal const float bitScale = 255f;

    protected string windowTitle = "";
    internal bool dragSliders = false;

    internal Texture swatch;
    internal Texture rainbow;
    internal Texture colorField;
    internal Texture2D barCarat;
    internal Texture2D fieldCarat;
    internal Rect colorFieldRect;
    internal Rect hueBarRect;

    internal static readonly string[] sliderModes = { "RGBA", "HSVA" };
    internal int sliderMode = 0; // Index into array above.
    internal bool RGBMode => sliderMode == 0;

    internal readonly string[] sliderTexts = new string[4];
    internal readonly Texture[] sliderTextures = new Texture[4];
    internal readonly Rect[] sliderBarRects = new Rect[4];

    private Color color_;
    internal Color color
    {
      get { return color_; }
      set
      {
        colorHSVCache_ = null;
        color_ = value;
        GenerateTextures();
        SetSliderTexts();
      }
    }

    private ColorHSV colorHSVCache_;
    internal ColorHSV colorHSV => colorHSVCache_ ??= new(color);
    internal float hue
    {
      get { return colorHSV.h; }
      set
      {
        ColorHSV hsv = new(color);
        hsv.h = value;
        color = hsv.ToRGB();
      }
    }

    public Color GetColor() => color;

    internal Vector4 GetRawColorValues()
    {
      if (RGBMode)
      {
        return new(color.r, color.g, color.b, color.a);
      }
      return new(colorHSV.h, colorHSV.s, colorHSV.v, colorHSV.a);
    }

    internal void SetColorByRawValues(Vector4 rawValues)
    {
      if (RGBMode)
      {
        color = new(rawValues.x, rawValues.y, rawValues.z, rawValues.w);
      }
      else
      {
        color = new ColorHSV(rawValues.x, rawValues.y, rawValues.z, rawValues.w).ToRGB();
      }
    }

    public UIColorPickerWindow(Color colorToEdit, bool show) : base(show)
    {
      ChangeColor(colorToEdit);
      WindowPosition = new Rect(Screen.width / 2 - 100, Screen.height / 2f, 300, 100);
    }

    public void ChangeColor(Color colorToEdit, bool forceShow = false)
    {
      color = colorToEdit;

      if (forceShow)
        showWindow = true;
    }

    protected override void InitUI()
    {
      windowTitle = "Color Picker";

      base.InitUI();
    }

    protected override void DrawWindow(int windowId)
    {
      DrawTitle();

      DrawColorField();
      DrawSliders();

      HandleClicks();
      HandleDrag();
    }

    public void Update() { }

    protected void HandleDrag()
    {
      if (dragSliders && Input.GetMouseButtonUp(0))
      {
        dragSliders = false;
      }
      if (!dragSliders)
      {
        GUI.DragWindow();
      }
    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, UIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = UIResources.GetColor("cancel_color");

      if (UIUtils.IconButton(buttonRect, "cancel", "button_cancel"))
      {
        ToggleWindow();
      }

      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected bool DrawSwatch(int row, int col, ColorSwatch swatchSlot)
    {
      Rect swatchAreaRect = new(col * 60, row * 30, 60, 30);
      using (new GUI.GroupScope(swatchAreaRect))
      {
        Rect btnRect = new(0f, 0f, 30f, 30f);
        Rect swatchRect = new(btnRect.x + 2, btnRect.y + 2, btnRect.width - 4, btnRect.height - 4);
        if (GUI.Button(btnRect, ""))
        {
          color = swatchSlot.swatchColor;
        }
        if (swatchSlot.swatchTexture != null)
        {
          GUI.DrawTexture(swatchRect, swatchSlot.swatchTexture);
        }
        if (GUI.Button(new Rect(31f, 7f, 18f, 16f), "x"))
        {
          return true;
        }
      }
      return false;
    }

    protected void DrawColorField()
    {
      Rect tempRect;
      using (new GUILayout.VerticalScope())
      {
        GUILayout.Space(10);

        /// Main Swatch
        using (new GUILayout.HorizontalScope())
        {
          tempRect = GUILayoutUtility.GetRect(120, 30, GUILayout.Width(40));
          GUI.DrawTexture(tempRect, swatch);
          GUILayout.Label("Current Colour");
        }
        GUILayout.Space(10);

        /// Swatches
        using (new GUILayout.HorizontalScope())
        {
          using (new GUILayout.VerticalScope())
          {
            GUILayout.Label("<b>Swatches</b>", GUILayout.Width(70));
            if (GUILayout.Button("Save"))
            {
              UICopy.ColorSwatches.Add(new ColorSwatch(color));
            }
            GUILayout.Space(5);
          }

          int usedSwatches = UICopy.ColorSwatches.Count;
          const int colCount = 4;
          int rows = (usedSwatches + colCount - 1) / colCount; // Integer division, rounding up.
          tempRect = GUILayoutUtility.GetRect(230, rows * 30);
          using (new GUI.GroupScope(tempRect))
          {
            for (int i = 0; i < UICopy.ColorSwatches.Count; ++i)
            {
              if (DrawSwatch(row: i / 4, col: i % 4, UICopy.ColorSwatches[i]))
              {
                UICopy.ColorSwatches.RemoveAt(i);
              }
            }
          }
        }
        GUILayout.Space(15);

        // Rainbow hue selector
        tempRect = GUILayoutUtility.GetRect(150, 20, GUILayout.Width(colorFieldSize));
        GUI.DrawTexture(tempRect, rainbow);
        hueBarRect = GUIUtility.GUIToScreenRect(tempRect);
        // Position the bar carat
        tempRect = new(tempRect.x + hue * tempRect.width, tempRect.y - 2f, 4f, tempRect.height + 4f);
        GUI.DrawTexture(tempRect, barCarat);
        GUILayout.Space(5);

        /// Color Field
        tempRect = GUILayoutUtility.GetRect(colorFieldSize, colorFieldSize, GUILayout.Width(colorFieldSize));
        GUI.DrawTexture(tempRect, colorField);
        colorFieldRect = GUIUtility.GUIToScreenRect(tempRect);
        // Position the field carat
        tempRect = new(tempRect.x + colorHSV.s * tempRect.width, tempRect.y + (1f - colorHSV.v) * tempRect.height, 10f, 10f);
        GUI.DrawTexture(tempRect, fieldCarat);
      }
      GUILayout.Space(4);
    }

    protected void DrawSliders()
    {
      int sliderModeNew = GUILayout.SelectionGrid(sliderMode, sliderModes, 2);
      if (sliderModeNew != sliderMode)
      {
        sliderMode = sliderModeNew;
        GenerateTextures();
        SetSliderTexts();
      }

      string sliderLabels = sliderModes[sliderMode];
      Vector4 rawValues = GetRawColorValues();
      Vector4 newValues = rawValues;
      for (int i = 0; i < 4; ++i)
      {
        using (new GUILayout.HorizontalScope())
        {
          GUILayout.Label(sliderLabels.Substring(i, 1), GUILayout.Width(15));
          using (new GUILayout.VerticalScope())
          {
            var tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
            GUI.DrawTexture(tRect, sliderTextures[i]);
            var caratRect = new Rect(tRect.x + rawValues[i] * tRect.width, tRect.y - 2f, 4f, tRect.height + 4f);
            GUI.DrawTexture(caratRect, barCarat);
            sliderBarRects[i] = GUIUtility.GUIToScreenRect(tRect);
          }
          string newText = GUILayout.TextArea(sliderTexts[i]);
          if (newText != sliderTexts[i] && float.TryParse(newText, out float valueScaled))
          {
            valueScaled = Mathf.Clamp(valueScaled, 0f, bitScale);
            sliderTexts[i] = $"{valueScaled:F0}";
            newValues[i] = valueScaled / bitScale;
          }
        }
        if (i != 3) GUILayout.Space(2);
      }

      if (newValues != rawValues)
      {
        SetColorByRawValues(newValues);
      }
    }

    protected void SetSliderTexts()
    {
      var rawValues = GetRawColorValues();
      for (int i = 0; i < sliderTexts.Length; ++i)
      {
        sliderTexts[i] = $"{rawValues[i] * bitScale:F0}";
      }
    }

    protected bool TryGetRectClickPos(Vector2 mPos, Rect screenRect, out Vector2 result)
    {
      result = Vector2.zero;
      if (screenRect.Contains(mPos))
      {
        dragSliders = true;
        result = new Vector2(Mathf.Clamp(mPos.x - screenRect.xMin, 0, screenRect.width),
         (int)screenRect.width - (int)Mathf.Clamp(mPos.y - screenRect.yMin, 0, screenRect.width));
        return true;
      }
      return false;
    }
    protected bool TryGetRectTextureColor(Vector2 mPos, Rect screenRect, Texture2D texture, out Color result)
    {
      result = new Color();
      if (TryGetRectClickPos(mPos, screenRect, out Vector2 clickPos))
      {
        result = texture.GetPixel((int)clickPos.x, (int)clickPos.y);
        return true;
      }
      return false;
    }

    protected void HandleColorFieldClick(Vector2 mPos)
    {
      if (TryGetRectTextureColor(mPos, colorFieldRect, (Texture2D)colorField, out Color colorFieldSample))
      {
        color = new(colorFieldSample.r, colorFieldSample.g, colorFieldSample.b, color.a);
      }
    }
    protected void HandleHueBarClick(Vector2 mPos)
    {
      if (TryGetRectClickPos(mPos, hueBarRect, out Vector2 clickPos))
      {
        hue = clickPos.x / hueBarRect.width;
      }
    }

    protected void HandleColorBarClick(Vector2 mPos, Rect barRect, int barID)
    {
      if (TryGetRectClickPos(mPos, barRect, out Vector2 clickPos))
      {
        var rawValues = GetRawColorValues();
        rawValues[barID] = clickPos.x / barRect.width;
        SetColorByRawValues(rawValues);
      }
    }

    protected void HandleClicks()
    {
      if (Event.current.type == EventType.Repaint)
        if (Input.GetMouseButton(0))
        {
          Vector2 mPos = new(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
          HandleHueBarClick(mPos);
          HandleColorFieldClick(mPos);
          for (int i = 0; i < sliderBarRects.Length; ++i)
          {
            HandleColorBarClick(mPos, sliderBarRects[i], i);
          }
        }
    }

    protected void GenerateTextures()
    {
      rainbow = TextureUtils.GenerateRainbowGradient(120, 20);
      colorField = TextureUtils.GenerateColorField(colorFieldSize, colorFieldSize, color);

      if (RGBMode)
      {
        sliderTextures[0] = TextureUtils.GenerateGradientTexture(100, 20,
          new Color(0.0f, color.g, color.b, 1.0f),
          new Color(1.0f, color.g, color.b, 1.0f));
        sliderTextures[1] = TextureUtils.GenerateGradientTexture(100, 20,
          new Color(color.r, 0f, color.b, 1.0f),
          new Color(color.r, 1f, color.b, 1.0f));
        sliderTextures[2] = TextureUtils.GenerateGradientTexture(100, 20,
          new Color(color.r, color.g, 0f, 1.0f),
          new Color(color.r, color.g, 1f, 1.0f));
      }
      else
      {
        sliderTextures[0] = TextureUtils.GenerateRainbowGradient(120, 20);
        sliderTextures[1] = TextureUtils.GenerateGradientTexture(100, 20,
          new ColorHSV(colorHSV.h, 0f, colorHSV.v).ToRGB(),
          new ColorHSV(colorHSV.h, 1f, colorHSV.v).ToRGB());
        sliderTextures[2] = TextureUtils.GenerateGradientTexture(100, 20,
          new ColorHSV(colorHSV.h, colorHSV.s, 0f).ToRGB(),
          new ColorHSV(colorHSV.h, colorHSV.s, 1f).ToRGB());
      }
      sliderTextures[3] = TextureUtils.GenerateGradientTexture(100, 20,
        Color.black,
        Color.white);

      swatch = TextureUtils.GenerateColorTexture(40, 20, color);

      barCarat = TextureUtils.GenerateSliderCarat(5, 20,
        new Color(1f, 1f, 1f, 0.75f),
        Color.black);

      fieldCarat = TextureUtils.GenerateRoundCarat(20, 20, Color.white, Color.black);
    }
  }
}
