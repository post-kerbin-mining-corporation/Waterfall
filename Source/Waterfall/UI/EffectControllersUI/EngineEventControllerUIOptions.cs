using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class EngineEventControllerUIOptions : DefaultEffectControllerUIOptions<EngineEventController>
  {

    private readonly string[] eventTypes      = { "ignition", "flameout" };
    private readonly Vector2  curveButtonDims = new(100f, 50f);

    private readonly int               texWidth  = 80;
    private readonly int               texHeight = 30;
    private          int               eventFlag;
    private          FloatCurve        eventCurve;
    private          float             eventDuration = 2f;
    private          string            eventDurationString;
    private          Texture2D         miniCurve;
    private          UICurveEditWindow curveEditor;

    public EngineEventControllerUIOptions()
    {
      eventDurationString = eventDuration.ToString();
      eventCurve          = new();
      eventCurve.Add(0f,   0f);
      eventCurve.Add(0.1f, 1f);
      eventCurve.Add(1f,   0f);
      GenerateCurveThumbs();
    }

    public override void DrawOptions()
    {
      GUILayout.Label("Event name");
      int eventFlagChanged = GUILayout.SelectionGrid(eventFlag, eventTypes, Mathf.Min(eventTypes.Length, 4), UIResources.GetStyle("radio_text_button"));

      eventFlag = eventFlagChanged;

      var buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      var imageRect  = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(eventCurve, UpdateEventCurve);
      }

      GUI.DrawTexture(imageRect, miniCurve);
      GUILayout.BeginHorizontal();
      GUILayout.Label("Event duration", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      eventDurationString = GUILayout.TextArea(eventDurationString, GUILayout.MaxWidth(60f));
      if (Single.TryParse(eventDurationString, out float floatParsed))
      {
        eventDuration = floatParsed;
      }

      GUILayout.EndHorizontal();
    }

    protected override void LoadOptions(EngineEventController controller)
    {
      eventFlag           = Math.Max(0, eventTypes.IndexOf(controller.eventName));
      eventCurve          = controller.eventCurve;
      eventDuration       = controller.eventDuration;
      eventDurationString = controller.eventDuration.ToString();

      GenerateCurveThumbs();
    }

    protected override EngineEventController CreateControllerInternal() =>
      new()
      {
        eventName     = eventTypes[eventFlag],
        eventCurve    = eventCurve,
        eventDuration = eventDuration
      };

    private void EditCurve(FloatCurve toEdit, CurveUpdateFunction function)
    {
      Utils.Log($"Started editing curve {toEdit.Curve}", LogType.UI);
      curveEditor = WaterfallUI.Instance.OpenCurveEditor(toEdit, function);
    }

    private void GenerateCurveThumbs()
    {
      miniCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, eventCurve, Color.green);
    }

    private void UpdateEventCurve(FloatCurve curve)
    {
      eventCurve = curve;
      GenerateCurveThumbs();
    }
  }
}