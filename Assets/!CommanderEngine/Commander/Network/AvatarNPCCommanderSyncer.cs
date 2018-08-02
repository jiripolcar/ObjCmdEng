using CommanderEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine.Network
{
    public class AvatarNPCCommanderSyncer : CommanderSyncer
    {
        [SerializeField] private AvatarNPCCommander avatarNPC;
        private const string Sit = "SI";
        private const string StandUp = "SU";
        private const string Pos = "PS";


        private void Start()
        {
            lastSyncedPosition = transform.position;
            lastSyncedYaw = transform.eulerAngles.y;
        }

        private void Reset()
        {
            avatarNPC = GetComponent<AvatarNPCCommander>();
            avatarNPC.syncer = this;
        }

        public void SendSyncSit(SeatControl seatControl)
        {
            if (Configuration.Data.startAsServer)
                NetworkCommander.SendSyncMessage(name + Del + Sit + Del + seatControl.gameObject.GetObjectIdentifier().identifyAs);
        }
        public void SendSyncStandUp()
        {
            if (Configuration.Data.startAsServer)
                NetworkCommander.SendSyncMessage(name + Del + StandUp);
        }

        private const float SyncPosInterval = 0.25f;
        private float lastSyncPos = 0;
        private Vector3 lastSyncedPosition;
        private const float LeastPositionSyncDistance = 0.001f;
        private float lastSyncedYaw;
        private Vector3 pos { get { return transform.position; } set { transform.position = value; } }
        private float yaw { get { return transform.eulerAngles.y; } set { Vector3 ea = transform.eulerAngles; ea.y = value; transform.eulerAngles = ea; } }
        private IEnumerator PositionSyncer()
        {

            yield return new WaitForSeconds(1);
            if (NetworkCommander.IsServer)
            {
                while (true)
                {
                    if (lastSyncPos < 0)
                    {
                        lastSyncPos = SyncPosInterval;
                        if ((lastSyncedPosition - pos).magnitude > LeastPositionSyncDistance)
                            NetworkCommander.SendSyncMessage(name + Del + Pos + Del + pos.x.ToString("0.00") + Del + pos.y.ToString("0.00") + Del + pos.z.ToString("0.00") + Del + yaw.ToString("0.00"));
                    }
                    lastSyncPos -= Time.deltaTime;
                    yield return 0;
                }
            }

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
                    avatarNPC.StartCoroutine(avatarNPC.Sit(ObjectIdentifier.Find(buffer[5]).seatControl));
                    break;
                case StandUp:
                    avatarNPC.StartCoroutine(avatarNPC.StandUp());
                    break;
                case Pos:
                    Vector3 targetPos = new Vector3(float.Parse(buffer[2]), float.Parse(buffer[3]), float.Parse(buffer[4]));
                    float yaw = float.Parse(buffer[4]);
                    StartCoroutine(Movements.TurnTowardsDuration(gameObject, yaw, SyncPosInterval));
                    StartCoroutine(Movements.MoveDuration(gameObject, pos, targetPos, SyncPosInterval));
                    break;
                default:
                    Debug.Log("Cannot determine");
                    break;
            }
        }



    }
}