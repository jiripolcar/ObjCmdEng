using UnityEngine;
using System.Collections.Generic;
using ConsoleLog;
using System;

namespace CommanderEngine
{
    public partial class Command
    {
        private static Dictionary<ushort, Command> registeredSyncedCommands;
        protected static Dictionary<ushort, Command> RegisteredSyncedCommands { get { if (registeredSyncedCommands == null) registeredSyncedCommands = new Dictionary<ushort, Command>(); return registeredSyncedCommands; } }

        public static Command GetSyncCommand(ushort id)
        {
            Command c;
            if (RegisteredSyncedCommands.TryGetValue(id, out c))
                return c;
            else
                return null;
        }

        public static void RegisterSyncedCommand(Command command)
        {
            if (command.syncId > 0 && !RegisteredSyncedCommands.ContainsKey(command.syncId))
                RegisteredSyncedCommands.Add(command.syncId, command);
        }



    }
}