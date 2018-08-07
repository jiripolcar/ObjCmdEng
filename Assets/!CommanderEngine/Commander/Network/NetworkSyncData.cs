using System.Collections.Generic;

namespace CommanderEngine.Network
{


    [System.Serializable]
    public class NetworkSyncData
    {
        public const char Del = ';';
        public List<AvatarSyncMessage> avatarNPCSync = new List<AvatarSyncMessage>();


        public AvatarSyncMessage Pull()
        {
            if (avatarNPCSync.Count > 0)
            {
                AvatarSyncMessage msg = avatarNPCSync[0];
                avatarNPCSync.RemoveAt(0);
                return msg;
            }
            else return null;
        }

        public string Serialize()
        {

            string ser = (avatarNPCSync.Count).ToString();

            AvatarSyncMessage asm = Pull();
            while (asm != null)
            {
                ser += Del + asm.Serialize();
                asm = Pull();
            }

            return ser;
        }

        public static NetworkSyncData Deserialize(string serialized)
        {
            string[] b = serialized.Split(Del);


            NetworkSyncData l = new NetworkSyncData();
            l.avatarNPCSync = new List<AvatarSyncMessage>();

            int avatarSyncs = int.Parse(b[0]);

            int i = 1;
            for (; i < avatarSyncs + 1; i++)
            {
                l.avatarNPCSync.Add(AvatarSyncMessage.FromString(b[i]));
            }
            return l;
            //return JsonUtility.FromJson<ListOfAvatarNPCCommanderSyncMessage>(json);
        }

        public void ApplyAll()
        {
            avatarNPCSync.ForEach((msg) => msg.Apply());
        }
    }
}