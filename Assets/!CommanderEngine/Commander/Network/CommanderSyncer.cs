using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderSyncer : MonoBehaviour {

    public const char Del = ':';

    public virtual void ReceiveSync(string syncString)
    {
        ConsoleLog.Log.Write("Commander can not be synced " + name, ConsoleLog.LogRecordType.NetworkCommander);
    }
}
