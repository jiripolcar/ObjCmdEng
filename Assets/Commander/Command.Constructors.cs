using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class Command
    {
        public const int DefaultCommandPriority = 0;
        public const float DefaultStoppingDistance = 0.1f;

        public static WalkCommand Walk(Commander Owner, Transform Destination, WalkCommand.WalkCommandEndingStyle EndStyle,
            int? Priority = null, bool? Catch = null, float? StoppingDistance = null, bool? PrecisionAlignAtEnd = null,
            float? Delay = null)
        {
            if (Owner == null)
                throw new System.Exception("Commander.Walk: Owner does not exist!");
            if (Destination == null)
                throw new System.Exception("Commander.Walk: Destination does not exist!");

            WalkCommand c = new WalkCommand()
            {
                destination = Destination,
                endingStyle = EndStyle,
                priority = Priority == null ? DefaultCommandPriority : Priority.Value,
                lerpAtEndPrecisely = PrecisionAlignAtEnd == null ? false : PrecisionAlignAtEnd.Value,
                delay = Delay == null ? 0 : Delay.Value,
                owner = Owner,
                updatePosition = Catch == null ? false : Catch.Value,
                state = CommandState.New,
                stoppingDistance = StoppingDistance == null ? 0.1f : StoppingDistance.Value
            };
            return c;
        }
    }
}