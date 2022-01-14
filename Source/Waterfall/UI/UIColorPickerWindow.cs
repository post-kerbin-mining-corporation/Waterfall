using System;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIColorPickerWindow : UIPopupWindow
  {
    protected        string windowTitle    = "";
    private readonly int    colorFieldSize = 150;
    private readonly float  sliderWidth    = 150f;

    private float rValue;
    private float gValue;
    private float bValue;
    private float aValue;


    private string rText = "";
    private string gText = "";
    private string bText = "";
    private string aText = "";

    private Texture swatch;

    private Texture rTexture;
    private Texture gTexture;
    private Texture bTexture;
    private Texture aTexture;

    private Texture rainbow;
    private Texture colorField;

    private float hueValue;
    private float prevHue = 0f;


    private HSBColor currentHSVColor;
    private HSBColor prevHSVColor;

    private Color currentColor;
    private Color savedColor;
    private Color prevColor;

    public UIColorPickerWindow(Color colorToEdit, bool show) : base(show)
    {
      currentColor = colorToEdit;
      savedColor   = colorToEdit;
      prevColor    = colorToEdit;

      aValue = currentColor.a * 255f;
      rValue = currentColor.r * 255f;
      gValue = currentColor.g * 255f;
      bValue = currentColor.b * 255f;

      aText           = aValue.ToString("F0");
      rText           = rValue.ToString("F0");
      gText           = gValue.ToString("F0");
      bText           = bValue.ToString("F0");
      currentHSVColor = new(currentColor);
      prevHSVColor    = new(currentColor);

      GenerateTextures();
      rainbow    = MaterialUtils.GenerateRainbowGradient(120, 20);
      aTexture   = MaterialUtils.GenerateGradientTexture(100, 20, Color.black, Color.white);
      colorField = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());

      WindowPosition = new(Screen.width / 2 - 100, Screen.height / 2f, 210, 100);
    }

    public Color GetColor() => currentColor;

    public void ChangeColor(Color colorToEdit)
    {
      currentColor = colorToEdit;
      savedColor   = colorToEdit;
      prevColor    = colorToEdit;

      aValue = currentColor.a * 255f;
      rValue = currentColor.r * 255f;
      gValue = currentColor.g * 255f;
      bValue = currentColor.b * 255f;

      aText = aValue.ToString("F0");
      rText = rValue.ToString("F0");
      gText = gValue.ToString("F0");
      bText = bValue.ToString("F0");

      currentHSVColor = new(currentColor);
      prevHSVColor    = new(currentColor);

      GenerateTextures();
      rainbow    = MaterialUtils.GenerateRainbowGradient(120, 20);
      aTexture   = MaterialUtils.GenerateGradientTexture(100, 20, Color.black, Color.white);
      colorField = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());

      if (!showWindow)
        WindowPosition = new(Screen.width / 2 - 100, Screen.height / 2f, 210, 100);

      GUI.BringWindowToFront(windowID);
    }

    public void Update()
    {
      if (!currentColor.Equals(prevColor))
      {
        GenerateTextures();

        currentHSVColor = new(currentColor);
        hueValue        = currentHSVColor.h;
        colorField      = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());
        prevColor       = currentColor;
      }
    }

    public void GenerateTextures()
    {
      rTexture = MaterialUtils.GenerateGradientTexture(100, 20, new(0.0f, currentColor.g, currentColor.b, 1.0f), new(1.0f, currentColor.g, currentColor.b, 1.0f));
      gTexture = MaterialUtils.GenerateGradientTexture(100, 20, new(currentColor.r, 0f, currentColor.b, 1.0f),   new(currentColor.r, 1f, currentColor.b, 1.0f));
      bTexture = MaterialUtils.GenerateGradientTexture(100, 20, new(currentColor.r, currentColor.g, 0f, 1.0f),   new(currentColor.r, currentColor.g, 1f, 1.0f));

      swatch = MaterialUtils.GenerateColorTexture(60, 20, currentColor);
    }

    protected override void InitUI()
    {
      windowTitle = "Color Picker";

      base.InitUI();
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      DrawColorField();
      DrawSliders();
      GUI.DragWindow();
    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, UIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      var buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = UIResources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", UIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, UIResources.GetIcon("cancel").iconAtlas, UIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected void DrawColorField()
    {
      GUILayout.BeginVertical();
      GUILayout.Space(4);
      var tRect = GUILayoutUtility.GetRect(120, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, swatch);
      GUILayout.Space(4);
      tRect = GUILayoutUtility.GetRect(150, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, rainbow);
      hueValue        = GUILayout.HorizontalSlider(hueValue, 0f, 1f, GUILayout.Width(sliderWidth));
      currentHSVColor = new(hueValue, currentHSVColor.s, currentHSVColor.b);
      tRect           = GUILayoutUtility.GetRect(colorFieldSize, colorFieldSize, GUILayout.Width(colorFieldSize));
      GUI.DrawTexture(tRect, colorField);

      var eventType = Event.current.type;
      if (eventType == EventType.Repaint)
        if (Input.GetMouseButtonDown(0))
        {
          var screenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
          var screenRect  = GUIUtility.GUIToScreenRect(tRect);


          if (tRect.Contains(Event.current.mousePosition))
          {
            int x       = (int)Mathf.Clamp(Event.current.mousePosition.x - tRect.xMin, 0, sliderWidth);
            int y       = colorFieldSize - (int)Mathf.Clamp(Event.current.mousePosition.y - tRect.yMin, 0, sliderWidth);
            var sampled = ((Texture2D)colorField).GetPixel(x, y);
            Utils.Log($"Color picked at {x}, {y} is {sampled}", LogType.UI);
            currentHSVColor = new(sampled);
            currentColor    = new(sampled.r, sampled.g, sampled.b, currentColor.a);
            aValue          = currentColor.a * 255f;
            rValue          = currentColor.r * 255f;
            gValue          = currentColor.g * 255f;
            bValue          = currentColor.b * 255f;

            aText = aValue.ToString("F0");
            rText = rValue.ToString("F0");
            gText = gValue.ToString("F0");
            bText = bValue.ToString("F0");
          }
        }

      GUILayout.EndVertical();
      GUILayout.Space(4);


      if (!currentHSVColor.ToColor().Equals(prevHSVColor.ToColor()))
      {
        colorField   = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());
        prevHSVColor = currentHSVColor;
      }
    }

    protected void DrawSliders()
    {
      float  sliderValue;
      string fieldText;
      GUILayout.BeginHorizontal();
      GUILayout.Label("R");
      GUILayout.BeginVertical();
      var tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, rTexture);
      sliderValue = GUILayout.HorizontalSlider(rValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != rValue)
      {
        rValue = sliderValue;
        rText  = rValue.ToString("F0");
      }

      fieldText = GUILayout.TextArea(rText);

      if (fieldText != rText)
      {
        if (Single.TryParse(fieldText, out rValue))
        {
          rText = fieldText;
        }
      }

      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label("G");
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, gTexture);
      sliderValue = GUILayout.HorizontalSlider(gValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != gValue)
      {
        gValue = sliderValue;
        gText  = gValue.ToString("F0");
      }

      fieldText = GUILayout.TextArea(gText);

      if (fieldText != gText)
      {
        if (Single.TryParse(fieldText, out gValue))
        {
          gText = fieldText;
        }
      }

      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("B");
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, bTexture);
      sliderValue = GUILayout.HorizontalSlider(bValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != bValue)
      {
        bValue = sliderValue;
        bText  = bValue.ToString("F0");
      }

      fieldText = GUILayout.TextArea(bText);
      if (fieldText != bText)
      {
        if (Single.TryParse(fieldText, out bValue))
        {
          bText = fieldText;
        }
      }

      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("A");
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, aTexture);
      sliderValue = GUILayout.HorizontalSlider(aValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != aValue)
      {
        aValue = sliderValue;
        aText  = aValue.ToString();
      }

      fieldText = GUILayout.TextArea(aText);
      if (fieldText != aText)
      {
        if (Single.TryParse(fieldText, out aValue))
        {
          aText = fieldText;
        }
      }

      GUILayout.EndHorizontal();

      currentColor = new(rValue / 255f, gValue / 255f, bValue / 255f, aValue / 255f);
    }
  }
}