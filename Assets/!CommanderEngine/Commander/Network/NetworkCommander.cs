using CommanderEngine;
using CommanderEngine.Network;
using NetworkUnetDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCommander : MonoBehaviour
{
    public static NetworkCommander Instance;
    public static bool IsClient { get { return NetworkClient != null; } }
    public static bool IsServer { get { return NetworkServer != null; } }
    public static NetworkServer NetworkServer = null;
    public static NetworkClient NetworkClient = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (IsServer)
        {
            List<Commander> cmdrs = ObjectIdentifier.GetAllCommanders();
            cmdrs.ForEach((cmdr) =>
            {
                if (cmdr is AvatarNPCCommander)
                {
                    AvatarNPCCommander acmdr = (AvatarNPCCommander)cmdr;
                    if (acmdr.syncer == null)
                    {
                        AvatarSyncer syncer = acmdr.gameObject.AddComponent<AvatarSyncer>();
                        syncer.avatarNPC = acmdr;
                        acmdr.syncer = syncer;

                    }
                }
            });
        }
    }

    public NetworkSyncData syncData;

    public static void CollectSyncMessage(AvatarSyncMessage msg)
    {
        if (IsServer)
            Instance.syncData.avatarNPCSync.Add(msg);
        else
            ConsoleLog.Log.Write("Only Network Server can send state syncs!", ConsoleLog.LogRecordType.Error);
    }

    public static void CollectCommandSync(Command cmd)
    {
        if (IsClient)
            Instance.syncData.commandSync.Add(cmd);
        else
            ConsoleLog.Log.Write("Only Network Clients can add command syncs!", ConsoleLog.LogRecordType.Error);
    }

    private void LateUpdate()
    {
        if (IsServer)
            ServerSendSync();
        else
            ClientSendSync();
    }

    private void ServerSendSync()
    {
        if (syncData.avatarNPCSync.Count > 0)
        {
            string syncString = syncData.Serialize();
            syncData.avatarNPCSync = new List<AvatarSyncMessage>();
            ConsoleLog.Log.Write(syncString, ConsoleLog.LogRecordType.NetworkCommander, false);
            NetworkServer.SendData(syncString);
        }
    }

    private void ClientSendSync()
    {
        if (syncData.commandSync.Count > 0)
        {
            string syncString = syncData.Serialize();
            syncData.avatarNPCSync = new List<AvatarSyncMessage>();
            ConsoleLog.Log.Write(syncString, ConsoleLog.LogRecordType.NetworkCommander, false);
            NetworkServer.SendData(syncString);
        }
    }

    public static void ReceiveSyncMessages(string data)
    {
        NetworkSyncData incoming = NetworkSyncData.Deserialize(data);
        if (IsClient)
            incoming.ApplyAllAvatarSyncs();
        if (IsServer)
            incoming.DoAllCommands();
    }

}
