using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Movements {

    public static IEnumerator Move(GameObject target, Vector3 start, Vector3 end, float speed = 1)
    {
        float time = (start - end).magnitude / speed;
        float lerp = 0;
        do
        {
            LerpPosition(target, start, end, lerp);
            yield return 0;
            lerp += Time.deltaTime / time;
        }
        while (lerp < 1);
    }

    public static IEnumerator MoveDuration(GameObject target, Vector3 start, Vector3 end, float time = 1)
    {        
        float lerp = 0;
        do
        {
            LerpPosition(target, start, end, lerp);
            yield return 0;
            lerp += Time.deltaTime / time;
        }
        while (lerp < 1);
    }

    public static void LerpPosition(GameObject target, Vector3 start, Vector3 end, float lerp)
    {
        target.transform.position = Vector3.Lerp(start, end, lerp);
    }

    public static void LerpLocalPosition(GameObject target, Vector3 start, Vector3 end, float lerp)
    {
        target.transform.localPosition = Vector3.Lerp(start, end, lerp);
    }

    public static void LerpPositionInCoordinatesOf(GameObject target, Transform coordinateSystem, Vector3 start, Vector3 end, float lerp)
    {
        Vector3 positionInCoordinateSystem = Vector3.Lerp(start, end, lerp);
        target.transform.position = coordinateSystem.TransformPoint(positionInCoordinateSystem);
    }


}
