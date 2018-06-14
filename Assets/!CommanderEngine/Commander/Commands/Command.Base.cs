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


        [SerializeField] protected CommandState state = CommandState.Empty;
        public CommandState State { get { return state; } internal set { state = value; } }
        [SerializeField] protected Commander owner;
        public Commander Owner { get { return owner; } }
        [SerializeField] internal int priority = 0;
        [SerializeField] internal float delay = 0;
        internal Command GroupSuccessor { get; set; }
        internal Command GroupPredecessor { get; set; }
        [NonSerialized] private List<Command> syncsWith;
        public List<Command> SyncsWith { get { if (syncsWith == null) syncsWith = new List<Command>(); return syncsWith; } }

        internal virtual bool Cancel() { state = CommandState.Canceled; return true; } // override if command class is not cancelable

        public override string ToString()
        {

            return Owner.gameObject.name +
                ";priority:" + priority +
                ";delay:" + delay.ToString("0.000") +
                ";state:" + state.ToString() +
                (GroupSuccessor == null ? "" : ";hasSuccessor") +
                (GroupPredecessor == null ? "" : ";hasPredecessor") +
                (SyncsWith == null && SyncsWith.Count > 0 ? "" : ";syncsWith:" + SyncsWith.Count);
        }

        public int CompareTo(object obj)
        {
            Command other = obj as Command;
            if (HasSuccessor && !other.HasSuccessor) return 1;
            else if (!HasSuccessor && other.HasSuccessor) return -1;
            else
                return priority - other.priority;
        }

        public void SyncWith(Command other)
        {
            if (!SyncsWith.Contains(other))
            {
                SyncsWith.Add(other);
                if (!other.SyncsWith.Contains(this))
                    other.SyncsWith.Add(this);
            }
        }

        public static void Sync(Command[] commands)
        {
            if (commands == null || commands.Length < 2) return;
            for (int i = 0; i < commands.Length; i++)
            {
                for (int j = i + 1; j < commands.Length; j++)
                {
                    commands[i].SyncWith(commands[j]);
                }
            }
        }

        public void SetSuccessor(Command successor)
        {
            GroupSuccessor = successor;
            owner.SortCommands();
        }

        internal bool IsSyncedCommand { get { return SyncsWith != null || SyncsWith.Count > 0; } }
        internal bool HasSuccessor { get { return GroupSuccessor != null; } }
    }
}