﻿using UnityEngine;
using System.Collections.Generic;
using ConsoleLog;
using System;

namespace CommanderEngine
{


    [System.Serializable]
    public partial class Command : IComparable
    {

        private static ushort lastCommandSyncID = 1;
        private static ushort GetNewCommandSyncID { get { while (GetSyncCommand(++lastCommandSyncID) != null) { } return lastCommandSyncID; } }

        [SerializeField] protected CommandState state = CommandState.Empty;
        public CommandState State { get { return state; } internal set { state = value; } }
        [SerializeField] protected Commander owner;
        public Commander Owner { get { return owner; } }
        [SerializeField] internal int priority = 0;
        [SerializeField] internal float delay = 0;
        [SerializeField] internal ushort syncId = 0;
        [SerializeField] internal ushort successor = 0;
        [SerializeField] internal ushort predecessor = 0;
        [SerializeField] internal List<ushort> syncsWith;


        internal List<Command> SyncsWith
        {
            get
            {
                if (syncId == 0)
                    return new List<Command>();
                else
                {
                    if (syncsWith == null || syncsWith.Count == 0)
                        return new List<Command>();
                    else
                    {
                        List<Command> l = new List<Command>();
                        foreach (ushort id in syncsWith)
                            l.Add(GetSyncCommand(id));
                        return l;
                    }
                }
            }
        }
        internal Command SyncedSuccessor
        {
            get { if (successor > 0) return GetSyncCommand(successor); else return null; }
            set { successor = value.syncId; }
        }
        internal Command SyncedPredecessor
        {
            get { if (predecessor > 0) return GetSyncCommand(predecessor); else return null; }
            set { predecessor = value.syncId; }
        }



        internal virtual bool Cancel() { state = CommandState.Canceled; return true; } // override if command class is not cancelable

        public override string ToString()
        {

            string syncs;
            if (syncsWith != null && syncsWith.Count > 0)
            {
                syncs = "sw:";
                for (int i = 0; i < syncsWith.Count; i++)
                    syncs += (i > 0 ? "," : "") + syncsWith;
            }
            else
                syncs = "";


            return "ow:" + Owner.gameObject.name
                + (priority != 0 ? ";pr:" + priority : "")
                + (delay > 0 ? ";de:" + delay.ToString("0.000") : "")
                + ";st:" + (int)state
                + (syncId > 0 ? ";id:" + syncId : "")
                + (successor > 0 ? ";sc:" + successor : "")
                + (predecessor > 0 ? ";pc:" + predecessor : "")
                + syncs;
        }

        public int CompareTo(object obj)
        {
            Command other = obj as Command;
            // if has predecessor, it will always follow a command => all commands that have predecessor can be at the end of the list
            if (HasPredecessor && !other.HasPredecessor) return 1;
            else if (!HasPredecessor && other.HasPredecessor) return -1;
            else
                return other.priority - priority;
        }

        public void SyncWith(Command other)
        {
            GetOrMakeSyncID();
            if (!SyncsWith.Contains(other))
            {
                other.GetOrMakeSyncID();
                syncsWith.Add(other.syncId);
                if (!other.syncsWith.Contains(this.syncId))
                    other.syncsWith.Add(this.syncId);
            }
        }

        public static void Sync(Command[] commands)
        {
            foreach (Command cmd in commands) cmd.GetOrMakeSyncID();
            if (commands == null || commands.Length < 2) return;
            for (int i = 0; i < commands.Length; i++)
            {
                for (int j = i + 1; j < commands.Length; j++)
                {
                    commands[i].SyncWith(commands[j]);
                }
            }
        }

        internal ushort GetOrMakeSyncID()
        {
            if (syncId == 0)
            {
                syncId = GetNewCommandSyncID;
                RegisterSyncedCommand(this);
            }
            return syncId;
        }

        public void SetSuccessor(Command successorCommand)
        {
            successorCommand.predecessor = GetOrMakeSyncID();
            successor = successorCommand.GetOrMakeSyncID();
            owner.SortCommands();
            if (successorCommand.owner != owner)
                successorCommand.owner.SortCommands();
        }

        public bool IsSyncedCommand { get { return syncsWith != null && syncsWith.Count > 0; } }
        public bool HasPredecessor { get { return predecessor != 0; } }
    }
}