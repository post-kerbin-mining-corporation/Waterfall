using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;


/// <summary>
/// A collection of keys mapping times to values, interpolated between keys by a cubic Hermite spline. <para/>
/// The implementation produce identical results as a UnityEngine.AnimationCurve, but calling Evaluate() is at least twice faster. <para/>
/// However, it doesn't support keys in/out weights, and behavior is always identical to WrapMode.ClampForever. 
/// </summary>
public class FastFloatCurve : IEnumerable<FastFloatCurve.Key>, IConfigNode
{
    /// <summary>
    /// Define how a key tangent will be adjusted.
    /// </summary>
    public enum AutoTangentMode
    {
        /// <summary>Don't adjust the key tangents.</summary>
        Ignore = 0,
        /// <summary>The curve will be flat before and after this key, ignoring the adjacent keys tangents.</summary>
        Step = 1,
        /// <summary>The curve will have flat transition at this key.</summary>
        Flat = 2,
        /// <summary>The curve will point toward the adjacent keys, with a hard angle instead of a smooth transition.</summary>
        Straight = 3,
        /// <summary>The curve will have a smooth transition with the adjacent keys.</summary>
        Smooth = 4,
        /// <summary>The curve will have a smooth transition with the adjacent keys, but will never overshoot the adjacent keys values</summary>
        SmoothClamped = 5
    }

    /// <summary>
    /// A key defining a point and its tangents on a FastFloatCurve
    /// </summary>
    public struct Key : IComparable<Key>
    {
        /// <summary>The time of the key.</summary>
        public float time;

        /// <summary>The value of the curve at the key time.</summary>
        public float value;

        /// <summary>The incoming tangent affects the slope of the curve from the previous key to this key.</summary>
        /// <remarks>This can be set to float.Infinity to have last key value being constant up to the key.</remarks>
        public float inTangent;

        /// <summary>The outgoing tangent affects the slope of the curve from this key to the next key.</summary>
        /// /// <remarks>This can be set to float.Infinity to get a constant value from this key to the next one</remarks>
        public float outTangent;

        /// <summary>Define how a key tangent will be adjusted.</summary>
        public AutoTangentMode tangentMode;

        public Key(float time, float value, float inTangent, float outTangent)
        {
            this.time = time;
            this.value = value;
            this.inTangent = inTangent;
            this.outTangent = outTangent;
            tangentMode = AutoTangentMode.Ignore;
        }

        public Key(float time, float value, AutoTangentMode tangentMode = AutoTangentMode.Ignore)
        {
            this.time = time;
            this.value = value;
            this.tangentMode = tangentMode;
            inTangent = 0f;
            outTangent = 0f;
        }

        public int CompareTo(Key other) => time.CompareTo(other.time);

        public override string ToString()
        {
            if (inTangent == 0f && outTangent == 0f)
                return $"{time} | {value}";

            return $"{time} | {value} | {inTangent} | {outTangent}";
        }
    }

    private struct Range
    {
        public double a, b, c, d;
        public float minTime;
    }

    private bool _autoTangents;
    private bool _isCompiled;

    private List<Key> _keys;

    private int _rangeCount;
    private int _lastRangeIdx;
    private Range[] _ranges;

    private float _firstTime;
    private float _lastTime;
    private float _firstValue;
    private float _lastValue;

    /// <summary> Create a new empty curve. </summary>
    public FastFloatCurve()
    {
        _keys = new List<Key>();
        _isCompiled = false;
    }

    /// <summary> Create a new curve from the provided keys. </summary>
    /// <param name="keys">The keys to add to the curve.</param>
    public FastFloatCurve(params Key[] keys)
    {
        int keyCount = keys.Length;
        _keys = new List<Key>(keyCount);
        for (int i = keyCount; i-- > 0;)
        {
            Key key = keys[i];
            bool isDuplicateTime = false;
            for (int j = keyCount; j-- > 0;)
            {
                if (i != j && key.time == keys[j].time)
                {
                    isDuplicateTime = true;
                    break;
                }
            }

            if (isDuplicateTime)
                continue;

            _keys.Add(key);
        }
        _isCompiled = false;
    }

