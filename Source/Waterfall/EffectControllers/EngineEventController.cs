using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

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

    private Func<ModuleEngines, bool> getEngineStateFunc; // when the result of this function transitions from false -> true, the event should fire
    private bool  eventPlaying;
    private bool  eventReady;     // the event can fire on the next transition
    private float eventTime;

    private static readonly Dictionary<string, Func<ModuleEngines, bool>> EngineStateFuncs = new()
    {
      { "flameout", (engineModule) => engineModule.flameout || !engineModule.EngineIgnited},
      { "ignition", (engineModule) => engineModule.EngineIgnited},
    };

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
        Utils.LogError($"[EngineEventController] Could not find engine module for waterfall moduleID '{host.moduleID}' engine '{host.engineID}' in part '{host.part.name}' on Initialize");

      EngineStateFuncs.TryGetValue(eventName, out getEngineStateFunc);

      if (getEngineStateFunc != null)
      {
        eventReady = !getEngineStateFunc(engineModule);
      }
      else
      {
        Utils.LogError($"[EngineEventController] Invalid engine eventName '{eventName}' in waterfall moduleID '{host.moduleID}' engine '{host.engineID}' in part '{host.part.name}'");
      }
    }

    public override void Update()
    {
      value = 0;
      if (engineModule != null && getEngineStateFunc != null)
        value = eventCurve.Evaluate(CheckStateChange());
      //Utils.Log($"{eventName} =>_ Ready: {eventReady}, prestate {enginePreState}, time {eventTime}, playing {eventPlaying}");
    }

    public float CheckStateChange()
    {
      if (eventReady)
      {
        /// Check if engine state flipped
        if (getEngineStateFunc(engineModule))
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
        if (!getEngineStateFunc(engineModule))
        {
          Utils.Log($"[EngineEventController] {eventName} ready", LogType.Modifiers);
          eventReady = true;
        }
      }

      return 0f;
    }

    public override void UpgradeToCurrentVersion(Version loadedVersion)
    {
      base.UpgradeToCurrentVersion(loadedVersion);

      if (loadedVersion < Version.FixedRampRates)
      {
        float scaleFactor = 1.0f / Math.Max(1, referencingModifierCount);

        eventDuration *= scaleFactor;

        var keys = eventCurve.Curve.keys;
        
        for (int i = 0; i < keys.Length; ++i)
        {
          Keyframe key = keys[i];

          key.time *= scaleFactor;
          key.inTangent /= scaleFactor;
          key.outTangent /= scaleFactor;

          keys[i] = key;
        }

        eventCurve = new FloatCurve(keys);
      }
    }
  }
}
