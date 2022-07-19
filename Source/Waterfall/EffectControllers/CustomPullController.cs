using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  ///   A controller that pulls specified value from engines module.
  ///   Output is normalized to 0..1 range and smoothing is applied when <see cref="responseRateUp" /> or <see cref="responseRateDown" /> not zero.
  ///   This is pull based alternative to <see cref="CustomPushController" /> which is push based.
  /// </summary>
  /// <example>
  ///   For example, effects from air-breathing engines in AJE mod will be able to take nozzleArea introduced by this mod into account and scale appropriately.
  /// </example>
  [Serializable]
  [DisplayName("Custom (Pull)")]
  public class CustomPullController : WaterfallController
  {
    [Persistent] public string engineID   = String.Empty;
    [Persistent] public string memberName = "currentThrottle"; // There is ThrottleController for that, but works as an example

    [Persistent] public float minInputValue;
    [Persistent] public float maxInputValue = 1;

    [Persistent] public float responseRateUp;
    [Persistent] public float responseRateDown;
    private float currentValue;

    private ModuleEngines engineController;
    private Func<float>   pullValueMethod = () => 0;

    public CustomPullController() : base() { }

    public CustomPullController(ConfigNode node) : base(node) { }

    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      engineController = host.GetComponents<ModuleEngines>().FirstOrDefault(x => x.engineID == engineID);
      if (engineController == null)
      {
        Utils.Log($"[{nameof(CustomPullController)}]: Could not find engine ID {engineID}, using first module if available");
        engineController = host.part.FindModuleImplementing<ModuleEngines>();
      }

      if (engineController == null)
      {
        Utils.LogError($"[{nameof(CustomPullController)}]: Could not find any {nameof(ModuleEngines)} to use with {nameof(CustomPullController)} named {name}, effect controller will not do anything");
        return;
      }

      pullValueMethod = FindSuitableMemberOnEnginesModule();
    }

    private Func<float> FindSuitableMemberOnEnginesModule()
    {
      const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

      var methodInfo = engineController.GetType()
        .GetMethods(bindingFlags)
        .FirstOrDefault(m => m.Name == memberName && m.GetParameters().Length == 0);

      if (methodInfo != null)
        return () => Convert.ToSingle(methodInfo.Invoke(engineController, new object[0]));

      var propertyInfo = engineController.GetType()
        .GetProperty(memberName, bindingFlags);

      if (propertyInfo != null)
        return () => (float)propertyInfo.GetValue(engineController);

      var fieldInfo = engineController.GetType()
        .GetField(memberName, bindingFlags);

      if (fieldInfo != null)
        return () => Convert.ToSingle(fieldInfo.GetValue(engineController));

      Utils.LogError($"[{nameof(CustomPullController)}]: Could not find any public instance method, property or field named {memberName} to use with {nameof(CustomPullController)} named {name}, effect controller will not do anything");
      return () => 0;
    }

    public override void Update()
    {
      if (!overridden)
      {
        float newValue = Mathf.InverseLerp(minInputValue, maxInputValue, GetValue());
        float responseRate = newValue > currentValue
          ? responseRateUp
          : responseRateDown;

        currentValue = responseRate > 0
          ? Mathf.MoveTowards(currentValue, newValue, responseRate * TimeWarp.deltaTime)
          : newValue;
      }
      value = currentValue;
    }

    private float GetValue()
    {
      if (engineController == null)
        return 0;

      if (!engineController.isOperational)
        return 0;

      engineID = engineController.engineID; // Make sure that engineID is in-sync with actually used module

      try
      {
        return pullValueMethod.Invoke();
      }
      catch (Exception ex)
      {
        Utils.LogError($"[{nameof(CustomPullController)}]: Error while getting value of specified member {memberName}: {ex.Message}");
        return 0;
      }
    }

    public override void UpgradeToCurrentVersion(Version loadedVersion)
    {
      base.UpgradeToCurrentVersion(loadedVersion);

      if (loadedVersion < Version.FixedRampRates)
      {
        responseRateDown *= Math.Max(1, referencingModifierCount);
        responseRateUp *= Math.Max(1, referencingModifierCount);
      }
    }
  }
}
