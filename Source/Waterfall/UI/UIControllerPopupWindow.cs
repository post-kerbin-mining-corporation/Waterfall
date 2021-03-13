using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall;

namespace Waterfall.UI
{
  public enum ControllerPopupMode
  {
    Add,
    Delete,
    Modify
  }
  public class UIControllerPopupWindow : UIPopupWindow
  {
    protected int texWidth = 80;
    protected int texHeight = 30;
    Texture2D miniCurve;
    Vector2 curveButtonDims = new Vector2(100f, 50f);

    protected string windowTitle = "";
    ControllerPopupMode windowMode;

    #region GUI Widgets
    protected UICurveEditWindow curveEditor;
    #endregion

    WaterfallController control;
    ModuleWaterfallFX fxMod;
    string newControllerName = "controller";
    string[] controllerTypes = new string[] { "atmosphere_density", "custom", "engineEvent", "gimbal", "light", "mach", "random", "rcs", "throttle" };
    int controllerFlag = 0;
    CurveUpdateFunction eventFun;

    // throttle
    string[] throttleStrings = new string[2];
    float rampRateUp = 100f;
    float rampRateDown = 100f;

    // Azis
    string[] axisTypes = new string[] { "x", "y", "z" };
    int axisFlag = 0;

    // Randomness
    string[] randTypes = new string[] { "random", "perlin" };
    int randFlag = 0;

    string[] randomStrings = new string[4];
    Vector2 randomRange;
    int perlinSeed = 0;
    float perlinMin = 0f;
    float perlinScale = 1f;
    float perlinSpeed = 1f;

    // Event
    string[] eventTypes = new string[] { "ignition", "flameout" };
    int eventFlag = 0;
    FloatCurve eventCurve = new FloatCurve();

    public UIControllerPopupWindow(bool show) : base(show)
    {
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2f, 400, 400);
      eventFun = new CurveUpdateFunction(UpdateEventCurve);
    }

