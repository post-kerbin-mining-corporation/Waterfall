using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waterfall
{
  public static class ParticleUtils
  {

  }

  public enum ParticleParameterType
  {
    Value,
    Range
  }
  public class ParticleParameterData
  {
    /// <summary>
    /// The parameter name
    /// </summary>
    public string Name;

    /// <summary>
    /// The type,
    /// </summary>
    public ParticleParameterType ParamType;

    public ParticleParameterData(string nm, ParticleParameterType tp)
    {

    }
  }
}
