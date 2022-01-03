using System;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public class EngineEventControllerUIOptions : DefaultEffectControllerUIOptions<EngineEventController>
  {
    private readonly UIResources guiResources;

    private string[] eventTypes = { "ignition", "flameout" };
    private int eventFlag = 0;
    private FloatCurve eventCurve;
    private float eventDuration = 2f;
    private string eventDurationString;
    private Texture2D miniCurve;
    private Vector2 curveButtonDims = new Vector2(100f, 50f);
    private UICurveEditWindow curveEditor;

    int texWidth = 80;
    int texHeight = 30;

    public EngineEventControllerUIOptions(UIResources guiResources)
    {
      this.guiResources = guiResources ?? throw new ArgumentNullException(nameof(guiResources));

      eventDurationString = eventDuration.ToString();
      eventCurve = new FloatCurve();
      eventCurve.Add(0f, 0f);
      eventCurve.Add(0.1f, 1f);
      eventCurve.Add(1f, 0f);
      GenerateCurveThumbs();
    }

    public override void DrawOptions()
    {
      GUILayout.Label("Event name");
      int eventFlagChanged = GUILayout.SelectionGrid(eventFlag, eventTypes, Mathf.Min(eventTypes.Length, 4), guiResources.GetStyle("radio_text_button"));

      eventFlag = eventFlagChanged;

      Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
      Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
      if (GUI.Button(buttonRect, ""))
      {
        EditCurve(eventCurve, UpdateEventCurve);
      }

      GUI.DrawTexture(imageRect, miniCurve);
      GUILayout.BeginHorizontal();
      GUILayout.Label("Event duration", guiResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      eventDurationString = GUILayout.TextArea(eventDurationString, GUILayout.MaxWidth(60f));
      if (float.TryParse(eventDurationString, out float floatParsed))
      {
        eventDuration = floatParsed;
      }

      GUILayout.EndHorizontal();
    }

    protected override void LoadOptions(EngineEventController controller)
    {
      eventCurve = controller.eventCurve;
      eventDuration = controller.eventDuration;
      eventDurationString = controller.eventDuration.ToString();

      GenerateCurveThumbs();
    }

    protected override EngineEventController CreateControllerInternal()
    {
      return new EngineEventController
      {
        eventName = eventTypes[eventFlag],
        eventCurve = eventCurve,
        eventDuration = eventDuration
      };
    }

    private void EditCurve(FloatCurve toEdit, CurveUpdateFunction function)
    {
      Utils.Log($"Started editing curve {toEdit.Curve.ToString()}", LogType.UI);
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