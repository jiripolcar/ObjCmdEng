using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    public Transform t1, t2, t3, t4;
    public CharacterCommander c1, c2, c3, ethan, joe;

    private void Start()
    {
        Commander.DoWalk(ethan, t1);
        Commander.DoWalk(joe, ethan.transform, null, null, WalkCommand.WalkCommandEndingStyle.None, false, 1, true, 2);
    }
    
	// Update is called once per frame
	void Update () {
		
	}
}
