using UnityEngine;
using Random = System.Random;

namespace Waterfall.UI
{
  public class UIBaseWindow : MonoBehaviour
  {
    // Control Vars
    protected static bool showWindow;
    public           Rect windowPos = new(200f, 200f, 1000f, 400f);
    protected        bool initUI;

    // Assets
    private   float       scrollHeight   = 0f;
    private   Vector2     scrollPosition = Vector2.zero;
    protected int         windowID       = new Random(13123).Next();


    public Rect WindowPosition
    {
      get => windowPos;
      set => windowPos = value;
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
    ///   Turn the window on or off
    /// </summary>
    public static void ToggleWindow()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Toggle Window");
      showWindow = !showWindow;
    }

    /// <summary>
    ///   Initialize the UI comoponents, do localization, set up styles
    /// </summary>
    protected virtual void InitUI()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Initializing");
      initUI    = true;
    }

    /// <summary>
    ///   Draw the UI
    /// </summary>
    protected virtual void Draw()
    {
      if (!initUI)
        InitUI();

      GUI.skin = HighLogic.Skin;
      if (showWindow)
      {
        //windowPos.height = Mathf.Min(scrollHeight + 50f, 96f * 3f + 50f);
        windowPos = GUILayout.Window(windowID, windowPos, DrawWindow, new GUIContent(), 
          UIResources.GetStyle("window_main"), GUILayout.ExpandHeight(true));
      }
    }

    /// <summary>
    ///   Draw the window
    /// </summary>
    /// <param name="windowId">window ID</param>
    protected virtual void DrawWindow(int windowId) { }
  }
}