using System;
using System.Globalization;
using UnityEngine;

namespace Waterfall.UI.EffectControllersUI
{
  public sealed class CustomPullControllerUIOptions : DefaultEffectControllerUIOptions<CustomPullController>
  {
    private string memberName;

    private string minInputValueString;
    private string maxInputValueString;

    private string responseRateUpString;
    private string responseRateDownString;

    public CustomPullControllerUIOptions()
    {
      LoadOptions(new()); // Initialize default values from dummy model instance
    }

    private static float ParseOrZero(string str)
    {
      if (Single.TryParse(str, out float result))
      {
        return result;
      }

      return 0;
    }

    public override void DrawOptions()
    {
      GUILayout.BeginVertical();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Member", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(100));
      memberName = GUILayout.TextArea(memberName, GUILayout.MaxWidth(300));
      GUILayout.EndHorizontal();

      GUILayout.Label("Specified member should be public instance parameterless method, property or field on ModuleEngines component returning numeric value", UIResources.GetStyle("data_comment"), GUILayout.MaxWidth(600));

      GUILayout.EndVertical();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Min Expected Value", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      minInputValueString = GUILayout.TextArea(minInputValueString, GUILayout.MaxWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Max Expected Value", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      maxInputValueString = GUILayout.TextArea(maxInputValueString, GUILayout.MaxWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Response Rate Up", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      responseRateUpString = GUILayout.TextArea(responseRateUpString, GUILayout.MaxWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Response Rate Down", UIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      responseRateDownString = GUILayout.TextArea(responseRateDownString, GUILayout.MaxWidth(60f));
      GUILayout.EndHorizontal();
    }

    protected override void LoadOptions(CustomPullController controller)
    {
      memberName             = controller.memberName;
      minInputValueString    = controller.minInputValue.ToString(CultureInfo.InvariantCulture);
      maxInputValueString    = controller.maxInputValue.ToString(CultureInfo.InvariantCulture);
      responseRateUpString   = controller.responseRateUp.ToString(CultureInfo.InvariantCulture);
      responseRateDownString = controller.responseRateDown.ToString(CultureInfo.InvariantCulture);
    }

    protected override CustomPullController CreateControllerInternal() =>
      new()
      {
        memberName       = memberName,
        minInputValue    = ParseOrZero(minInputValueString),
        maxInputValue    = ParseOrZero(maxInputValueString),
        responseRateUp   = ParseOrZero(responseRateUpString),
        responseRateDown = ParseOrZero(responseRateDownString)
      };
  }
}