    /// <summary> Create a new curve with the same keys as an UnityEngine.AnimationCurve. </summary>
    /// <param name="animationCurve">The unity AnimationCurve to copy keys from.</param>
    public FastFloatCurve(AnimationCurve animationCurve)
    {
        // note : an Unity AnimationCurve cannot have same-time keys, so no need to check again.
        Keyframe[] unityKeys = animationCurve.keys;
        int keyCount = unityKeys.Length;
        _keys = new List<Key>(keyCount);
        for (int i = 0; i < keyCount; i++)
        {
            Keyframe unityKey = unityKeys[i];
            _keys.Add(new Key(unityKey.time, unityKey.value, unityKey.inTangent, unityKey.outTangent));
        }
        _isCompiled = false;
    }

    /// <summary> Create a new curve with the same keys as a KSP FastFloatCurve. </summary>
    /// <param name="kspFastFloatCurve">The FastFloatCurve to copy keys from.</param>
    public FastFloatCurve(FastFloatCurve kspFastFloatCurve) : this(kspFastFloatCurve.fCurve) { }

    /// <summary> Create a copy of this curve instance. </summary>
    /// <returns>A newly instantiated clone of this curve.</returns>
    public FastFloatCurve Clone()
    {
        FastFloatCurve clone = new FastFloatCurve();
        clone._keys.AddRange(_keys);
        clone._autoTangents = _autoTangents;
        if (_isCompiled && _rangeCount > 0)
        {
            clone._isCompiled = true;
            clone._rangeCount = _rangeCount;
            clone._ranges = new Range[_rangeCount];
            Array.Copy(_ranges, clone._ranges, _rangeCount);
            clone._lastRangeIdx = _lastRangeIdx;
            clone._firstTime = _firstTime;
            clone._firstValue = _firstValue;
            clone._lastTime = _lastTime;
            clone._lastValue = _lastValue;
        }
        return clone;
    }

    /// <summary> Set all keys in this curve to the keys of another curve.</summary>
    /// <param name="other">The other curve to copy the key from</param>
    public void CopyFrom(FastFloatCurve other)
    {
        _keys.Clear();
        _keys.AddRange(other._keys);
        _autoTangents = other._autoTangents;
        if (other._isCompiled && other._rangeCount > 0)
        {
            _isCompiled = true;
            _rangeCount = other._rangeCount;
            _ranges = new Range[_rangeCount];
            Array.Copy(other._ranges, _ranges, _rangeCount);
            _lastRangeIdx = other._lastRangeIdx;
            _firstTime = other._firstTime;
            _firstValue = other._firstValue;
            _lastTime = other._lastTime;
            _lastValue = other._lastValue;
        }
        else
        {
            _isCompiled = false;
        }
    }

    /// <summary>When true, the curve keys tangents will be automatically adjusted according to the tangent mode defined in each key.</summary>
    public bool AutoTangents
    {
        get => _autoTangents;
        set
        {
            if (!_autoTangents && value)
                _isCompiled = false;

            _autoTangents = value;
        }
    }

    /// <summary>Set the TangentMode on all keys.</summary>
    /// <param name="tangentMode"></param>
    public void SetKeysTangentMode(AutoTangentMode tangentMode)
    {
        for (int i = _keys.Count; i-- > 0;)
        {
            Key key = _keys[i];
            key.tangentMode = tangentMode;
            _keys[i] = key;
        }
        _isCompiled = false;
    }

    /// <summary> The amount of keys in the curve.</summary>
    public int KeyCount => _keys.Count;

    /// <summary> The time of the first key. </summary>
    public float FirstTime
    {
        get
        {
            if (!_isCompiled)
                Compile();

            switch (_keys.Count)
            {
                case 0: return 0f;
                case 1: return _keys[0].time;
                default: return _firstTime;
            }
        }
    }

