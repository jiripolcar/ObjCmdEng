using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{


    public CharacterCommander c1, c2, c3, ethan, joe;
    public WalkCommand walk1, walk2, walk3, walk4;
    public SitCommand sit1, sit2, sit3, sit4;
    public StandUpCommand joeStandup, ethanStandup, kidStandup;
        

    private void WalkSitStandup()
    {
        List<Command> toReg = new List<Command>() { walk1, walk3, walk2, sit1, sit3, sit2, joeStandup, kidStandup, ethanStandup };
        toReg.ForEach((t) => Command.RegisterSyncedCommand(t));
        Commander.Do(walk1);
        Commander.Do(walk3);
        Commander.Do(walk2);
        Commander.Do(sit1);
        Commander.Do(joeStandup);
        Commander.Do(sit3);
        Commander.Do(sit2);
        Commander.Do(kidStandup);
        Commander.Do(ethanStandup);
    }

    private void Sit()
    {
        sit1 = (SitCommand)Commander.Do("ow:Joe,cmd:sit,tgt:ChairJoe,de:0");
        sit3 = (SitCommand)Commander.Do("ow:Kid,cmd:sit,tgt:ChairKid,de:2");
        sit2 = (SitCommand)Commander.Do("ow:Ethan,cmd:sit,tgt:ChairEthan,de:4");
        sit4 = (SitCommand)Commander.Do("ow:Vaclav_Unity,cmd:sit,tgt:ChairVaclav,de:6");
    }

    private void Run()
    {
        walk1 = (WalkCommand)Commander.Do("ow:Joe,cmd:walk,dest:targ1,de:6,spd:1");
        walk2 = (WalkCommand)Commander.Do("ow:Kid,cmd:walk,dest:targ2,de:4,spd:1");
        walk3 = (WalkCommand)Commander.Do("ow:Vaclav_Unity,cmd:walk,dest:targ3,de:3,spd:1");
        walk4 = (WalkCommand)Commander.Do("ow:Ethan,cmd:walk,dest:targ4,de:2,spd:1");
    }

    private void Start()
    {
        Invoke("Sit", 0.5f);
       Invoke("Run", 1f);

    }

    // Update is called once per frame
    void Update()
    {

    }


}
