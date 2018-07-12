using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public float speed = 5;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(transform.up, Time.deltaTime * speed);
	}
}