    /// <summary> The time of the last key. </summary>
    public float LastTime
    {
        get
        {
            if (!_isCompiled)
                Compile();

            switch (_keys.Count)
            {
                case 0: return 0f;
                case 1: return _keys[0].time;
                default: return _lastTime;
            }
        }
    }

    /// <summary> The value of the first key</summary>
    public float FirstValue
    {
        get
        {
            if (!_isCompiled)
                Compile();

            switch (_keys.Count)
            {
                case 0: return 0f;
                case 1: return _keys[0].value;
                default: return _firstValue;
            }
        }
    }

    /// <summary> The value of the last key</summary>
    public float LastValue
    {
        get
        {
            if (!_isCompiled)
                Compile();

            switch (_keys.Count)
            {
                case 0: return 0f;
                case 1: return _keys[0].value;
                default: return _lastValue;
            }
        }
    }

    /// <summary> Get or set a key at the specified index</summary>
    /// <param name="keyIndex">The zero-based index of the key</param>
    /// <returns>The key at the specified index.</returns>
    public Key this[int keyIndex]
    {
        get
        {
            return _keys[keyIndex];
        }
        set
        {
            for (int i = _keys.Count; i-- > 0;)
                if (i != keyIndex && _keys[i].time == value.time)
                    return;

            _keys[keyIndex] = value;
            _isCompiled = false;
        }
    }

    /// <summary> Add a new key to the curve.</summary>
    /// <param name="key">The key to add to the curve.</param>
    /// <returns>true if the key was added, false if a key with the same time already exists.</returns>
    public bool Add(Key key)
    {
        if (HasKeyWithTime(key.time))
            return false;

        _keys.Add(key);
        _isCompiled = false;
        return true;
    }

    /// <summary> Add a new key to the curve, with flat tangents.</summary>
    /// <param name="time">The time of the key.</param>
    /// <param name="value">The value of the curve at the key time.</param>
    /// <param name="tangentMode">The tangent mode to use when automatically adjusting tangents.</param>
    /// <returns>true if the key was added, false if a key with the same time already exists.</returns>
    public bool Add(float time, float value, AutoTangentMode tangentMode = AutoTangentMode.Ignore)
    {
        if (HasKeyWithTime(time))
            return false;

        _keys.Add(new Key(time, value, tangentMode));
        _isCompiled = false;
        return true;
    }

    /// <summary> Add a new key to the curve.</summary>
    /// <param name="time">The time of the key.</param>
    /// <param name="value">The value of the curve at the key time.</param>
    /// <param name="inTangent">The incoming tangent affects the slope of the curve from the previous key to this key.</param>
    /// <param name="outTangent">The outgoing tangent affects the slope of the curve from this key to the next key.</param>
    /// <returns>true if the key was added, false if a key with the same time already exists.</returns>
    public bool Add(float time, float value, float inTangent, float outTangent)
    {
        if (HasKeyWithTime(time))
            return false;

        _keys.Add(new Key(time, value, inTangent, outTangent));
        _isCompiled = false;
        return true;
    }

    /// <summary> Remove a key from the curve</summary>
    /// <param name="keyIndex">The index of key to remove</param>
    public void RemoveKey(int keyIndex)
    {
        _keys.RemoveAt(keyIndex);
        _isCompiled = false;
    }

    private bool HasKeyWithTime(float time)
    {
        for (int i = _keys.Count; i-- > 0;)
            if (_keys[i].time == time)
                return true;

        return false;
    }

    /// <summary> Returns an enumerator that iterates through curve keys.</summary>
    public IEnumerator<Key> GetEnumerator() => _keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _keys.GetEnumerator();

    /// <summary> Evaluate the curve at the specified time.</summary>
    /// <param name="time">The time to evaluate (the horizontal axis in the curve graph).</param>
    /// <returns> The value of the curve at the specified time.</returns>
    public unsafe float Evaluate(float time)
    {
        if (!_isCompiled)
            Compile();

        if (time <= _firstTime)
            return _firstValue;
        if (time >= _lastTime)
            return _lastValue;

        int i = _rangeCount;
        fixed (Range* lastRangePtr = &_ranges[_lastRangeIdx]) // avoid struct copying and array bounds checks
        {
            Range* rangePtr = lastRangePtr;
            while (i > 0)
            {
                if (time > rangePtr->minTime)
                    return (float)(rangePtr->d + time * (rangePtr->c + time * (rangePtr->b + time * rangePtr->a)));

                rangePtr--;
                i--;
            }
        }
        return 0f;
    }

