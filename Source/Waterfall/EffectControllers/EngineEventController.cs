using System;
using System.Collections.Generic;
using System.Linq;

namespace Waterfall
{
  /// <summary>
  /// </summary>
  public class EngineEventController : WaterfallController
  {
    public const string ControllerTypeId = "engineEvent";
    public const string DisplayName = "Engine Event";

    public float currentThrottle = 1;
    ModuleEngines engineController;
    public string eventName;


    public FloatCurve eventCurve = new FloatCurve();

    bool enginePreState = false;
    bool eventPlaying = false;
    bool eventReady = false;
    float eventTime = 0f;
    public float eventDuration = 1f;

    public EngineEventController()
    {
      name = ControllerTypeId;
      linkedTo = ControllerTypeId;
    }

    public EngineEventController(ConfigNode node) : this()
    {
      node.TryGetValue(nameof(name), ref name);
      node.TryGetValue(nameof(eventName), ref eventName);
      node.TryGetValue(nameof(eventDuration), ref eventDuration);

      eventCurve.Load(node.GetNode(nameof(eventCurve)));
    }

    public override ConfigNode Save()
    {
      ConfigNode c = base.Save();
      c.AddValue(nameof(eventDuration), eventDuration);
      c.AddValue(nameof(eventName), eventName);
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
        return new List<float>() { overrideValue };

      if (engineController == null)
      {
        Utils.LogWarning("[EngineEventController] Engine controller not assigned");
        return new List<float>() { 0f };
      }

      //Utils.Log($"{eventName} =>_ Ready: {eventReady}, prestate {enginePreState}, time {eventTime}, playing {eventPlaying}");
      if (eventName == "flameout")
      {
        return new List<float>() { eventCurve.Evaluate(CheckStateChange()) };
      }

      if (eventName == "ignition")
      {
        return new List<float>() { eventCurve.Evaluate(CheckStateChange()) };
      }

      return new List<float>() { 0f };
    }

    public float CheckStateChange()
    {
      if (eventReady)
      {
        /// Check if engine state flipped
        if (engineController.EngineIgnited != enginePreState)
        {
          Utils.Log($"[EngineEventController] {eventName} fired ", LogType.Modifiers);
          eventReady = false;
          eventPlaying = true;
          eventTime = 0f;
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