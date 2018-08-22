using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConsoleLog;

namespace CommanderEngine
{
    public partial class Commander : MonoBehaviour
    {
        [SerializeField] private List<Command> commandQueue = new List<Command>();
        [SerializeField] private Command currentCommand;        
        public Command CurrentCommand { get { return currentCommand; } }
        public bool Busy { get; private set; }
        public bool InCommandCycle { get; private set; }
        [SerializeField] internal CommanderSyncer syncer;

        private void Reset()
        {
            CommanderReset();   
        }

        private void Awake()
        {
            InitAndRegister(this);
        }

        protected void CommanderReset()
        {
            if (!gameObject.GetObjectIdentifier())
                gameObject.AddComponent<ObjectIdentifier>();
        }

        internal void AddCommand(Command c)
        {
            commandQueue.Add(c);
            commandQueue.Sort();
            c.State = CommandState.InQueue;
            if (!InCommandCycle)
                StartCoroutine(CommitCommandsCycle());
        }

        internal void SortCommands()
        {
            commandQueue.Sort();
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
            if (cmd.SyncedPredecessor != null && commandQueue.Contains(cmd.SyncedPredecessor))
                RemoveCommand(cmd.SyncedPredecessor, false);
            if (cmd.SyncedSuccessor != null && commandQueue.Contains(cmd.SyncedSuccessor))
                RemoveCommand(cmd.SyncedSuccessor, false);
            if (cmd.SyncsWith != null && cmd.SyncsWith.Count > 0)
                foreach (Command c in cmd.SyncsWith)
                    if (c.Owner.commandQueue.Contains(c))
                        c.Owner.RemoveCommand(c, true);
        }


        private IEnumerator CommitCommandsCycle()
        {
            InCommandCycle = true;
            while (commandQueue.Count > 0)
            {
                currentCommand = currentCommand == null || currentCommand.State <= CommandState.Empty ? commandQueue[0] : currentCommand;
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
                    if (currentCommand.HasPredecessor)
                    {
                        Command predecessor = currentCommand.SyncedPredecessor;
                        Log.Write("WaitPredecessor:\t" + currentCommand.ToString() + " PRED: " + predecessor.ToString(), LogRecordType.Commander);
                        currentCommand.State = CommandState.WaitingPredecessor;                        
                        while (currentCommand.State == CommandState.WaitingPredecessor && predecessor.State <= CommandState.Pending)
                            yield return 0;
                    }
                    if (currentCommand.IsSyncedCommand && currentCommand.State < CommandState.Pending)
                    {
                        Log.Write("WaitSync:\t" + currentCommand.ToString(), LogRecordType.Commander);
                        currentCommand.State = CommandState.WaitingSync;
                        List<Command> syncs = currentCommand.SyncsWith;
                        while (currentCommand.State == CommandState.WaitingSync)
                        {
                            int syncsLeft = syncs.Count;
                            syncs.ForEach((c) => { if (c.State >= CommandState.WaitingSync) syncsLeft--; });
                            if (syncsLeft <= 0) break;
                            yield return 0;
                        }
                    }


                    currentCommand.State = CommandState.Pending;                    
                    Log.Write("Commiting:\t" + currentCommand.ToString(), LogRecordType.Commander);
                    
                    Busy = true;
                    yield return StartCoroutine(Commit(currentCommand));
                    currentCommand.State = CommandState.Disposed;
                    commandQueue.Remove(currentCommand);
                    DisposedCommands.Add(currentCommand);
                    currentCommand = currentCommand.SyncedSuccessor;

                }
                yield return 0;
            }
            Busy = false;
            currentCommand = null;
            InCommandCycle = false;
            print(this + " exit command cycle ");
        }

        protected virtual IEnumerator Commit(Command command)
        {
            Log.Write("Do not use default Commander!", LogRecordType.Error);
            yield return new WaitForSeconds(1);
        }
    }
}