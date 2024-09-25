using System;
using System.Collections.Generic;

namespace Waterfall.Effects
{
  public struct MultiFloatKey : IComparable<MultiFloatKey>
  {
    public float time;
    public FloatCurve value;
    public MultiFloatKey(float time, FloatCurve value)
    {
      this.time = time;
      this.value = value;
    }
    public int CompareTo(MultiFloatKey other)
    {
      return time.CompareTo(other.time);
    }
  }
  public class MultiFloatCurve
  {
    public List<MultiFloatKey> Keys { get; private set; }

    public MultiFloatCurve() 
    {
      Keys = new();
    }
    public MultiFloatCurve(ConfigNode node) { Load(node); }
    public void Load(ConfigNode node)
    {
      Keys = new();
      foreach (ConfigNode keyNode in node.GetNodes("KEY") )
      {
        float time = 0f;
        if (keyNode.TryGetValue("time", ref time))
        {
          FloatCurve curve = new();
          curve.Load(node.GetNode("curve"));
          MultiFloatKey key = new(time, curve);
          Keys.Add(key);
        }        
      }
    }
    public ConfigNode Save()
    {
      ConfigNode node = new();
      for (int i = 0; i < Keys.Count; i++)
      {
        ConfigNode keyNode = new("KEY");
        ConfigNode curveNode = Utils.SerializeFloatCurve("curve", Keys[i].value);

        keyNode.AddValue("time", Keys[i].time);
        keyNode.AddNode(curveNode);
        node.AddNode(keyNode);
      }
      return node;
    }
    public void AddKey(float time, FloatCurve value)
    {
      Keys.Add(new MultiFloatKey(time, value));
      Keys.Sort();
    }
    public void Interpolate(float time)
    {

    }


  }
}
