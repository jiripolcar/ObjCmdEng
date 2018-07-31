using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine.Network
{
    public class AvatarNPCCommanderSyncer : CommanderSyncer
    {
        [SerializeField] private AvatarNPCCommander avatarNPC;
        private const string Sit = "SIT";
        private const string StandUp = "STANDUP";

                
        public void SendSyncSit(SeatControl seatControl)
        {
            NetworkCommander.SendSyncMessage(name + Del + Sit + Del + seatControl.gameObject.GetObjectIdentifier().identifyAs);
        }
        public void SendSyncStandUp()
        {
            NetworkCommander.SendSyncMessage(name + Del + StandUp);
        }

        private void Reset()
        {
            avatarNPC = GetComponent<AvatarNPCCommander>();
            avatarNPC.syncer = this;
        }

        public override void ReceiveSync(string syncString)
        {
            string x = "Determining\n";
            string[] buffer = syncString.Split(Del);
            foreach (string b in buffer)
                x += b + "\n";
            Debug.Log(x);

            switch (buffer[1])
            {
                case Sit:
                    avatarNPC.Sit(ObjectIdentifier.Find(buffer[2]).seatControl);
                    break;
                case StandUp:
                    avatarNPC.StandUp();
                    break;
            }
        }

    }
}