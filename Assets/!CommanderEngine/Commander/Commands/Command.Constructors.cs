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
            ObjectIdentifier Destination,
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
                stoppingDistance = StoppingDistance == null ? DefaultStoppingDistance : StoppingDistance.Value
            };
            return c;
        }

        public static SitCommand Sit(
            Commander Owner,
            SeatControl Target,
            int? Priority = null,
            float? Delay = null
        )
        {
            if (Owner == null)
                throw new System.Exception("Commander.Walk: Owner does not exist!");
            if (Target == null)
                throw new System.Exception("Commander.Walk: Destination does not exist!");

            SitCommand c = new SitCommand()
            {
                owner = Owner,
                state = CommandState.New,
                priority = Priority == null ? DefaultCommandPriority : Priority.Value,
                delay = Delay == null ? 0 : Delay.Value,
                target = Target
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


        public static Command FromString(string commandString)
        {
            Commander Owner = null;
            int Priority = 0;
            float Delay = 0;
            ushort SyncId = 0;
            ushort Successor = 0;
            ushort Predecessor = 0;
            List<ushort> SyncsWith = new List<ushort>();
            CommandState State = CommandState.New;

            string[] parsed = commandString.Split(';');
            List<string> rest = new List<string>();

            string cmd = "";

            foreach (string s in parsed)
            {
                string attribute;
                string value;
                GetAttributeAndValue(s, out attribute, out value);
                switch (attribute)
                {
                    case "ow": Owner = Commander.Find(value); break;
                    case "pr": Priority = int.Parse(value); break;
                    case "de": Delay = float.Parse(value); break;
                    case "st": State = (CommandState)int.Parse(value); break;
                    case "id": SyncId = ushort.Parse(value); break;
                    case "sc": Successor = ushort.Parse(value); break;
                    case "pc": Predecessor = ushort.Parse(value); break;
                    case "sw":
                        {
                            string[] buffer = value.Split(',');
                            foreach (string n in buffer)
                                SyncsWith.Add(ushort.Parse(n));
                        }
                        break;
                    case "cmd": cmd = value; break;
                    default: rest.Add(s); break;
                }
            }

            Command result;

            switch (cmd.ToLower())
            {
                case "walk": result = WalkFromString(Owner, Priority, State, Delay, SyncId, Successor, Predecessor, SyncsWith, rest.ToArray()); break;
                case "sit": result = SitFromString(Owner, Priority, State, Delay, SyncId, Successor, Predecessor, SyncsWith, rest.ToArray()); break;
                default: result = null; break;
            }
            if (result.syncId > 0 && !registeredSyncedCommands.ContainsKey(result.syncId))
                RegisterSyncedCommand(result);
            return result;
        }

        private static WalkCommand WalkFromString(Commander Owner, int Priority, CommandState State, float Delay, ushort SyncId, ushort Successor, ushort Predecesssor, List<ushort> SyncsWithList, string[] parsed)
        {
            ObjectIdentifier Destination = null;
            WalkCommand.WalkCommandEndingStyle EndStyle = WalkCommand.WalkCommandEndingStyle.None;
            bool LerpAtEnd = true;
            float StopDist = DefaultStoppingDistance;
            bool Catching = false;
            float Speed = DefaultWalkSpeed;

            foreach (string s in parsed)
            {
                string attribute;
                string value;
                GetAttributeAndValue(s, out attribute, out value);
                switch (attribute)
                {
                    case "dest": Destination = ObjectIdentifier.Find(value); break;
                    case "spd": Speed = float.Parse(value); break;
                    case "end": EndStyle = (WalkCommand.WalkCommandEndingStyle)int.Parse(value); break;
                    case "lrp": LerpAtEnd = bool.Parse(value); break;
                    case "ctc": Catching = bool.Parse(value); break;
                    case "stdst": StopDist = float.Parse(value); break;
                }
            }

            WalkCommand wc = new WalkCommand()
            {
                owner = Owner,
                state = State,
                syncId = SyncId,
                priority = Priority,
                delay = Delay,
                successor = Successor,
                predecessor = Predecesssor,
                syncsWith = SyncsWithList,
                destination = Destination,
                speed = Speed,
                endingStyle = EndStyle,
                lerpAtEndPrecisely = LerpAtEnd,
                updatePosition = Catching,
                stoppingDistance = StopDist
            };
            return wc;
        }

        private static SitCommand SitFromString(Commander Owner, int Priority, CommandState State, float Delay, ushort SyncId, ushort Successor, ushort Predecesssor, List<ushort> SyncsWithList, string[] parsed)
        {
            SeatControl Destination = null;

            foreach (string s in parsed)
            {
                string attribute;
                string value;
                GetAttributeAndValue(s, out attribute, out value);
                switch (attribute)
                {
                    case "tgt": Destination = ObjectIdentifier.Find(value).seatControl; Debug.Log(Destination.name); break;
                }
            }

            SitCommand sc = new SitCommand()
            {
                owner = Owner,
                state = State,
                syncId = SyncId,
                priority = Priority,
                delay = Delay,
                successor = Successor,
                predecessor = Predecesssor,
                syncsWith = SyncsWithList,
                target = Destination
            };
            return sc;
        }

        private static void GetAttributeAndValue(string parsedElement, out string attribute, out string value)
        {
            if (parsedElement.Contains(":"))
            {
                string[] split = parsedElement.Split(':');
                attribute = split[0].ToLower();
                value = split[1];
            }
            else
            {
                attribute = parsedElement;
                value = "true";
            }
        }
    }
}