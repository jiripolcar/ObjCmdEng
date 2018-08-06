using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine.Network
{


    [System.Serializable]
    public class ListOfAvatarNPCCommanderSyncMessage
    {
        public const char Del = ';';
        public List<AvatarNPCCommanderSyncMessage> list;

        public void Push(AvatarNPCCommanderSyncMessage msg)
        {
            list.Add(msg);
        }

        public AvatarNPCCommanderSyncMessage Pull()
        {
            if (list.Count > 0)
            {
                AvatarNPCCommanderSyncMessage msg = list[0];
                list.RemoveAt(0);
                return msg;
            }
            else return null;
        }

        public string Serialize()
        {
            string ser = "";
            for(int i = 0; i < list.Count; i++)
            {
                ser += list[i].Serialize();
                if (i < list.Count - 1)
                    ser += Del;
            }
            return ser;
            //return JsonUtility.ToJson(this);

        }

        public static ListOfAvatarNPCCommanderSyncMessage Deserialize(string serialized)
        {
            string[] b = serialized.Split(Del);
            ListOfAvatarNPCCommanderSyncMessage l = new ListOfAvatarNPCCommanderSyncMessage();
            l.list = new List<AvatarNPCCommanderSyncMessage>();
            foreach (string s in b)
            {
                l.list.Add(AvatarNPCCommanderSyncMessage.FromString(s));
            }
            return l;
            //return JsonUtility.FromJson<ListOfAvatarNPCCommanderSyncMessage>(json);
        }
    }

    [System.Serializable]
    public class AvatarNPCCommanderSyncMessage
    {
        public const char Del = ',';

        public string n;    // name
        public Vector3Int p;   // position
        public bool s;      // sitting
        public int y, f, r, v;    // yaw, front, right, variant

        public string Serialize()
        {
           // AvatarNPCCommanderSyncMessage msg = ToSyncMessage(av);
            return n + Del + p.x + Del + p.y + Del +p.z + Del
                + (s ? "1" : "0") + Del + y + Del + f + Del + r + Del + v;


            //return JsonUtility.ToJson(ToSyncMessage(av));
        }

        public static AvatarNPCCommanderSyncMessage ToSyncMessage(AvatarNPCCommander av)
        {
            AvatarNPCCommanderSyncMessage msg = new AvatarNPCCommanderSyncMessage()
            {
                n = av.name,
                p = Vector3Int.RoundToInt(av.transform.position * 1000),
                y = Mathf.RoundToInt(av.transform.eulerAngles.y * 1000),
                s = av.AnimatorSit,
                f = Mathf.RoundToInt(av.animator.GetFloat("Forward") * 1000),
                r = Mathf.RoundToInt(av.animator.GetFloat("Turn") * 1000),
                v = Mathf.RoundToInt(av.animator.GetFloat("Variant") * 1000)
            };
            return msg;
        }

        public static void Deserialize(string serialized)
        {
            AvatarNPCCommanderSyncMessage msg = FromString(serialized); //  JsonUtility.FromJson<AvatarNPCCommanderSyncMessage>(serialized);
            msg.Apply();
        }

        public static AvatarNPCCommanderSyncMessage FromString(string serialized)
        {
            string[] b = serialized.Split(Del);
            AvatarNPCCommanderSyncMessage msg = new AvatarNPCCommanderSyncMessage()
            {
                n = b[0],
                p = new Vector3Int(int.Parse(b[1]), int.Parse(b[2]), int.Parse(b[3])),
                s = b[4] == "1" ? true : false,
                y = int.Parse(b[5]),
                f = int.Parse(b[6]),
                r = int.Parse(b[7]),
                v = int.Parse(b[8])
            };
            return msg;
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

        public void Apply()
        {
            AvatarNPCCommander av = (AvatarNPCCommander)Commander.Find(n);
            av.transform.position = ((Vector3)p) / 1000;
            Vector3 ea = av.transform.eulerAngles;
            ea.y = y / 1000;
            av.transform.eulerAngles = ea;
            av.AnimatorSit = s;
            av.animator.SetFloat("Forward", ((float)f) / 1000);
            av.animator.SetFloat("Turn", ((float)r) / 1000);
            av.animator.SetFloat("Variant", ((float)v) / 1000);
        }

    }
}