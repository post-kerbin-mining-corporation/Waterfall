using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UILightEditWindow : UIPopupWindow
  {

    protected string windowTitle = "";
    WaterfallModel model;
    WaterfallLight light;

    float intensityValue;
    float rangeValue;
    float angleValue;

    Color colorValue;
    bool colorEdit = false;

    string intensityString;
    string rangeString;
    string angleString;
    string[] colorString;

    string[] typeOptions = new string[2] { "Spot", "Point" };
    int typeFlag = 0;

    Texture2D colorTexture;

    public UILightEditWindow(WaterfallModel modelToEdit, bool show) : base(show)
    {

      model = modelToEdit;
      Utils.Log($"[UILightEditWindow]: Started editing lights on {modelToEdit.ToString()}", LogType.UI);


      light = modelToEdit.lights.First();
      GetLightValues();
      WindowPosition = new Rect(Screen.width / 2 - 200, Screen.height / 2f, 400, 100);
    }

    protected override void InitUI()
    {
      windowTitle = "Light Editor";
      base.InitUI();


    }
    void GetLightValues()
    {
      intensityValue = light.intensity;
      intensityString = light.intensity.ToString();

      rangeString = light.range.ToString();
      rangeValue = light.range;

      angleString = light.angle.ToString();
      angleValue = light.angle;

      colorValue = light.color;
      colorString = MaterialUtils.ColorToStringArray(colorValue);
      colorTexture = MaterialUtils.GenerateColorTexture(32, 32, colorValue);

      if (light.lightType == LightType.Point)
      {
        typeFlag = 1;
      }
      else
      {
        typeFlag = 0;
      }
    }

    public void ChangeLight(WaterfallModel modelToEdit)
    {
      model = modelToEdit;
      Utils.Log($"[UILightEditWindow]: Started editing lights on {modelToEdit.ToString()}", LogType.UI);
      
      light = modelToEdit.lights.First();
      if (colorEdit)
      {
        WaterfallUI.Instance.OpenColorEditWindow(light.color);
      }
      GetLightValues();

      showWindow = true;


      WindowPosition = new Rect(Screen.width / 2 - 200, Screen.height / 2f, 400, 100);
    }

    protected override void DrawWindow(int windowId)
    {

      // Draw the header/tab controls
      DrawTitle();
      DrawLightEdit();
      GUI.DragWindow();

    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    protected void DrawLightEdit()
    {
      float headerWidth = 120f;
      bool delta = false;
      float sliderVal;
      string textVal;

      GUILayout.Label("<b>Light Parameters</b>");

      GUILayout.BeginHorizontal();
      GUILayout.Label("Light Color", GUILayout.Width(headerWidth));
      GUILayout.Space(10);

      // Button to set that we are toggling the color picker
      if (GUILayout.Button("", GUILayout.Width(60)))
      {
        colorEdit = !colorEdit;
        Utils.Log($"[CP] Edit flag state {colorEdit}", LogType.UI);
        // if yes, open the window
        if (colorEdit)
        {
          WaterfallUI.Instance.OpenColorEditWindow(colorValue);
          Utils.Log("[CP] Open Window", LogType.UI);
        }
      }

      // If picker open
      if (colorEdit)
      {
        // Close all other pickers


        Color c = WaterfallUI.Instance.GetColorFromPicker();
        if (!c.IsEqualTo(colorValue))
        {
          colorValue = c;
          delta = true;
        }
        if (delta)
        {
          colorTexture = MaterialUtils.GenerateColorTexture(64, 32, colorValue);
          model.SetLightColor(light, colorValue);
        }
      }
      

      
      Rect tRect = GUILayoutUtility.GetLastRect();
      tRect = new Rect(tRect.x + 3, tRect.y + 3, tRect.width - 6, tRect.height - 6);
      GUI.DrawTexture(tRect, colorTexture);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Light Type", GUILayout.Width(headerWidth));
      int newFlag = GUILayout.SelectionGrid(typeFlag,
        typeOptions, 2,
        GUIResources.GetStyle("radio_text_button"));


      if (newFlag != typeFlag)
      {
        typeFlag = newFlag;
        if (typeFlag == 1)
        model.SetLightType(light, LightType.Point);
        if (typeFlag == 0)
          model.SetLightType(light, LightType.Spot);
      }

      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Intensity", GUILayout.Width(headerWidth));
      sliderVal = GUILayout.HorizontalSlider(intensityValue, 0f, 10f);
      if (sliderVal != intensityValue)
      {
        intensityValue = sliderVal;
        intensityString = sliderVal.ToString();
        model.SetLightIntensity(light, intensityValue);
      }

      textVal = GUILayout.TextArea(intensityString, GUILayout.Width(90f));
      if (textVal != intensityString)
      {
        float outVal;
        if (float.TryParse(textVal, out outVal))
        {
          intensityValue = outVal;
          model.SetLightIntensity(light, intensityValue);
        }
        intensityString = textVal;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Range", GUILayout.Width(headerWidth));
      sliderVal = GUILayout.HorizontalSlider(rangeValue, 0f, 100f);
      if (sliderVal != rangeValue)
      {
        rangeValue = sliderVal;
        rangeString = sliderVal.ToString();
        model.SetLightRange(light, rangeValue);
      }

      textVal = GUILayout.TextArea(rangeString, GUILayout.Width(90f));
      if (textVal != rangeString)
      {
        float outVal;
        if (float.TryParse(textVal, out outVal))
        {
          rangeValue = outVal;
          model.SetLightRange(light, rangeValue);
        }
        rangeString = textVal;
      }
      GUILayout.EndHorizontal();
      if (typeFlag == 0)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Spot Angle", GUILayout.Width(headerWidth));
        sliderVal = GUILayout.HorizontalSlider(angleValue, 0f, 100f);
        if (sliderVal != angleValue)
        {
          angleValue = sliderVal;
          angleString = sliderVal.ToString();
          model.SetLightAngle(light, angleValue);
        }

        textVal = GUILayout.TextArea(angleString, GUILayout.Width(90f));
        if (textVal != angleString)
        {
          float outVal;
          if (float.TryParse(textVal, out outVal))
          {
            angleValue = outVal;
            model.SetLightAngle(light, angleValue);
          }
          angleString = textVal;
        }
        GUILayout.EndHorizontal();
      }
    }

  }
}

