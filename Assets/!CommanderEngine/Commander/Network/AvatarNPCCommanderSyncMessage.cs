using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine.Network
{
    [System.Serializable]
    public class AvatarNPCCommanderSyncMessage
    {
        public string name;
        public Vector3 position;
        public bool sitting;
        public float yaw, forward, right, variation;

        public static string JsonFromAvatarNPC(AvatarNPCCommander av)
        {
            return JsonUtility.ToJson(SyncFromAvatarNPC(av));
        }

        public static AvatarNPCCommanderSyncMessage SyncFromAvatarNPC(AvatarNPCCommander av)
        {
            AvatarNPCCommanderSyncMessage msg = new AvatarNPCCommanderSyncMessage()
            {
                name = av.name,
                position = av.transform.position,
                yaw = av.transform.eulerAngles.y,
                sitting = av.AnimatorSit,
                forward = av.animator.GetFloat("Forward"),
                right = av.animator.GetFloat("Turn"),
                variation = av.animator.GetFloat("Variant")
            };
            return msg;
        }

        public static void SyncAvatarNPCFromJson(string json)
        {
            AvatarNPCCommanderSyncMessage msg = JsonUtility.FromJson<AvatarNPCCommanderSyncMessage>(json);
            SyncAvatarNPC(msg);
        }

        public static void SyncAvatarNPC(AvatarNPCCommanderSyncMessage msg)
        {
            AvatarNPCCommander av = (AvatarNPCCommander)Commander.Find(msg.name);
            av.transform.position = msg.position;
            Vector3 ea = av.transform.eulerAngles;
            ea.y = msg.yaw;
            av.transform.eulerAngles = ea;
            av.AnimatorSit = msg.sitting;
            av.animator.SetFloat("Forward", msg.forward);
            av.animator.SetFloat("Turn", msg.right);
            av.animator.SetFloat("Variant", msg.variation);
        }

        public void ApplyToAvatar()
        {
            AvatarNPCCommander av = (AvatarNPCCommander)Commander.Find(name);
            av.transform.position = position;
            Vector3 ea = av.transform.eulerAngles;
            ea.y = yaw;
            av.transform.eulerAngles = ea;
            av.AnimatorSit = sitting;
            av.animator.SetFloat("Forward", forward);
            av.animator.SetFloat("Turn", right);
            av.animator.SetFloat("Variant", variation);
        }

    }
}