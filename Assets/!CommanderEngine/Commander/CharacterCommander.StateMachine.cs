using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class CharacterCommander : Commander
    {
        protected override IEnumerator Commit(Command command)
        {
            if (command is WalkCommand)
            {                
                yield return StartCoroutine(Walk((WalkCommand)command));
            }
        }



    }
}