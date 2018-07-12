using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTarget : MonoBehaviour {

    public Animator animator;
    public GameObject target;
    public AvatarTarget avT;
    public float startTime;
    public float endTime;
    public MatchTargetWeightMask mask;
    public bool match;

    public Vector3 matchWeight;
    public float rotWeight;

    public bool isMatching;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        isMatching = animator.isMatchingTarget;

        if (match && !isMatching)
        {
            mask = new MatchTargetWeightMask(matchWeight, rotWeight);
            match = false;
            animator.MatchTarget(target.transform.position, target.transform.rotation, avT, mask, startTime, endTime);
        }
	}

    public bool ik;

    void OnAnimatorIK()
    {
        if (ik)
        {
            animator.SetIKPosition(AvatarIKGoal.RightFoot, target.transform.position);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        }
    }
}