    /// <summary> Evaluate the curve at the specified time, allowing evaluation before the first key time and after the last key time.</summary>
    /// <param name="time">The time to evaluate (the horizontal axis in the curve graph).</param>
    /// <returns> The value of the curve at the specified time.</returns>
    public unsafe float EvaluateUnclamped(float time)
    {
        if (!_isCompiled)
            Compile();

        if (_firstTime == float.PositiveInfinity)
            return _firstValue;

        int i = _rangeCount;
        fixed (Range* lastRangePtr = &_ranges[_lastRangeIdx]) // avoid struct copying and array bounds checks
        {
            Range* rangePtr = lastRangePtr;
            while (true)
            {
                i--;
                if (time > rangePtr->minTime || i == 0)
                    return (float)(rangePtr->d + time * (rangePtr->c + time * (rangePtr->b + time * rangePtr->a)));

                rangePtr--;
            }
        }
    }

    /// <summary>
    /// Find the minimum and maximum values on the curve, and the corresponding times.
    /// </summary>
    /// <param name="minTime">The time for the minimum value.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxTime">The time for the maximum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    public unsafe void FindMinMax(out float minTime, out float minValue, out float maxTime, out float maxValue)
    {
        int keyCount = _keys.Count;
        if (keyCount == 0)
        {
            minTime = 0f;
            minValue = 0f;
            maxTime = 0f;
            maxValue = 0f;
            return;
        }

        if (keyCount == 1)
        {
            Key key = _keys[0];
            minTime = key.time;
            minValue = key.value;
            maxTime = key.time;
            maxValue = key.value;
            return;
        }

        if (!_isCompiled)
            Compile();

        minTime = float.MaxValue;
        minValue = float.MaxValue;
        maxTime = float.MinValue;
        maxValue = float.MinValue;

        float* times = stackalloc float[4];
        float* values = stackalloc float[4];

        for (int i = keyCount - 1; i-- > 0;)
        {
            Key k1 = _keys[i];
            Key k2 = _keys[i + 1];

            times[0] = k1.time;
            times[1] = k2.time;
            values[0] = k1.value;
            values[1] = k2.value;
            int valuesToCheck = 2;

            double p0 = k1.value;
            double p1 = k2.value;
            double m0 = k1.outTangent;
            double m1 = k2.inTangent;
            double t0 = k1.time;
            double t1 = k2.time;
            double tI = t1 - t0;

            if (tI > 0.0 && !double.IsInfinity(m0) && !double.IsInfinity(m1))
            {
                // The time of the 2 potential extremums are of the form :
                // r0 = (a + √b) / c
                // r1 = (a - √b) / c
                // Which are given by solving h'(t) = 0 for t where h'(t) is the 
                // derivative of the hermit function, see FindTangentBetweenKeysAtTime();

                double tI2 = tI * tI;
                double tI3 = tI2 * tI;
                double tI4 = tI2 * tI2;

                double sqrt = Math.Sqrt(
                      9.0 * p0 * p0 * tI2
                    + 6.0 * p0 * m0 * tI3
                    - 18.0 * p0 * p1 * tI2
                    + 6.0 * p0 * m1 * tI3
                    + m0 * m0 * tI4
                    - 6.0 * m0 * p1 * tI3
                    + m0 * m1 * tI4
                    + 9.0 * p1 * p1 * tI2
                    - 6.0 * p1 * m1 * tI3
                    + m1 * m1 * tI4);

                double factor =
                      6.0 * p0 * t0
                    + 3.0 * p0 * tI
                    + 3.0 * m0 * t0 * tI
                    + 2.0 * m0 * tI2
                    - 6.0 * p1 * t0
                    - 3.0 * p1 * tI
                    + 3.0 * m1 * t0 * tI
                    + m1 * tI2;

                double divisor = 3.0 * (2.0 * p0 + m0 * tI - 2.0 * p1 + m1 * tI);

                float time1 = (float)((factor + sqrt) / divisor);
                if (!double.IsNaN(time1) && time1 > t0 && time1 < t1)
                {
                    times[valuesToCheck] = time1;
                    values[valuesToCheck] = Evaluate(time1);
                    valuesToCheck++;
                }

                float time2 = (float)((factor - sqrt) / divisor);
                if (!double.IsNaN(time2) && time2 > t0 && time2 < t1)
                {
                    times[valuesToCheck] = time2;
                    values[valuesToCheck] = Evaluate(time2);
                    valuesToCheck++;
                }
            }

            while (valuesToCheck-- > 0)
            {
                float value = values[valuesToCheck];
                if (value < minValue)
                {
                    minValue = value;
                    minTime = times[valuesToCheck];
                }
                if (value > maxValue)
                {
                    maxValue = value;
                    maxTime = times[valuesToCheck];
                }
            }
        }
    }

