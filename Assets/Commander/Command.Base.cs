using UnityEngine;
using System.Collections.Generic;
using ConsoleLog;
using System;

namespace CommanderEngine
{


    [System.Serializable]
    public partial class Command : IComparable
    {
        /*
        private static uint lastCommandID = 0;
        private static uint GetCommandID { get { while (Commander.FindAnyCommandByID(++lastCommandID) != null) { } return lastCommandID; } }
        [SerializeField] internal uint commandID;
        */

#if UNTITY_EDITOR
        [SerializeField] 
#endif
        protected CommandState state = CommandState.Empty;
        public CommandState State { get { return state; } internal set { state = value; } }

#if UNTITY_EDITOR
        [SerializeField] 
#endif
        protected Commander owner = null;
        public Commander Owner { get { return owner; } }

#if UNTITY_EDITOR
        [SerializeField] 
#endif
        internal int priority = 0;

#if UNTITY_EDITOR
        [SerializeField] 
#endif
        internal float delay = 0;

        public Command GroupSuccessor { get; protected set; }
        public Command GroupPredecessor { get; protected set; }
        public List<Command> SyncWith { get; protected set; }

        internal virtual bool Cancel() { state = CommandState.Canceled; return true; } // override if command class is not cancelable

        public override string ToString()
        {
            return owner.name +
                ";priority:" + priority +
                ";delay:" + delay.ToString("0.000") +
                ";state:" + state.ToString() +
                (GroupSuccessor == null ? "" : ";hasSuccessor") +
            (GroupPredecessor == null ? "" : ";hasPredecessor") +
            (SyncWith == null && SyncWith.Count > 0 ? "" : ";syncsWith:" + SyncWith.Count);
        }

        public int CompareTo(object obj)
        {
            Command other = obj as Command;
            if (HasSuccessor && !other.HasSuccessor) return 1;
            else if (!HasSuccessor && other.HasSuccessor) return -1;
            else
                return priority - other.priority;
        }

        internal bool IsSyncedCommand { get { return SyncWith != null || SyncWith.Count > 0; } }
        internal bool HasSuccessor { get { return GroupSuccessor != null; } }
    }
}