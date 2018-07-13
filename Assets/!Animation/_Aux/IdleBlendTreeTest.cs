using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBlendTreeTest : MonoBehaviour {

    public Animator anim;    
    [Range(0f,1f)]    public float variant;
    // Update is called once per frame

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update () {
        anim.SetFloat("Variant", variant);
	}
}
