using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{

    [System.Serializable]
    public class StandUpCommand : Command
    {
        public override string ToString()
        {
            string s = base.ToString();
            s += parDel + "cmd" + valSep + "standup";
            return s;
        }

    }
}