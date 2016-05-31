//-------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
//-------------------------------------------------------------------------------------------------

public class RangeValue : MonoBehaviour
{
    public static T Get<T>(T _min, T _max, T _value) where T : System.IComparable<T>
    {
        var result = _value;

        if (0 < _min.CompareTo(result))
            result = _min;

        if (_max.CompareTo(result) < 0)
            result = _max;

        return result;
    }
}
//-------------------------------------------------------------------------------------------------