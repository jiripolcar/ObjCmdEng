using CommanderEngine;
using CommanderEngine.Network;
using NetworkUnetDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCommander : MonoBehaviour
{
    public static NetworkCommander Instance;

    public static bool IsServer { get { return NetworkServer != null; } }
    public static NetworkServer NetworkServer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (IsServer)
        {
            List<Commander> cmdrs = new List<Commander>(Commander.RegisteredCommanders.Values);
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
        Instance.syncData.avatarNPCSync.Add(msg);
    }

    private void LateUpdate()
    {
        if (syncData.avatarNPCSync.Count > 0)
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
        incoming.ApplyAll();
    }

}
