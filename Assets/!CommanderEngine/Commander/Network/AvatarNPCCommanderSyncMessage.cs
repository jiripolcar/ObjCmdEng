using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine.Network
{
    [System.Serializable]
    public class ListOfAvatarNPCCommanderSyncMessage
    {
        public List<AvatarNPCCommanderSyncMessage> list;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);

        }

        public static ListOfAvatarNPCCommanderSyncMessage FromJson(string json)
        {
            return JsonUtility.FromJson<ListOfAvatarNPCCommanderSyncMessage>(json);
        }
    }

    [System.Serializable]
    public class AvatarNPCCommanderSyncMessage
    {
        public string n;    // name
        public Vector3Int p;   // position
        public bool s;      // sitting
        public float y, f, r, v;    // yaw, front, right, variant

        public static string JsonFromAvatarNPC(AvatarNPCCommander av)
        {
            return JsonUtility.ToJson(SyncFromAvatarNPC(av));
        }

        public static AvatarNPCCommanderSyncMessage SyncFromAvatarNPC(AvatarNPCCommander av)
        {
            AvatarNPCCommanderSyncMessage msg = new AvatarNPCCommanderSyncMessage()
            {
                n = av.name,
                p = Vector3Int.RoundToInt(av.transform.position * 1000),
                y = av.transform.eulerAngles.y,
                s = av.AnimatorSit,
                f = av.animator.GetFloat("Forward"),
                r = av.animator.GetFloat("Turn"),
                v = av.animator.GetFloat("Variant")
            };
            return msg;
        }

        public static void SyncAvatarNPCFromJson(string json)
        {
            AvatarNPCCommanderSyncMessage msg = JsonUtility.FromJson<AvatarNPCCommanderSyncMessage>(json);
            msg.ApplyToAvatar();
        }

        /*public static void SyncAvatarNPC(AvatarNPCCommanderSyncMessage msg)
        {
            AvatarNPCCommander av = (AvatarNPCCommander)Commander.Find(msg.n);
            av.transform.position = ((Vector3)msg.p) / 1000;
            Vector3 ea = av.transform.eulerAngles;
            ea.y = msg.y;
            av.transform.eulerAngles = ea;
            av.AnimatorSit = msg.s;
            av.animator.SetFloat("Forward", msg.f);
            av.animator.SetFloat("Turn", msg.r);
            av.animator.SetFloat("Variant", msg.v);
        }*/

        public void ApplyToAvatar()
        {
            AvatarNPCCommander av = (AvatarNPCCommander)Commander.Find(n);
            av.transform.position = ((Vector3)p) / 1000;
            Vector3 ea = av.transform.eulerAngles;
            ea.y = y;
            av.transform.eulerAngles = ea;
            av.AnimatorSit = s;
            av.animator.SetFloat("Forward", f);
            av.animator.SetFloat("Turn", r);
            av.animator.SetFloat("Variant", v);
        }

    }
}