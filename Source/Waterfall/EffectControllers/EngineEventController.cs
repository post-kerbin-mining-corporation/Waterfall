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
    public string eventName;

    public  FloatCurve    eventCurve    = new();
    public  float         eventDuration = 1f;
    private ModuleEngines engineController;

    private bool  enginePreState;
    private bool  eventPlaying;
    private bool  eventReady;
    private float eventTime;

    public EngineEventController() : base() { }
    public EngineEventController(ConfigNode node) : base(node)
    {
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

      engineController = host.GetComponents<ModuleEngines>().FirstOrDefault(x => x.engineID == host.engineID);
      if (engineController == null)
        engineController = host.part.FindModuleImplementing<ModuleEngines>();

      if (engineController == null)
        Utils.LogError("[EngineEventController] Could not find engine controller on Initialize");

      if (eventName == "flameout")
      {
        eventReady = enginePreState = engineController.EngineIgnited;
      }
      else if (eventName == "ignition")
      {
        eventReady = enginePreState = !engineController.EngineIgnited;
      }
    }

    public override void Update()
    {
      if (engineController == null)
        Utils.LogWarning("[EngineEventController] Engine controller not assigned");

      value = 0;
      if (engineController != null && (eventName == "flameout" || eventName == "ignition"))
        value = eventCurve.Evaluate(CheckStateChange());
      //Utils.Log($"{eventName} =>_ Ready: {eventReady}, prestate {enginePreState}, time {eventTime}, playing {eventPlaying}");
    }

    public float CheckStateChange()
    {
      if (eventReady)
      {
        /// Check if engine state flipped
        if (engineController.EngineIgnited != enginePreState)
        {
          Utils.Log($"[EngineEventController] {eventName} fired", LogType.Modifiers);
          eventReady   = false;
          eventPlaying = true;
          eventTime    = 0f;
        }
      }
      else if (eventPlaying)
      {
        eventTime += TimeWarp.deltaTime;
        eventPlaying = eventTime <= eventDuration;
        return eventTime;
      }
      else if (!eventPlaying && !eventReady)
      {
        // Check to see if event can be reset
        if (engineController.EngineIgnited == enginePreState)
        {
          Utils.Log($"[EngineEventController] {eventName} ready", LogType.Modifiers);
          eventReady = true;
        }
      }

      return 0f;
    }
  }
}