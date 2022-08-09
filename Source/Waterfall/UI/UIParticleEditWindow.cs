using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  public class UIParticleEditWindow : UIPopupWindow
  {
    protected string windowTitle = "";
    private WaterfallParticle particle;
    private WaterfallModel model;
    private int materialID;


    private Dictionary<string, Vector2> rangeValues = new();
    private Dictionary<string, float> floatValues = new();
    private Dictionary<string, string[]> rangeStrings = new();
    private Dictionary<string, string> floatStrings = new();


    public UIParticleEditWindow(WaterfallModel modelToEdit, bool show) : base(show)
    {
      materialID = 0;
      model = modelToEdit;
      Utils.Log($"[UIParticleEditWindow]: Started editing particles on {modelToEdit}", LogType.UI);

      particle = modelToEdit.particles[materialID];


      InitializeParticleProperties(particle);
      WindowPosition = new(Screen.width / 2 - 200, Screen.height / 2f, 400, 100);
    }


    public void ChangeParticle(WaterfallModel modelToEdit)
    {
      model = modelToEdit;
      Utils.Log($"[UIParticleEditWindow]: Started editing particles on {modelToEdit}", LogType.UI);
      materialID = 0;
      particle = modelToEdit.particles[materialID];

      showWindow = true;
      GUI.BringWindowToFront(windowID);

      InitializeParticleProperties(particle);
    }

    protected override void InitUI()
    {
      windowTitle = "Particle System Editor";
      base.InitUI();
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      DrawParticleEdit();
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

    protected void DrawParticleEdit()
    {
      
      GUILayout.Label("<b>Particle Parameters</b>");

      DrawFloats();
      DrawRanges();
    }

    protected void DrawFloats()
    {

      foreach (var kvp in floatValues.ToList())
      {
        float headerWidth = 140f;
        float sliderVal;
        string textVal;
        GUILayout.BeginHorizontal();
        GUILayout.Label(kvp.Key, GUILayout.Width(headerWidth));
        sliderVal = GUILayout.HorizontalSlider(floatValues[kvp.Key],
                                               WaterfallParticleLoader.GetParticlePropertyMap()[kvp.Key].floatRange.x,
                                               WaterfallParticleLoader.GetParticlePropertyMap()[kvp.Key].floatRange.y);

        if (sliderVal != floatValues[kvp.Key])
        {
          floatValues[kvp.Key] = sliderVal;
          floatStrings[kvp.Key] = sliderVal.ToString();
          model.SetParticleValue(kvp.Key, floatValues[kvp.Key]);
        }
        textVal = GUILayout.TextArea(floatStrings[kvp.Key], GUILayout.Width(90f));


        if (textVal != floatStrings[kvp.Key])
        {
          float outVal;
          if (Single.TryParse(textVal, out outVal))
          {
            floatValues[kvp.Key] = outVal;
            model.SetParticleValue(kvp.Key, floatValues[kvp.Key]);
          }
          floatStrings[kvp.Key] = textVal;
        }

        GUILayout.EndHorizontal();
      }

    }


    protected void DrawRanges()
    {
      float headerWidth = 140f;
      foreach (var kvp in rangeValues.ToList())
      {
        float sliderValX;
        string textValX;
        float sliderValY;
        string textValY;

        GUILayout.Label($"<b>{kvp.Key}</b>", GUILayout.Width(headerWidth));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Low");
        sliderValX = GUILayout.HorizontalSlider(rangeValues[kvp.Key].x,
                                               WaterfallParticleLoader.GetParticlePropertyMap()[kvp.Key].floatRange.x,
                                               WaterfallParticleLoader.GetParticlePropertyMap()[kvp.Key].floatRange.y);

        if (sliderValX != rangeValues[kvp.Key].x)
        {
          rangeValues[kvp.Key] = new Vector2(sliderValX, rangeValues[kvp.Key].y);
          rangeStrings[kvp.Key][0] = sliderValX.ToString();

          model.SetParticleValue(kvp.Key, rangeValues[kvp.Key]);
        }

        textValX = GUILayout.TextArea(rangeStrings[kvp.Key][0], GUILayout.Width(90f));


        if (textValX != rangeStrings[kvp.Key][0])
        {
          float outVal;
          if (Single.TryParse(textValX, out outVal))
          {
            rangeValues[kvp.Key] = new Vector2(outVal, rangeValues[kvp.Key].y);

            model.SetParticleValue(kvp.Key, rangeValues[kvp.Key]);
          }

          rangeStrings[kvp.Key][0] = textValX;
        }


        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("High");
        sliderValY = GUILayout.HorizontalSlider(rangeValues[kvp.Key].y,
                                               WaterfallParticleLoader.GetParticlePropertyMap()[kvp.Key].floatRange.x,
                                               WaterfallParticleLoader.GetParticlePropertyMap()[kvp.Key].floatRange.y);

        if (sliderValY != rangeValues[kvp.Key].y)
        {
          rangeValues[kvp.Key] = new Vector2(rangeValues[kvp.Key].x, sliderValY);
          rangeStrings[kvp.Key][1] = sliderValY.ToString();
          model.SetParticleValue(kvp.Key, rangeValues[kvp.Key]);
        }

        textValY = GUILayout.TextArea(rangeStrings[kvp.Key][1], GUILayout.Width(90f));


        if (textValY != rangeStrings[kvp.Key][1])
        {
          float outVal;
          if (Single.TryParse(textValY, out outVal))
          {
            rangeValues[kvp.Key] = new Vector2(rangeValues[kvp.Key].x, outVal);
            model.SetParticleValue(kvp.Key, rangeValues[kvp.Key]);
          }

          rangeStrings[kvp.Key][1] = textValY;
        }


        GUILayout.EndHorizontal();
      }

    }


    protected void InitializeParticleProperties(WaterfallParticle p)
    {
      Utils.Log($"[ParticleEditor] Generating particle property map for {p}", LogType.UI);
      
      rangeValues = new();
      rangeStrings = new();
      floatValues = new();
      floatStrings = new();
      

      foreach (var pProp in WaterfallParticleLoader.GetParticlePropertyMap())
      {

        if (pProp.Value.type == WaterfallParticlePropertyType.Range)
        {
          ParticleUtils.GetParticleSystemValue(pProp.Key,  p.systems[0].emitter, out Vector2 vec2);
          rangeValues.Add(pProp.Key, vec2);
          rangeStrings.Add(pProp.Key, new[] { $"{vec2.x}", $"{vec2.y}"});
        }
        if (pProp.Value.type == WaterfallParticlePropertyType.Float)
        {
          ParticleUtils.GetParticleSystemValue(pProp.Key, p.systems[0].emitter, out float val);
          floatValues.Add(pProp.Key, val);
          floatStrings.Add(pProp.Key, $"{val}");
        }

      }
    }
  }
}