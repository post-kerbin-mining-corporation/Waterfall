using UnityEngine;

namespace Waterfall
{
  public class SerializedData : ScriptableObject
  {
    public string SerializedString;

    public ConfigNode SerializedNode = null;
  }
}