using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine.Network
{
    [System.Serializable]
    public class AvatarSyncMessage
    {
        public const char Del = ',';

        public string n;    // name
        public Vector3Int p;   // position
        public bool s;      // sitting
        public int y, f, r, v;    // yaw, front, right, variant

        public string Serialize()
        {
            return n + Del + p.x + Del + p.y + Del + p.z + Del
                + (s ? "1" : "0") + Del + y + Del + f + Del + r + Del + v;
        }

        public static AvatarSyncMessage ToSyncMessage(AvatarNPCCommander av)
        {
            AvatarSyncMessage msg = new AvatarSyncMessage()
            {
                n = av.name,
                p = Vector3Int.RoundToInt(av.transform.position * 1000),
                y = Mathf.RoundToInt(av.transform.eulerAngles.y*1000),
                s = av.AnimatorSit,
                f = Mathf.RoundToInt(av.animator.GetFloat("Forward") * 1000),
                r = Mathf.RoundToInt(av.animator.GetFloat("Turn") * 1000),
                v = Mathf.RoundToInt(av.animator.GetFloat("Variant") * 1000)
            };
            return msg;
        }

        public static void Deserialize(string serialized)
        {
            AvatarSyncMessage msg = FromString(serialized); //  JsonUtility.FromJson<AvatarNPCCommanderSyncMessage>(serialized);
            msg.Apply();
        }

        public static AvatarSyncMessage FromString(string serialized)
        {
            string[] b = serialized.Split(Del);
            AvatarSyncMessage msg = new AvatarSyncMessage()
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

        public void Apply()
        {
            AvatarNPCCommander av = (AvatarNPCCommander)Commander.Find(n);
            av.transform.position = ((Vector3)p) / 1000;
            Vector3 ea = av.transform.eulerAngles;
            ea.y = y/1000;
            av.transform.eulerAngles = ea;
            av.AnimatorSit = s;
            av.animator.SetFloat("Forward", ((float)f) / 1000);
            av.animator.SetFloat("Turn", ((float)r) / 1000);
            av.animator.SetFloat("Variant", ((float)v) / 1000);
        }


        public bool IsEqual(object obj)
        {
            AvatarSyncMessage o = (AvatarSyncMessage)obj;
            return f == o.f &&
                r == o.r &&
                Mathf.Abs (v -= o.v) < 1000 &&
                n == o.n &&
                s == o.s &&
                Mathf.Abs( y- o.y) < 1000 &&
                (p - o.p).magnitude < 100;
        }
    }
}