using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{

    [System.Serializable]
    public class SitCommand : Command
    {
        [SerializeField] internal SeatControl target;

        public override string ToString()
        {
            string s = base.ToString();
            s += parDel + "cmd" + parSep + "sit" + parDel + "tgt" + parSep + target.gameObject.GetObjectIdentifier().identifyAs;
            return s;
        }

    }
}