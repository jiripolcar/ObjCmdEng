using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CommanderEngine
{
    public partial class CharacterCommander : Commander
    {
        protected const int WalkStuckWatchListLength = 8;
        protected const float WalkUpdateInterval = 0.25f;
        protected const float WalkTargetPositionUpdateThresholdDistance = 0.1f;

        [SerializeField] protected NavMeshAgent nmAgent;
        [SerializeField] protected NavMeshObstacle nmObstacle;
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] private SeatControl _seatState;
        public virtual SeatControl SeatState
        {
            get
            {
                return _seatState;
            }
            set
            {
                if (value)
                    previousSeatState = value;
                _seatState = value;
            }
        }
        public SeatControl previousSeatState { get; private set; }
        [SerializeField] protected float walkAlignDuration = 0.33f;

        private void Reset()
        {
            CharacterCommanderReset();
        }

        protected void CharacterCommanderReset()
        {
            CommanderReset();
            nmAgent = gameObject.GetComponent<NavMeshAgent>();
            if (!nmAgent)
                nmAgent = gameObject.AddComponent<NavMeshAgent>();
            nmAgent.enabled = false;

            nmObstacle = gameObject.GetComponent<NavMeshObstacle>();
            if (!nmObstacle)
                nmObstacle = gameObject.AddComponent<NavMeshObstacle>();
            nmObstacle.enabled = true;

            audioSource = gameObject.GetComponent<AudioSource>();
            if (!audioSource)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public override string ToString()
        {
            return name + "(Character)";
        }

    }
}