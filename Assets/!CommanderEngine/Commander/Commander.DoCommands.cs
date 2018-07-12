using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class Commander : MonoBehaviour
    {
        public static WalkCommand DoWalk(Commander Owner,
            ObjectIdentifier Destination,
            int? Priority = null,
            float? Delay = null,
            WalkCommand.WalkCommandEndingStyle EndStyle = WalkCommand.WalkCommandEndingStyle.None,
            bool? PrecisionAlignAtEnd = null,
            float? Speed = null,
            bool? Catch = null,
            float? StoppingDistance = null)
        {
            WalkCommand walkCommand = Command.Walk(Owner, Destination, Priority, Delay, EndStyle, PrecisionAlignAtEnd, Speed, Catch, StoppingDistance);
            Do(walkCommand);
            return walkCommand;
        }
    }
}