using CommanderEngine;
using CommanderEngine.Network;
using Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCommander : MonoBehaviour {

    public static NetworkCommander Instance;

    public static bool IsServer { get { return NetworkServer != null; } }
    public static NetworkServer NetworkServer;

    private const char SyncStringDel = '#';

    private void Awake()
    {
        Instance = this;
    }



    public static void SendSyncMessage(string message)
    {
        if (NetworkServer == null)
            Debug.LogWarning("Network Server is null!");

        Instance.syncMessage += Instance.syncMessage.Length > 0 ? "#" : "" + message;
    }
    [TextArea(8, 8)]
    public string syncMessage;
    private void LateUpdate()
    {
        if (syncMessage.Length > 0)
        {
            NetworkServer.SendData(syncMessage);
        }
        syncMessage = "";
    }

    public static void ReceiveSyncedCommand(string data)
    {
        string[] multipleSyncsBuffer = data.Split(SyncStringDel);
        foreach (string singleSyncBuffer in multipleSyncsBuffer)
        {            
            string[] buffer = singleSyncBuffer.Split(CommanderSyncer.Del);
            Commander cmdr = Commander.Find(buffer[0]);
            Debug.Log("Syncing: " + singleSyncBuffer + (cmdr != null ? " Found " + cmdr.name : "NOT FOUND!"));
            if (cmdr)
                cmdr.syncer.ReceiveSync(singleSyncBuffer);
        }
        
    }
}
