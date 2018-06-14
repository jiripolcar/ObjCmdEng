using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static float Magnitude2D(this Vector3 v)
    {
        return Mathf.Sqrt(v.x * v.x + v.z * v.z);
    }

    public static float PathLength(this List<Vector3> vectors)
    {
        float result = 0;
        int i;
        for (i = 1; i < vectors.Count; i++)
        {
            result += (vectors[i] - vectors[i - 1]).magnitude;
        }
        return result;
    }

}