    /// <summary> Find the slope of the tangent (the derivative) to the curve at the given time.</summary>
    /// <param name="time">The time to evaluate, must be within the min and max time defined by the curve.</param>
    /// <returns>The slope of the tangent (the derivative) at the given time</returns>
    public float FindTangent(float time)
    {
        int i = _keys.Count;
        if (i < 2)
            return 0f;

        if (!_isCompiled)
            Compile();

        if (time < _keys[0].time)
            return 0f;

        if (time > _keys[--i].time)
            return 0f;

        while (i-- > 0)
        {
            if (time < _keys[i].time)
                continue;

            return FindTangentBetweenKeysAtTime(time, _keys[i], _keys[i + 1]);
        }

        return 0f;
    }


    /// <summary> Find the slope of the tangent (the derivative) to a curve segment at the given time.</summary>
    /// <param name="time">The time to evaluate.</param>
    /// <param name="k0">The first key defining the curve segment.</param>
    /// <param name="k1">The second key defining the curve segment.</param>
    /// <returns>The slope of the tangent (the derivative) at the given time</returns>
    public static float FindTangentBetweenKeysAtTime(float time, Key k0, Key k1)
    {
        // Derivatives of the Hermite base functions :
        // h00'(t) = 2t² - 6t
        // h10'(t) = 3t² - 4t + 1
        // h01'(t) = -6t² + 6t
        // h11'(t) = 3t² - 2t
        // Derivative at time t on an arbitrary interval [t0, t1], for the values p0, p1 and tangents m0, m1 :
        //                      1                                     1                                (t - t0)
        // h'(t) = h00'(tI)·---------·p0 + h10'(tI)·m0 + h01'(tI)·---------·p1 + h11'(tI)·m1 with tI = ---------
        //                  (t1 - t0)                             (t1 - t0)                            (t1 - t0)

        //if (float.IsInfinity(k0.outTangent) || float.IsInfinity(k0.inTangent))

        double i = (double)k1.time - k0.time;
        double tI = (time - k0.time) / i;
        double tI2 = tI * tI;

        double h00 = 6.0 * tI2 - 6.0 * tI;
        double h10 = 3.0 * tI2 - 4.0 * tI + 1.0;
        double h01 = -6.0 * tI2 + 6.0 * tI;
        double h11 = 3.0 * tI2 - 2.0 * tI;

        double iD = 1.0 / i;

        return (float)(h00 * iD * k0.value + h10 * k0.outTangent + h01 * iD * k1.value + h11 * k1.inTangent);
    }

