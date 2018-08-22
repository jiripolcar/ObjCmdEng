using System.Collections.Generic;

namespace CommanderEngine.Network
{


    [System.Serializable]
    public class NetworkSyncData
    {
        public const char Del = ';';
        public List<AvatarSyncMessage> avatarNPCSync = new List<AvatarSyncMessage>();
        public List<Command> commandSync = new List<Command>();

        private AvatarSyncMessage PullAsm()
        {
            if (avatarNPCSync.Count > 0)
            {
                AvatarSyncMessage msg = avatarNPCSync[0];
                avatarNPCSync.RemoveAt(0);
                return msg;
            }
            else return null;
        }

        private Command PullCmd()
        {
            if (commandSync.Count > 0)
            {
                Command cmd = commandSync[0];                
                commandSync.RemoveAt(0);
                return cmd;
            }
            else return null;
        }

        public string Serialize()
        {
            string ser = (avatarNPCSync.Count).ToString() + Del + (commandSync.Count).ToString();
            AvatarSyncMessage asm = PullAsm();
            while (asm != null)
            {
                ser += Del + asm.Serialize();
                asm = PullAsm();
            }
            Command cmd = PullCmd();
            while (cmd != null)
            {
                ser += Del + cmd.ToString();
                cmd = PullCmd();
            }
            return ser;
        }

        public static NetworkSyncData Deserialize(string serialized)
        {
            string[] b = serialized.Split(Del);


            NetworkSyncData l = new NetworkSyncData();
            l.avatarNPCSync = new List<AvatarSyncMessage>();
            l.commandSync = new List<Command>();

            int avatarSyncs = int.Parse(b[0]);
            int commandSyncs = int.Parse(b[1]);

            int avatarSyncsEnd = 2 + avatarSyncs;
            int commandSyncsEnd = avatarSyncsEnd + commandSyncs;

            int i = 2;
            for (; i < avatarSyncsEnd; i++)
            {
                l.avatarNPCSync.Add(AvatarSyncMessage.FromString(b[i]));
            }
            for (; i < avatarSyncsEnd; i++)
            {
                l.commandSync.Add(Command.FromString(b[i]));
            }
            return l;
        }

        public void ApplyAllAvatarSyncs()
        {
            avatarNPCSync.ForEach((msg) => msg.Apply());
        }

        public void DoAllCommands()
        {
            commandSync.ForEach((cmd) => Commander.Do(cmd));
        }
    }
}