    public void SetDeleteMode(WaterfallController ctrl, ModuleWaterfallFX mod)
    {
      showWindow = true;
      control = ctrl;
      fxMod = mod;
      windowMode = ControllerPopupMode.Delete;
      WindowPosition = new Rect(Screen.width / 2 - 100, Screen.height / 2f - 50, 200, 100);
    }
    public void SetEditMode(WaterfallController ctrl, ModuleWaterfallFX mod)
    {
      showWindow = true;
      control = ctrl;
      fxMod = mod;
      newControllerName = ctrl.name;
      controllerFlag = controllerTypes.ToList().IndexOf(ctrl.linkedTo);
      windowMode = ControllerPopupMode.Modify;
      WindowPosition = new Rect(Screen.width / 2 - 100, Screen.height / 2f - 50, 400, 100);
      eventCurve = new FloatCurve();
      try
      {
        RandomnessController r = (RandomnessController)control;
        if (r.noiseType == "random")
        {
          randomStrings[0] = r.range.x.ToString();
          randomStrings[1] = r.range.y.ToString();
        }
        if (r.noiseType == "perlin")
        {
          
          randomStrings[0] = r.seed.ToString();
          randomStrings[1] = r.scale.ToString();
          randomStrings[2] = r.speed.ToString();
          randomStrings[3] = r.minimum.ToString();
        }
        EngineEventController e = (EngineEventController)control;
        ThrottleController t = (ThrottleController)control;

          throttleStrings[0] = t.responseRateUp.ToString();
          throttleStrings[1] = t.responseRateDown.ToString();
        
        eventCurve = e.eventCurve;
        
      }
      catch( Exception)
      { }
      GenerateCurveThumbs();
    }
    public void SetAddMode(ModuleWaterfallFX mod)
    {
      showWindow = true;
      fxMod = mod;
      windowMode = ControllerPopupMode.Add;
      controllerFlag = 0;
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2f, 400, 400);
      eventCurve = new FloatCurve();
      eventCurve.Add(0f,0f);
      eventCurve.Add(0.1f, 1f);
      eventCurve.Add(1f, 0f);
      GenerateCurveThumbs();
    }

    protected override void InitUI()
    {
      if (windowMode == ControllerPopupMode.Add)
        windowTitle = "Add new Controller";
      if (windowMode == ControllerPopupMode.Delete)
        windowTitle = "Confirm Delete";
      if (windowMode == ControllerPopupMode.Modify)
        windowTitle = "Edit Controller";
      base.InitUI();


    }
    protected void UpdateEventCurve(FloatCurve curve)
    {
      eventCurve = curve;

      GenerateCurveThumbs();
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      if (windowMode == ControllerPopupMode.Add)
        DrawAdd();
      if (windowMode == ControllerPopupMode.Delete)
        DrawDelete();
      if (windowMode == ControllerPopupMode.Modify)
        DrawModify();
      GUI.DragWindow();

    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f), GUILayout.MinWidth(350f));

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

    protected void DrawDelete()
    {
      GUILayout.Label($"Are you sure you want to delete {control.name}?");
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Yes"))
      {
        fxMod.RemoveController(control);
        showWindow = false;
      }
      if (GUILayout.Button("No"))
      {
        showWindow = false;
      }
      GUILayout.EndHorizontal();
    }
    protected void DrawAdd()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller name");
      newControllerName = GUILayout.TextArea(newControllerName);
      GUILayout.EndHorizontal();
      GUILayout.Label("Controller type");
      int controllerFlagChanged = GUILayout.SelectionGrid(controllerFlag, controllerTypes, Mathf.Min(controllerTypes.Length, 4), GUIResources.GetStyle("radio_text_button"));

      if (controllerFlagChanged != controllerFlag)
      {
        controllerFlag = controllerFlagChanged;

      }

      DrawControllerOptions();

      if (GUILayout.Button("Add"))
      {

        fxMod.AddController(CreateNewController());
        showWindow = false;
      }
      if (GUILayout.Button("Cancel"))
      {
        showWindow = false;
      }
    }
    protected void DrawModify()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller name");
      newControllerName = GUILayout.TextArea(newControllerName);
      GUILayout.EndHorizontal();
      GUILayout.Label("Controller type");
      int controllerFlagChanged = GUILayout.SelectionGrid(controllerFlag, controllerTypes, Mathf.Min(controllerTypes.Length, 4), GUIResources.GetStyle("radio_text_button"));

      if (controllerFlagChanged != controllerFlag)
      {
        controllerFlag = controllerFlagChanged;

      }

      DrawControllerOptions();

      if (GUILayout.Button("Apply"))
      {
        fxMod.RemoveController(control);
        fxMod.AddController(CreateNewController());
        showWindow = false;
      }
      if (GUILayout.Button("Cancel"))
      {
        showWindow = false;
      }
    }
    WaterfallController CreateNewController()
    {
      if (controllerTypes[controllerFlag] == "atmosphere_density")
      {
        AtmosphereDensityController newCtrl = new AtmosphereDensityController();
        newCtrl.name = newControllerName;
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "custom")
      {
        CustomController newCtrl = new CustomController();
        newCtrl.name = newControllerName;
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "engineEvent")
      {
        EngineEventController newCtrl = new EngineEventController();
        newCtrl.name = newControllerName;
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        newCtrl.eventName = eventTypes[eventFlag];
        newCtrl.eventCurve = eventCurve;
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "gimbal")
      {
        GimbalController newCtrl = new GimbalController();
        newCtrl.name = newControllerName;
        newCtrl.axis = axisTypes[axisFlag];
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "light")
      {
        LightController newCtrl = new LightController();
        newCtrl.name = newControllerName;
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "mach")
      {
        MachController newCtrl = new MachController();
        newCtrl.name = newControllerName;
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "random")
      {
        RandomnessController newCtrl = new RandomnessController();
        newCtrl.name = newControllerName;
        newCtrl.range = randomRange;
        newCtrl.scale = perlinScale;
        newCtrl.minimum = perlinMin;
        newCtrl.seed = perlinSeed;
        newCtrl.speed = perlinSpeed;
        newCtrl.noiseType = randTypes[randFlag];
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "rcs")
      {
        RCSController newCtrl = new RCSController();
        newCtrl.name = newControllerName;
        newCtrl.linkedTo = controllerTypes[controllerFlag];
        return newCtrl;
      }
      else if (controllerTypes[controllerFlag] == "throttle")
      {
        ThrottleController newCtrl = new ThrottleController();
        newCtrl.name = newControllerName;
        newCtrl.linkedTo = controllerTypes[controllerFlag];

        newCtrl.responseRateUp = rampRateUp;
        newCtrl.responseRateDown = rampRateDown;
        return newCtrl;
      }
      else
      {
        return null;
      }
    }

    public void DrawControllerOptions()
    {
      if (controllerTypes[controllerFlag] == "atmosphere_density")
      {
        // no special config
      }
      else if (controllerTypes[controllerFlag] == "custom")
      {
        // no special config
      }
      else if (controllerTypes[controllerFlag] == "engineEvent")
      {

        GUILayout.Label("Event name");
        int eventFlagChanged = GUILayout.SelectionGrid(eventFlag, eventTypes, Mathf.Min(eventTypes.Length, 4), GUIResources.GetStyle("radio_text_button"));

        if (eventFlagChanged != eventFlag)
        {
          eventFlag = eventFlagChanged;

        }
        Rect buttonRect = GUILayoutUtility.GetRect(curveButtonDims.x, curveButtonDims.y);
        Rect imageRect = new Rect(buttonRect.xMin + 10f, buttonRect.yMin + 10, buttonRect.width - 20, buttonRect.height - 20);
        if (GUI.Button(buttonRect, ""))
        {

          EditCurve(eventCurve, eventFun);
        }
        GUI.DrawTexture(imageRect, miniCurve);

      }
      else if (controllerTypes[controllerFlag] == "gimbal")
      {
        GUILayout.Label("Gimbal axis");
        int axisFlagChanged = GUILayout.SelectionGrid(axisFlag, axisTypes, Mathf.Min(axisTypes.Length, 4), GUIResources.GetStyle("radio_text_button"));

        if (axisFlagChanged != axisFlag)
        {
          axisFlag = axisFlagChanged;

        }
      }
      else if (controllerTypes[controllerFlag] == "light")
      {

      }
      else if (controllerTypes[controllerFlag] == "mach")
      {
        // no special config
      }
      else if (controllerTypes[controllerFlag] == "random")
      {
        GUILayout.Label("Random type");
        int randFlagChanged = GUILayout.SelectionGrid(randFlag, randTypes, Mathf.Min(randTypes.Length, 4), GUIResources.GetStyle("radio_text_button"));

        if (randFlagChanged != randFlag)
        {
          randFlag = randFlagChanged;

        }
        if (randTypes[randFlag] == "random")
        {
          GUILayout.BeginHorizontal();
          GUILayout.Label("Min/Max", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));

          randomStrings[0] = GUILayout.TextArea(randomStrings[0], GUILayout.MaxWidth(60f));
          randomStrings[1] = GUILayout.TextArea(randomStrings[1], GUILayout.MaxWidth(60f));

          float xParsed;
          float yParsed;
          Vector2 newRand = new Vector2(randomRange.x, randomRange.y);
          if (float.TryParse(randomStrings[0], out xParsed))
          {
            if (xParsed != randomRange.x)
            {
              newRand.x = xParsed;
            }
          }
          if (float.TryParse(randomStrings[1], out yParsed))
          {
            if (yParsed != randomRange.y)
            {
              newRand.y = yParsed;
            }
          }
          if (newRand.x != randomRange.x || newRand.y != randomRange.y)
          {
            randomRange = new Vector2(xParsed, yParsed);
          }
          GUILayout.EndHorizontal();
        }
        if (randTypes[randFlag] == "perlin")
        {
          GUILayout.BeginHorizontal();
          GUILayout.Label("Seed", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
          randomStrings[0] = GUILayout.TextArea(randomStrings[0], GUILayout.MaxWidth(60f));
          int intParsed;
          if (int.TryParse(randomStrings[0], out intParsed))
          {
            if (perlinSeed != intParsed)
            {
              perlinSeed = intParsed;
            }
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          GUILayout.Label("Minimum", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
          randomStrings[3] = GUILayout.TextArea(randomStrings[3], GUILayout.MaxWidth(60f));
          float floatParsed;
          if (float.TryParse(randomStrings[3], out floatParsed))
          {
            if (perlinMin != floatParsed)
            {
              perlinMin = floatParsed;
            }
          }
          GUILayout.EndHorizontal();

          GUILayout.BeginHorizontal();
          GUILayout.Label("Maximum", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
          randomStrings[1] = GUILayout.TextArea(randomStrings[1], GUILayout.MaxWidth(60f));
          if (float.TryParse(randomStrings[1], out floatParsed))
          {
            if (perlinScale != floatParsed)
            {
              perlinScale = floatParsed;
            }
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          GUILayout.Label("Speed", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
          randomStrings[2] = GUILayout.TextArea(randomStrings[2], GUILayout.MaxWidth(60f));
          
          if (float.TryParse(randomStrings[2], out floatParsed))
          {
            if (perlinSpeed != floatParsed)
            {
              perlinSpeed = floatParsed;
            }
          }
          GUILayout.EndHorizontal();
        }
      }
      else if (controllerTypes[controllerFlag] == "rcs")
      {
        // no special config
      }
      else if (controllerTypes[controllerFlag] == "throttle")
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Ramp Rate Up", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        throttleStrings[0] = GUILayout.TextArea(throttleStrings[0], GUILayout.MaxWidth(60f));
        float floatParsed;
        if (float.TryParse(throttleStrings[0], out floatParsed))
        {
          if (rampRateUp != floatParsed)
          {
            rampRateUp = floatParsed;
          }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Ramp Rate Down", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
        throttleStrings[1] = GUILayout.TextArea(throttleStrings[1], GUILayout.MaxWidth(60f));
        if (float.TryParse(throttleStrings[1], out floatParsed))
        {
          if (rampRateDown != floatParsed)
          {
            rampRateDown = floatParsed;
          }
        }
        GUILayout.EndHorizontal();
      }
    }
    protected void GenerateCurveThumbs()
    {
      miniCurve = GraphUtils.GenerateCurveTexture(texWidth, texHeight, eventCurve, Color.green);
    }
    protected void EditCurve(FloatCurve toEdit, CurveUpdateFunction function)
    {
      Utils.Log($"Started editing curve {toEdit.Curve.ToString()}", LogType.UI);
      curveEditor = WaterfallUI.Instance.OpenCurveEditor(toEdit, function);
    }
  }
}