    /// <summary>Create a key at the specified time, where the key value and tangents are set such as to match the curve current shape.</summary>
    /// <param name="time">The time at which the key must be created.</param>
    /// <param name="tangentKey">The resulting key where the value and tangents match the curve current shape.</param>
    /// <returns>True if the key was succesfully created, false otherwise.</returns>
    public bool CreateTangentKey(float time, out Key tangentKey)
    {
        tangentKey = default;
        int i = _keys.Count;
        if (i < 2)
            return false;

        if (!_isCompiled)
            Compile();

        i--;
        while (i-- > 0)
        {
            if (i > 0 && time < _keys[i].time)
                continue;

            Key k0 = _keys[i];
            Key k1 = _keys[i + 1];

            tangentKey.time = time;
            tangentKey.value = Evaluate(time, k0, k1);
            float tangent = FindTangentBetweenKeysAtTime(time, k0, k1);
            tangentKey.inTangent = tangent;
            tangentKey.outTangent = tangent;

            return IsFinite(tangentKey.value) && IsFinite(tangent);
        }

        return false;
    }

    /// <summary> Interpolate a Hermite cubic spline segment at the given time.</summary>
    /// <param name="time">The time to evaluate, must be within the time range defined by the keys.</param>
    /// <param name="k0">The first key defining the curve segment.</param>
    /// <param name="k1">The second key defining the curve segment.</param>
    /// <returns>The interpolated value at the given time.</returns>
    /// <remarks>
    /// This produce identical results as the Evaluate() instance method, but for every range, the instance
    /// method compute and cache the polynomial form of the Hermite function, which is much faster to evaluate.
    /// </remarks>
    public static float Evaluate(float time, Key k0, Key k1)
    {
        // Hermite base functions :
        // h00(t) = 2t³ - 3t² + 1
        // h10(t) = t³ - 2t² + t
        // h01(t) = -2t³ + 3t²
        // h11(t) = t³ - t²
        // Interpolation at time t on an arbitrary interval [t0, t1], for the values p0, p1 and tangents m0, m1 :
        //                                                                                        (t - t0)
        // h(t) = h00(tI)·(t1 - t0)·p0 + h10(tI)·m0 + h01(tI)·(t1 - t0)·p1 + h11(tI)·m1 with tI = ---------
        //                                                                                        (t1 - t0)

        float m0 = k0.outTangent;
        float m1 = k1.inTangent;

        if (float.IsInfinity(m0) || float.IsInfinity(m1))
            return k0.value;

        double i = (double)k1.time - k0.time;
        double tI = (time - k0.time) / i;
        double tI2 = tI * tI;
        double tI3 = tI2 * tI;

        double h00 = 2.0 * tI3 - 3.0 * tI2 + 1.0;
        double h10 = tI3 - 2.0 * tI2 + tI;
        double h01 = -2.0 * tI3 + 3.0 * tI2;
        double h11 = tI3 - tI2;

        return (float)(h00 * k0.value + h10 * i * m0 + h01 * k1.value + h11 * i * m1);
    }

