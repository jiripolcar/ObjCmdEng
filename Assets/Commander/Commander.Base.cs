using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConsoleLog;

namespace CommanderEngine
{
    public partial class Commander : MonoBehaviour
    {
        [SerializeField] List<Command> commandQueue = new List<Command>();
        [SerializeField] private Command currentCommand;
        public Command CurrentCommand { get { return currentCommand; } }
        public bool Busy { get; private set; }
        public bool InCommandCycle { get; private set; }

        private void Awake()
        {
            InitAndRegister(this);
        }

        internal void AddCommand(Command c)
        {
            commandQueue.Add(c);
            commandQueue.Sort();
            if (!InCommandCycle)
                StartCoroutine(CommitCommandsCycle());
        }

        public bool CancelCurrent()
        {
            return currentCommand.Cancel();
        }

        public bool RemoveCommand(Command cmd, bool removeSyncedToo = true)
        {
            if (commandQueue.Contains(cmd))
            {
                if (cmd == currentCommand)
                {
                    if (CancelCurrent())
                    {
                        commandQueue.Remove(cmd);
                        RemoveCommandSynced(cmd);
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    commandQueue.Remove(cmd);
                    RemoveCommandSynced(cmd);
                    return true;
                }
            }
            return false;
        }

        private void RemoveCommandSynced(Command cmd)
        {
            if (cmd.GroupPredecessor != null && commandQueue.Contains(cmd.GroupPredecessor))
                RemoveCommand(cmd.GroupPredecessor, false);
            if (cmd.GroupSuccessor != null && commandQueue.Contains(cmd.GroupSuccessor))
                RemoveCommand(cmd.GroupSuccessor, false);
            if (cmd.SyncWith != null && cmd.SyncWith.Count > 0)
                foreach (Command c in cmd.SyncWith)
                    if (c.Owner.commandQueue.Contains(c))
                        c.Owner.RemoveCommand(c, true);
        }


        private IEnumerator CommitCommandsCycle()
        {
            InCommandCycle = true;
            while (commandQueue.Count > 0)
            {
                currentCommand = currentCommand == null ? commandQueue[0] : currentCommand;
                if (currentCommand.State > CommandState.Pending)
                    commandQueue.Remove(currentCommand);
                else if (currentCommand.delay > 0)
                {
                    Busy = false;
                    currentCommand.State = CommandState.WaitingDelay;
                    currentCommand.delay -= Time.deltaTime;
                }
                else
                {
                    
                    if (currentCommand.IsSyncedCommand)
                    {
                        Log.Write("WaitSync:\t" + currentCommand.ToString(), LogRecordType.Commander);
                        currentCommand.State = CommandState.WaitingSync;
                        while (true)
                        {
                            int syncsLeft = currentCommand.SyncWith.Count;
                            currentCommand.SyncWith.ForEach((c) => { if (c.State >= CommandState.WaitingSync) syncsLeft--; });
                            if (syncsLeft <= 0) break;
                            yield return 0;
                        }

                    }
                    Log.Write("Commiting:\t" + currentCommand.ToString(), LogRecordType.Commander);
                    currentCommand.State = CommandState.Pending;
                    Busy = true;
                    yield return StartCoroutine(Commit(currentCommand));
                    currentCommand.State = CommandState.Disposed;
                    commandQueue.Remove(currentCommand);
                    DisposedCommands.Add(currentCommand);
                    currentCommand = currentCommand.GroupSuccessor;
                }
                yield return 0;
            }
            Busy = false;
            currentCommand = null;
            InCommandCycle = false;
        }

        protected virtual IEnumerator Commit(Command command)
        {
            Log.Write("Do not use default Commander!", LogRecordType.Error);
            yield break;
        }
    }
}