using KSP.UI.Screens;

namespace Waterfall.UI
{
  public class UIAppToolbarWindow : UIBaseWindow
  {
    // Stock toolbar button
    protected static ApplicationLauncherButton stockToolbarButton;

    protected override void Awake()
    {
      base.Awake();
      if (Settings.ShowEffectEditor)
      {
        GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
      }
    }

    protected override void Start()
    {
      base.Start();
      if (Settings.ShowEffectEditor && ApplicationLauncher.Ready)
        OnGUIAppLauncherReady();
    }

    // Stock toolbar handling methods
    public void OnDestroy()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: OnDestroy Fired");
      // Remove the stock toolbar button
      GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
      if (stockToolbarButton != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
      }
    }

    protected void OnToolbarButtonToggle()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Toolbar Button Toggled");
      ToggleWindow();
      stockToolbarButton.SetTexture(GameDatabase.Instance.GetTexture(showWindow ? UIConstants.UIApplicationButton_On : UIConstants.UIApplicationButton_Off, false));
    }


    protected void OnGUIAppLauncherReady()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: App Launcher Ready");

      if (ApplicationLauncher.Ready && stockToolbarButton == null)
      {
        stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(OnToolbarButtonToggle,
                                                                            OnToolbarButtonToggle,
                                                                            DummyVoid,
                                                                            DummyVoid,
                                                                            DummyVoid,
                                                                            DummyVoid,
                                                                            ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
                                                                            GameDatabase.Instance.GetTexture(UIConstants.UIApplicationButton_Off, false));
      }
    }

    protected void OnGUIAppLauncherDestroyed()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: App Launcher Destroyed");
      if (stockToolbarButton != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
        stockToolbarButton = null;
      }
    }

    protected void onAppLaunchToggleOff()
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: App Launcher Toggle Off");
      stockToolbarButton.SetTexture(GameDatabase.Instance.GetTexture(UIConstants.UIApplicationButton_Off, false));
    }

    protected void DummyVoid() { }
  }
}