    /// <summary>Adjust all the curve keys tangents according to the tangent mode defined in each key.</summary>
    /// <remarks>This doesn't need to be called manually when AutoTangents is set to true.</remarks>
    public void AdjustTangents()
    {
        int keyCount = _keys.Count;

        if (keyCount < 2)
            return;

        if (!_isCompiled)
            _keys.Sort();
        else
            _isCompiled = false;

        Key key0 = _keys[0];
        if (key0.tangentMode == AutoTangentMode.Flat)
        {
            key0.outTangent = 0f;
        }
        else if (key0.tangentMode == AutoTangentMode.Step)
        {
            key0.outTangent = float.PositiveInfinity;
        }
        else if (key0.tangentMode != AutoTangentMode.Ignore)
        {
            Key key1 = _keys[1];
            float tgt1 = (key1.value - key0.value) / (key1.time - key0.time);
            float tangent;
            if (keyCount > 2 && key0.tangentMode != AutoTangentMode.Straight)
            {
                Key key2 = _keys[2];
                float tgt2 = (key2.value - key0.value) / (key2.time - key0.time);
                tangent = 2f * tgt1 - tgt2;

                if (key0.tangentMode == AutoTangentMode.SmoothClamped)
                {
                    if (tgt1 < 0f && tangent < tgt1)
                        tangent = tgt1;
                    else if (tangent < 0f)
                        tangent = 0f;
                }
            }
            else
            {
                tangent = tgt1;
            }

            key0.outTangent = tangent;
        }
        _keys[0] = key0;


        Key keyL = _keys[keyCount - 1];
        if (keyL.tangentMode == AutoTangentMode.Flat)
        {
            keyL.inTangent = 0f;
        }
        else if (keyL.tangentMode == AutoTangentMode.Step)
        {
            keyL.inTangent = float.PositiveInfinity;
        }
        else if (keyL.tangentMode != AutoTangentMode.Ignore)
        {
            Key keyL1 = _keys[keyCount - 2];
            float tgtL1 = (keyL.value - keyL1.value) / (keyL.time - keyL1.time);
            float tangent;
            if (keyCount > 2 && keyL.tangentMode != AutoTangentMode.Straight)
            {
                Key keyL2 = _keys[keyCount - 3];
                float tgtL2 = (keyL.value - keyL2.value) / (keyL.time - keyL2.time);
                tangent = 2f * tgtL1 - tgtL2;

                if (key0.tangentMode == AutoTangentMode.SmoothClamped)
                {
                    if (tgtL1 < 0f && tangent < tgtL1)
                        tangent = tgtL1;
                    else if (tangent < 0f)
                        tangent = 0f;
                }
            }
            else
            {
                tangent = tgtL1;
            }

            keyL.inTangent = tangent;
        }
        _keys[keyCount - 1] = keyL;

        for (int i = 1; i < keyCount - 1; i++)
        {
            Key key = _keys[i];

            if (key.tangentMode == AutoTangentMode.Ignore)
                continue;

            if (key.tangentMode == AutoTangentMode.Flat)
            {
                key.inTangent = 0f;
                key.outTangent = 0f;
            }
            else if (key.tangentMode == AutoTangentMode.Step)
            {
                key.inTangent = float.PositiveInfinity;
                key.outTangent = float.PositiveInfinity;
            }
            else
            {
                Key prevKey = _keys[i - 1];
                Key nextKey = _keys[i + 1];
                float inTangent = (key.value - prevKey.value) / (key.time - prevKey.time);
                float outTangent = (nextKey.value - key.value) / (nextKey.time - key.time);
                if (key.tangentMode == AutoTangentMode.Straight)
                {
                    key.inTangent = inTangent;
                    key.outTangent = outTangent;
                }
                else if (key.tangentMode == AutoTangentMode.Smooth)
                {
                    float tangent = (inTangent + outTangent) * 0.5f;
                    key.inTangent = tangent;
                    key.outTangent = tangent;
                }
                else if (key.tangentMode == AutoTangentMode.SmoothClamped)
                {
                    float minTangent = Mathf.Min(inTangent, outTangent);
                    float tangent;
                    if (minTangent < 0f)
                        tangent = 0f;
                    else
                        tangent = Mathf.Min(minTangent * 3f, (inTangent + outTangent) * 0.5f);

                    key.inTangent = tangent;
                    key.outTangent = tangent;
                }
            }

            _keys[i] = key;
        }
    }

    public override string ToString() => $"{_keys.Count} keys, range = [{FirstTime}, {LastTime}], values = [{FirstValue}, {LastValue}]";

