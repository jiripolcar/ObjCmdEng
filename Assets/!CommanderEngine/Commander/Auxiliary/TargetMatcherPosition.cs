using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMatcherPosition : MonoBehaviour
{

    private Transform reference;
    private Transform destination;
    private float lerp = 1;
    private float time = 1;
    private float initialDistance;

    public static TargetMatcherPosition Match(GameObject subject, Transform reference, Transform destination, float time = 1)
    {
        TargetMatcherPosition targetMatcher = subject.AddComponent<TargetMatcherPosition>();
        targetMatcher.reference = reference;
        targetMatcher.destination = destination;
        targetMatcher.initialDistance = (destination.position - reference.position).magnitude;
        targetMatcher.time = time;
        return targetMatcher;
    }

    public void End()
    {
        Destroy(this);
    }

    void LateUpdate()
    {
        lerp -= Time.deltaTime / time;
        Vector3 delta = destination.position - reference.position;
    //    float angleDelta = Quaternion.Angle(transform.rotation, destination.rotation);
        if (lerp < 0)
        {
            transform.position += delta;
      //      transform.rotation = destination.rotation;
            End();
        }
        else
        {
            float currentDistance = (destination.position - reference.position).magnitude;
            float desiredDistance = initialDistance * lerp;
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, destination.rotation, angleDelta * Time.deltaTime / time);
            transform.position += delta.normalized * (currentDistance - desiredDistance);
        }

    }
}
