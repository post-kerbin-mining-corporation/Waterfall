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
    [Persistent] public string eventName;

    public FloatCurve eventCurve = new();
    [Persistent] public float eventDuration = 1f;
    private ModuleEngines engineModule;

    private bool  enginePreState;
    private bool  eventPlaying;
    private bool  eventReady;
    private float eventTime;

    public EngineEventController() : base() { }
    public EngineEventController(ConfigNode node) : base(node)
    {
      eventCurve.Load(node.GetNode(nameof(eventCurve)));
    }

    public override ConfigNode Save()
    {
      var c = base.Save();
      c.AddNode(Utils.SerializeFloatCurve(nameof(eventCurve), eventCurve));
      return c;
    }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      engineModule = host.GetComponents<ModuleEngines>().FirstOrDefault(x => x.engineID == host.engineID);
      if (engineModule == null)
        engineModule = host.part.FindModuleImplementing<ModuleEngines>();

      if (engineModule == null)
        Utils.LogError("[EngineEventController] Could not find engine controller on Initialize");

      enginePreState = engineModule.EngineIgnited;

      if (eventName == "flameout")
      {
        eventReady = enginePreState;
      }
      else if (eventName == "ignition")
      {
        eventReady = !enginePreState;
      }
    }

    public override void Update()
    {
      if (engineModule == null)
        Utils.LogWarning("[EngineEventController] Engine controller not assigned");

      value = 0;
      if (engineModule != null && (eventName == "flameout" || eventName == "ignition"))
        value = eventCurve.Evaluate(CheckStateChange());
      //Utils.Log($"{eventName} =>_ Ready: {eventReady}, prestate {enginePreState}, time {eventTime}, playing {eventPlaying}");
    }

    public float CheckStateChange()
    {
      if (eventReady)
      {
        /// Check if engine state flipped
        if (engineModule.EngineIgnited != enginePreState)
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
        if (engineModule.EngineIgnited == enginePreState)
        {
          Utils.Log($"[EngineEventController] {eventName} ready", LogType.Modifiers);
          eventReady = true;
        }
      }

      return 0f;
    }
  }
}
