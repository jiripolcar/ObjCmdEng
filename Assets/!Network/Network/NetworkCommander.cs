using CommanderEngine;
using CommanderEngine.Network;
using Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCommander : MonoBehaviour {

    public static NetworkCommander Instance;

    public static bool IsServer = false;
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
        Instance.syncMessage += "#" + message;
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
            Debug.Log("Syncing: " + singleSyncBuffer + (cmdr == null ? " Found " + cmdr.name : "NOT FOUND!"));
            if (cmdr is AvatarNPCCommander)
                cmdr.syncer.ReceiveSync(singleSyncBuffer);
        }
        
    }
}
