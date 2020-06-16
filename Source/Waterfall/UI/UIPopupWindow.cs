using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIPopupWindow
  {
    // Control Vars
    protected bool showWindow = false;
    protected int windowID = new System.Random(123123).Next();
    public Rect windowPos = new Rect(200f, 200f, 500f, 400f);
    private Vector2 scrollPosition = Vector2.zero;
    private float scrollHeight = 0f;
    protected bool initUI = false;

    // Assets
    protected UIResources resources;

   

    public Rect WindowPosition { get { return windowPos; } set { windowPos = value; } }
    public UIResources GUIResources { get { return resources; } }

    /// <summary>
    /// Turn the window on or off
    /// </summary>
    public void ToggleWindow()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Toggle Window");
      showWindow = !showWindow;
    }

    public void SetWindowState(bool on)
    {
      showWindow = on;
    }

    /// <summary>
    /// Initialize the UI comoponents, do localization, set up styles
    /// </summary>
    protected virtual void InitUI()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Initializing");

      resources = new UIResources();
      initUI = true;
    }
    

    public UIPopupWindow(bool show)
    {
      windowID = new System.Random().Next();
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Start fired");
      showWindow = show;

    }
    
    /// <summary>
    /// Draw the UI
    /// </summary>
    public virtual void Draw()
    {
      if (!initUI)
        InitUI();

      if (showWindow)
      {
        //windowPos.height = Mathf.Min(scrollHeight + 50f, 96f * 3f + 50f);
        windowPos = GUILayout.Window(windowID, windowPos, DrawWindow, new GUIContent(), GUIResources.GetStyle("window_main"),GUILayout.ExpandHeight(true));
      }
    }

    /// <summary>
    /// Draw the window
    /// </summary>
    /// <param name="windowId">window ID</param>
    protected virtual void DrawWindow(int windowId)
    { }

   

  }


}
