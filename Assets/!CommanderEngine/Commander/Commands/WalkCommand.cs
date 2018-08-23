using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{

    [System.Serializable]
    public class WalkCommand : Command
    {
        public enum WalkCommandEndingStyle
        {
            None,
            AlignWith,
            Face
        }

        [SerializeField] internal GameObject destination;
        [SerializeField] internal WalkCommandEndingStyle endingStyle = WalkCommandEndingStyle.None;
        [SerializeField] internal bool lerpAtEndPrecisely = true;
        [SerializeField] internal float stoppingDistance = DefaultStoppingDistance;
        [SerializeField] internal bool updatePosition = false;
        [SerializeField] internal float speed = DefaultWalkSpeed;

        public override string ToString()
        {
            string s = base.ToString();
            s += parDel + "cmd" + valSep + "walk" + parDel +
               parDel + "dest" + valSep + destination.name +
               parDel + "spd" + valSep + speed.ToString("0.000") +
                parDel + "end" + valSep + (int)endingStyle +
                (lerpAtEndPrecisely ? parDel + "lrp" : "") +
                (updatePosition ? parDel + "ctc" : "") +
                parDel + "stdst" + valSep + stoppingDistance.ToString("0.000");
            return s;
        }

    }
}