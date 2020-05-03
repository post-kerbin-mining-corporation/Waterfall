using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace Waterfall.UI
{

  public class UIBaseWindow : MonoBehaviour
  {
    // Control Vars
    protected static bool showWindow = false;
    protected int windowID = new System.Random(13123).Next();
    public Rect windowPos = new Rect(200f, 200f, 1000f, 400f);
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
    public static void ToggleWindow()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Toggle Window");
      showWindow = !showWindow;
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

    protected virtual void Awake()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Awake fired");
    }

    protected virtual void Start()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Start fired");
      
    }

    protected virtual void OnGUI()
    {
      if (Event.current.type == EventType.Repaint || Event.current.isMouse) { }
      Draw();
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    protected virtual void Draw()
    {
      if (!initUI)
        InitUI();

      if (showWindow)
      {
        GUI.skin = HighLogic.Skin;
        //windowPos.height = Mathf.Min(scrollHeight + 50f, 96f * 3f + 50f);
        windowPos = GUI.Window(windowID, windowPos, DrawWindow, new GUIContent(), GUIResources.GetStyle("window_main"));
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
