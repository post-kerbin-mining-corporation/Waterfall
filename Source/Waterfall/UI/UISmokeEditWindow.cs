using UnityEngine;
using Waterfall.Modules;

namespace Waterfall.UI
{
  public class UISmokeEditWindow : UIPopupWindow
  {
    protected        string   windowTitle = "";
    private readonly string[] startSizeRangeString;
    private readonly string[] lifetimeRangeString;
    private readonly string[] emissionRateRangeString;
    private readonly string[] emissionSpeedRangeString;

    private readonly ModuleWaterfallSmoke smokeModule;

    private Vector2 startSizeRange;
    private Vector2 lifetimeRange;
    private Vector2 emissionRateRange;
    private Vector2 emissionSpeedRange;

    public UISmokeEditWindow(ModuleWaterfallSmoke smoke, bool show) : base(show)
    {
      smokeModule = smoke;
      Utils.Log($"[UISmokeEditWindoww]: Started editing smoke on {true.ToString()}", LogType.UI);

      startSizeRange     = smoke.startSizeRange;
      emissionRateRange  = smoke.emissionRateRange;
      emissionSpeedRange = smoke.emissionSpeedRange;
      lifetimeRange      = smoke.lifetimeRange;

      startSizeRangeString     = new[] { startSizeRange.x.ToString(), startSizeRange.y.ToString() };
      emissionRateRangeString  = new[] { emissionRateRange.x.ToString(), emissionRateRange.y.ToString() };
      emissionSpeedRangeString = new[] { emissionSpeedRange.x.ToString(), emissionSpeedRange.y.ToString() };
      lifetimeRangeString      = new[] { lifetimeRange.x.ToString(), lifetimeRange.y.ToString() };

      WindowPosition = new(Screen.width / 2 - 200, Screen.height / 2f, 400, 100);
    }

    public void Update()
    {
      if (smokeModule)
      {
        if (smokeModule.startSizeRange != startSizeRange || smokeModule.emissionRateRange != emissionRateRange || smokeModule.emissionSpeedRange != emissionSpeedRange || smokeModule.lifetimeRange != lifetimeRange)
        {
          smokeModule.SetRanges(emissionRateRange, emissionSpeedRange, startSizeRange, lifetimeRange);
        }
      }
    }

    protected override void InitUI()
    {
      windowTitle = "Smoke Editor";
      base.InitUI();
    }

    protected override void DrawWindow(int windowId)
    {
      DrawTitle();
      DrawEditor();
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

    protected void DrawEditor()
    {
      bool delta = false;

      GUILayout.BeginVertical(GUILayout.MaxWidth(400f));
      GUILayout.BeginHorizontal();
      GUILayout.Label("Start Size");
      startSizeRange = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), startSizeRange, startSizeRangeString, GUI.skin.label, GUI.skin.textArea, out delta);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Emission Rate");
      emissionRateRange = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), emissionRateRange, emissionRateRangeString, GUI.skin.label, GUI.skin.textArea, out delta);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Emission Speed");
      emissionSpeedRange = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), emissionSpeedRange, emissionSpeedRangeString, GUI.skin.label, GUI.skin.textArea, out delta);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Lifetime Range");
      lifetimeRange = UIUtils.Vector2InputField(GUILayoutUtility.GetRect(200f, 30f), lifetimeRange, lifetimeRangeString, GUI.skin.label, GUI.skin.textArea, out delta);
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
    }
  }
}