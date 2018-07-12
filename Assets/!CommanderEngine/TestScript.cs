using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{


    public CharacterCommander c1, c2, c3, ethan, joe;
    public WalkCommand a, b, c, d, e, joeWalkToChair, ethanWalkToChair, kidWalkToChair;
    public SitCommand joeSit, ethanSit, kidSit;
    public StandUpCommand joeStandup, ethanStandup, kidStandup;


    private void WalkSitStandup()
    {
        List<Command> toReg = new List<Command>() { a, b, c, d, e, joeWalkToChair, kidWalkToChair, ethanWalkToChair, joeSit, kidSit, ethanSit, joeStandup, kidStandup, ethanStandup };
        toReg.ForEach((t) => Command.RegisterSyncedCommand(t));
        Commander.Do(joeWalkToChair);
        Commander.Do(kidWalkToChair);
        Commander.Do(ethanWalkToChair);
        Commander.Do(joeSit);
        Commander.Do(joeStandup);
        Commander.Do(kidSit);
        Commander.Do(ethanSit);
        Commander.Do(kidStandup);
        Commander.Do(ethanStandup);
    }

    private void Sit()
    {
        joeSit = (SitCommand)Commander.Do("ow:Joe;cmd:sit;tgt:ChairJoe");
        kidSit = (SitCommand)Commander.Do("ow:Kid;cmd:sit;tgt:ChairKid");
        ethanSit = (SitCommand)Commander.Do("ow:Ethan;cmd:sit;tgt:ChairEthan");
        Commander.Do("ow:Vaclav_Unity;cmd:sit;tgt:ChairVaclav");
    }

    private void Start()
    {
        Invoke("Sit", 0.5f);
        
    }

    // Update is called once per frame
    void Update()
    {

    }


}
