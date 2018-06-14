using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class Command
    {
        public const int DefaultCommandPriority = 0;
        public const float DefaultStoppingDistance = 0.1f;
        public const float DefaultWalkSpeed = 0.5f;


        /// <summary>
        /// Creates a WalkCommand.
        /// </summary>
        /// <param name="Owner">Subject commander</param>
        /// <param name="Destination">Destination transform to reach.</param>
        /// <param name="EndStyle">Style of aligning at the end. Default: none</param>
        /// <param name="PrecisionAlignAtEnd">Will lerp to the transform's position precisely. Default: false</param>
        /// <param name="Speed">Maximum value of the animator blend tree parameter.</param>
        /// <param name="Priority">Command priority</param>
        /// <param name="Catch">Update NMA target destination for a moving target. Default: false</param>
        /// <param name="StoppingDistance">Stops at the distance from the target. Default: see constant in Command class</param>
        /// <param name="Delay">Delay to wait before commiting this command.</param>
        /// <returns></returns>
        public static WalkCommand Walk(
            Commander Owner,
            Transform Destination,
            int? Priority = null,
            float? Delay = null,            
            WalkCommand.WalkCommandEndingStyle EndStyle = WalkCommand.WalkCommandEndingStyle.None,
            bool? PrecisionAlignAtEnd = null,
            float? Speed = null,
            bool? Catch = null,
            float? StoppingDistance = null
            )
        {
            if (Owner == null)
                throw new System.Exception("Commander.Walk: Owner does not exist!");
            if (Destination == null)
                throw new System.Exception("Commander.Walk: Destination does not exist!");

            WalkCommand c = new WalkCommand()
            {
                owner = Owner,
                state = CommandState.New,
                priority = Priority == null ? DefaultCommandPriority : Priority.Value,
                delay = Delay == null ? 0 : Delay.Value,
                speed = Speed == null ? DefaultWalkSpeed : Speed.Value,
                destination = Destination,
                endingStyle = EndStyle,
                lerpAtEndPrecisely = PrecisionAlignAtEnd == null ? false : PrecisionAlignAtEnd.Value,
                updatePosition = Catch == null ? false : Catch.Value,
                stoppingDistance = StoppingDistance == null ? 0.1f : StoppingDistance.Value
            };
            return c;
        }
        /*
        /// <summary>
        /// Format:
        /// owner;destinationOI;[priority];[delay];[endstyle:none/align/face];[lerpAtEnd];[catch];[stopDist];
        /// </summary>
        /// <param name="fromString"></param>
        /// <returns></returns>
        public static WalkCommand Walk(string fromString) {
            string[] vs = fromString.Split(';');

        }        
        */
    }
}