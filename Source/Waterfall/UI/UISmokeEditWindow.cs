using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Waterfall.Modules;

namespace Waterfall.UI
{
  public class UISmokeEditWindow : UIPopupWindow
  {


    protected string windowTitle = "";

    Vector2 startSizeRange;
    string[] startSizeRangeString;
    Vector2 lifetimeRange;
    string[] lifetimeRangeString;
    Vector2 emissionRateRange;
    string[] emissionRateRangeString;
    Vector2 emissionSpeedRange;
    string[] emissionSpeedRangeString;

    ModuleWaterfallSmoke smokeModule;

    public UISmokeEditWindow(ModuleWaterfallSmoke smoke, bool show) : base(show)
    {
      smokeModule = smoke;
      Utils.Log($"[UISmokeEditWindoww]: Started editing smoke on {true.ToString()}", LogType.UI);

      startSizeRange = smoke.startSizeRange;
      emissionRateRange = smoke.emissionRateRange;
      emissionSpeedRange = smoke.emissionSpeedRange;
      lifetimeRange = smoke.lifetimeRange;

      startSizeRangeString = new string[] {startSizeRange.x.ToString(), startSizeRange.y.ToString() };
      emissionRateRangeString = new string[] { emissionRateRange.x.ToString(), emissionRateRange.y.ToString() };
      emissionSpeedRangeString = new string[] { emissionSpeedRange.x.ToString(), emissionSpeedRange.y.ToString() };
      lifetimeRangeString = new string[] { lifetimeRange.x.ToString(), lifetimeRange.y.ToString() };

      WindowPosition = new Rect(Screen.width / 2 - 200, Screen.height / 2f, 400, 100);
    }

    protected override void InitUI()
    {
      windowTitle = "Smoke Editor";
      base.InitUI();


    }
    public void Update()
    { 
    
      if (smokeModule)
      {
        if (smokeModule.startSizeRange != startSizeRange ||
          smokeModule.emissionRateRange != emissionRateRange ||
          smokeModule.emissionSpeedRange != emissionSpeedRange ||
          smokeModule.lifetimeRange != lifetimeRange)
        {
         // Utils.Log(string.Format("{0} {1} {2} {3}", emissionRateRange, emissionSpeedRange, startSizeRange, lifetimeRange));
          smokeModule.SetRanges(
             emissionRateRange, emissionSpeedRange, startSizeRange, lifetimeRange);
        }
      }
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
