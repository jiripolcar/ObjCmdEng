﻿using CommanderEngine;
using CommanderEngine.Network;
using Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCommander : MonoBehaviour
{

    public static NetworkCommander Instance;

    public static bool IsServer { get { return NetworkServer != null; } }
    //public static bool HasClients { get { return NetworkServer.networkServerConnections.Count > 0; } }
    public static NetworkServer NetworkServer;

    private const char SyncStringDel = '#';

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        List<Commander> cmdrs = new List<Commander>(Commander.RegisteredCommanders.Values);
        cmdrs.ForEach((cmdr) =>
        {
            if (cmdr is AvatarNPCCommander)
            {
                AvatarNPCCommander acmdr = (AvatarNPCCommander)cmdr;
                if (acmdr.syncer == null)
                {
                    AvatarNPCCommanderSyncer syncer = acmdr.gameObject.AddComponent<AvatarNPCCommanderSyncer>();
                    syncer.avatarNPC = acmdr;
                    acmdr.syncer = syncer;

                }
            }

        });

    }
    /*
    public static void SendSyncMessage(string message)
    {
        if (NetworkServer == null)
            Debug.LogWarning("Network Server is null!");
        if (!HasClients)
            Debug.LogWarning("Server has no clients!");

        Instance.syncMessage += (Instance.syncMessage.Length > 0 ? "#" : "") + message;
        Debug.Log(Instance.syncMessage);
    }
    [TextArea(8, 8)]
    public string syncMessage;

    private void LateUpdate()
    {
        if (syncMessage.Length > 0)
        {
            if (NetworkServer.SendData(syncMessage))
                syncMessage = "";
            else
                Debug.Log("Could not send " + syncMessage);
        }

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

    }*/

    private List<AvatarNPCCommanderSyncMessage> syncList = new List<AvatarNPCCommanderSyncMessage>();

    public static void CollectSyncMessage(AvatarNPCCommanderSyncMessage msg)
    {
        Instance.syncList.Add(msg);
    }

    private void LateUpdate()
    {
        if (syncList.Count > 0)
        {
            string syncJson = JsonUtility.ToJson(syncList);
            ConsoleLog.Log.Write(syncJson, ConsoleLog.LogRecordType.NetworkCommander, false);
            NetworkServer.SendData(syncJson);
            syncList = new List<AvatarNPCCommanderSyncMessage>();

        }

    }

    public static void ReceiveSyncMessages(string data)
    {
        List<AvatarNPCCommanderSyncMessage> incoming = JsonUtility.FromJson<List<AvatarNPCCommanderSyncMessage>>(data);
        incoming.ForEach((msg) => msg.ApplyToAvatar());
    }
}
