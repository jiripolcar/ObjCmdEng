using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{


    public CharacterCommander c1, c2, c3, ethan, joe;
    public WalkCommand a, b, c, d, e;

    private void Start()
    {
        string x = a.ToString();
        ConsoleLog.Log.Write(x, ConsoleLog.LogRecordType.Engine);
        b = (WalkCommand)Command.FromString(x);
        c = (WalkCommand)Command.FromString("ow:Joe;st:10;cmd:walk;dest:targ2");

        b.SyncWith(c);

        

        Commander.Do(b);
        Commander.Do(c);
        Commander.Do(d);

    }

    // Update is called once per frame
    void Update()
    {

    }


}
