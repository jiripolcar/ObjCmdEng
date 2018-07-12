using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMatcherRotation : MonoBehaviour
{


    private Transform destination;
    private float time = 1;


    public static TargetMatcherRotation Match(GameObject subject, Transform destination, float time = 1)
    {
        TargetMatcherRotation targetMatcher = subject.AddComponent<TargetMatcherRotation>();
        targetMatcher.destination = destination;
        targetMatcher.time = time;
        return targetMatcher;
    }

    public void End()
    {
        Destroy(this);
    }

    void LateUpdate()
    {


        float angleDelta = Quaternion.Angle(transform.rotation, destination.rotation);
        if (Quaternion.Angle(transform.rotation, destination.rotation) < 1)
        {
            transform.rotation = destination.rotation;
            End();
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, destination.rotation, angleDelta * Time.deltaTime / time);
        }

    }
}
