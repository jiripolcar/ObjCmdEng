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

        [SerializeField] internal Transform destination;
        [SerializeField] internal WalkCommandEndingStyle endingStyle = WalkCommandEndingStyle.None;
        [SerializeField] internal bool lerpAtEndPrecisely = true;
        [SerializeField] internal float stoppingDistance;
        [SerializeField] internal bool updatePosition = false;
        [SerializeField] internal float speed = 0.5f;

        public override string ToString()
        {
            string s = base.ToString();
            s += ";Command:Walk;" + destination.name
                + ";EndStyle:" + endingStyle.ToString()
                + (lerpAtEndPrecisely ? ";Lerp;" : ";")
                + (updatePosition ? ";Catch;" : ";")
                + "StopDist:" + stoppingDistance.ToString("0.000");
            return s;
        }

    }
}