using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommanderEngine
{
    public enum CommandState : int
    {
        Canceled = -20,
        Invalid = -10,
        Empty = 0,
        New = 10,
        InQueue = 20,
        WaitingDelay = 30,
        WaitingPredecessor = 35,
        WaitingSync = 40,
        Pending = 50,        
        Disposed = 60
    }
}