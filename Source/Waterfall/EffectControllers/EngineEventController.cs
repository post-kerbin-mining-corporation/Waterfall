using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Waterfall
{
  /// <summary>
  /// </summary>
  [DisplayName("Engine Event")]
  public class EngineEventController : WaterfallController
  {
    public float  currentThrottle = 1;
    public string eventName;


    public  FloatCurve    eventCurve    = new();
    public  float         eventDuration = 1f;
    private ModuleEngines engineController;

    private bool  enginePreState;
    private bool  eventPlaying;
    private bool  eventReady;
    private float eventTime;

    public EngineEventController() { }

    public EngineEventController(ConfigNode node)
    {
      node.TryGetValue(nameof(name),          ref name);
      node.TryGetValue(nameof(eventName),     ref eventName);
      node.TryGetValue(nameof(eventDuration), ref eventDuration);

      eventCurve.Load(node.GetNode(nameof(eventCurve)));
    }

    public override ConfigNode Save()
    {
      var c = base.Save();
      c.AddValue(nameof(eventDuration), eventDuration);
      c.AddValue(nameof(eventName),     eventName);
      c.AddNode(Utils.SerializeFloatCurve(nameof(eventCurve), eventCurve));
      return c;
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      engineController = host.GetComponents<ModuleEngines>().ToList().Find(x => x.engineID == host.engineID);
      if (engineController == null)
        engineController = host.GetComponent<ModuleEngines>();

      if (engineController == null)
        Utils.LogError("[EngineEventController] Could not find engine controller on Initialize");

      if (eventName == "flameout")
      {
        eventReady = engineController.EngineIgnited;
        if (engineController.EngineIgnited)
          enginePreState = true;
      }

      if (eventName == "ignition")
      {
        eventReady = !engineController.EngineIgnited;
        if (engineController.EngineIgnited)
        {
          enginePreState = false;
        }
      }
    }

    public override List<float> Get()
    {
      if (overridden)
        return new() { overrideValue };

      if (engineController == null)
      {
        Utils.LogWarning("[EngineEventController] Engine controller not assigned");
        return new() { 0f };
      }

      //Utils.Log($"{eventName} =>_ Ready: {eventReady}, prestate {enginePreState}, time {eventTime}, playing {eventPlaying}");
      if (eventName == "flameout")
      {
        return new() { eventCurve.Evaluate(CheckStateChange()) };
      }

      if (eventName == "ignition")
      {
        return new() { eventCurve.Evaluate(CheckStateChange()) };
      }

      return new() { 0f };
    }

    public float CheckStateChange()
    {
      if (eventReady)
      {
        /// Check if engine state flipped
        if (engineController.EngineIgnited != enginePreState)
        {
          Utils.Log($"[EngineEventController] {eventName} fired ", LogType.Modifiers);
          eventReady   = false;
          eventPlaying = true;
          eventTime    = 0f;
        }
      }
      else if (eventPlaying)
      {
        eventTime += TimeWarp.deltaTime;
        if (eventTime > eventDuration)
        {
          eventPlaying = false;
        }

        return eventTime;
      }
      else if (!eventPlaying && !eventReady)
      {
        // Check to see if event can be reset
        if (engineController.EngineIgnited == enginePreState)
        {
          Utils.Log($"[EngineEventController] {eventName} ready ", LogType.Modifiers);
          eventReady = true;
        }
      }

      return 0f;
    }
  }
}