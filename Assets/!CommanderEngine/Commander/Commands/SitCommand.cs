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
            s += parDel + "cmd" + valSep + "sit" + parDel + "tgt" + valSep + target.gameObject.GetObjectIdentifier().identifyAs;
            return s;
        }

    }
}