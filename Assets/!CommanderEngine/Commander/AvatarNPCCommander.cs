using CommanderEngine.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CommanderEngine
{
    [System.Serializable]
    public class AvatarTargets
    {
        public Transform bottom, leftFoot, rightFoot;
    }

    public partial class AvatarNPCCommander : CharacterCommander
    {
        [SerializeField] internal Animator animator;
        [SerializeField] private AvatarTargets avatarTargets;
        private AvatarSyncer avSyncer { get { return (AvatarSyncer)syncer; } }

        private void Reset()
        {
            AvatarCommanderReset();
        }

        protected void AvatarCommanderReset()
        {
            CommanderReset();
            CharacterCommanderReset();
            if (!animator)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
            animationVariator = StartCoroutine(AnimationVariator());
        }
    }
}