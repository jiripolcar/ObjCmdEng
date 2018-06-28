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

        [SerializeField] internal ObjectIdentifier destination;
        [SerializeField] internal WalkCommandEndingStyle endingStyle = WalkCommandEndingStyle.None;
        [SerializeField] internal bool lerpAtEndPrecisely = true;
        [SerializeField] internal float stoppingDistance = DefaultStoppingDistance;
        [SerializeField] internal bool updatePosition = false;
        [SerializeField] internal float speed = DefaultWalkSpeed;

        public override string ToString()
        {
            string s = base.ToString();
            s += ";cmd:walk;" +
                ";dest:" + destination.name +
                ";spd:" + speed.ToString("0.000") +
                ";end:" + (int)endingStyle +
                (lerpAtEndPrecisely ? ";lrp" : "") +
                (updatePosition ? ";ctc" : "") +
                ";stdst:" + stoppingDistance.ToString("0.000");
            return s;
        }

    }
}