    /// <summary> Set the keys of this curve from a serialized list of keys. Any existing keys will be overriden.</summary>
    /// <param name="node">A ConfigNode with a list of keys formatted as "<c>key = time value inTangent outTangent</c>". The tangent parameters are optional.</param>
    public void Load(ConfigNode node)
    {
        _isCompiled = false;
        _keys = new List<Key>(node.values.Count);

        for (int i = 0; i < node.values.Count; i++)
        {
            ConfigNode.Value nodeValue = node.values[i];
            if (nodeValue.name != "key")
                continue;

            string[] keyValues = nodeValue.value.Split(FastFloatCurve.delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (keyValues.Length < 2)
            {
                Debug.LogError($"Invalid FastFloatCurve key : \"{nodeValue.value}\"");
                continue;
            }

            if (keyValues.Length == 4)
                _keys.Add(new Key(float.Parse(keyValues[0]), float.Parse(keyValues[1]), float.Parse(keyValues[2]), float.Parse(keyValues[3])));
            else
                _keys.Add(new Key(float.Parse(keyValues[0]), float.Parse(keyValues[1])));
        }
    }

    /// <summary> Serialize this curve keys as values in the ConfigNode.</summary>
    /// <param name="node">The ConfigNode to add keys to.</param>
    public void Save(ConfigNode node)
    {
        for (int i = 0; i < _keys.Count; i++)
        {
            Key key = _keys[i];
            node.AddValue("key", $"{key.time} {key.value} {key.inTangent} {key.outTangent}");
        }
    }

    /// <summary>
    /// This will be called automatically when necessary, but can be called manually to be in control of when the associated performance cost is paid.
    /// </summary>
    /// <remarks>
    /// Sort the keys by time, adjust the tangents if requested and cache the polynomial form of the hermit curve for every key pair.
    /// Doesn't need to called again unless the keys are modified.
    /// </remarks>
    public void Compile()
    {
        _isCompiled = true;
        _keys.Sort();
        int keyCount = _keys.Count;

        if (_autoTangents)
            AdjustTangents();

        if (keyCount < 2)
        {
            _firstTime = float.PositiveInfinity;
            _firstValue = keyCount == 1 ? _keys[0].value : 0f;
            return;
        }

        _rangeCount = keyCount - 1;
        _lastRangeIdx = _rangeCount - 1;

        _ranges = new Range[_rangeCount];
        for (int i = 0; i < _rangeCount; i++)
            _ranges[i] = ComputeRangePolynomial(_keys[i], _keys[i + 1]);

        Key firstKey = _keys[0];
        _firstValue = firstKey.value;
        _firstTime = firstKey.time;

        Key lastKey = _keys[_rangeCount];
        _lastValue = lastKey.value;
        _lastTime = lastKey.time;
    }

    /// <summary> Compute the factors of the polynomial form of the hermit curve equation for the range between two keys.</summary>
    /// <remarks> The resulting factors are the expression of the hermit spline in the form ax³ + bx² + cx + d.</remarks>
    private static Range ComputeRangePolynomial(Key p1, Key p2)
    {
        double p1x = p1.time;
        double p1y = p1.value;
        double tp1 = p1.outTangent;
        double p2x = p2.time;
        double p2y = p2.value;
        double tp2 = p2.inTangent;
        double a, b, c, d;

        if (double.IsInfinity(tp1) || double.IsInfinity(tp2))
        {
            a = 0.0;
            b = 0.0;
            c = 0.0;
            d = p1.value;
        }
        else
        {
            double divisor = (p1x * p1x * p1x) - (p2x * p2x * p2x) + (3.0 * p1x * p2x * (p2x - p1x));
            a = ((tp1 + tp2) * (p1x - p2x) + (p2y - p1y) * 2.0) / divisor;
            b = (2.0 * (p2x * p2x * tp1 - p1x * p1x * tp2) - p1x * p1x * tp1 + p2x * p2x * tp2 + p1x * p2x * (tp2 - tp1) + 3.0 * (p1x + p2x) * (p1y - p2y)) / divisor;
            c = (p1x * p1x * p1x * tp2 - p2x * p2x * p2x * tp1 + p1x * p2x * (p1x * (2.0 * tp1 + tp2) - p2x * (tp1 + 2.0 * tp2)) + 6.0 * p1x * p2x * (p2y - p1y)) / divisor;
            d = ((p1x * p2x * p2x - p1x * p1x * p2x) * (p2x * tp1 + p1x * tp2) - p1y * p2x * p2x * p2x + p1x * p1x * p1x * p2y + 3.0 * p1x * p2x * (p2x * p1y - p1x * p2y)) / divisor;
        }
        return new Range() { a = a, b = b, c = c, d = d, minTime = p1.time };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe static bool IsFinite(float f)
    {
        return (*(int*)(&f) & 0x7FFFFFFF) < 2139095040;
    }
}