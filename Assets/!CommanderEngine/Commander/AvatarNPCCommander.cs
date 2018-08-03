using CommanderEngine.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CommanderEngine
{
    [System.Serializable] public class AvatarTargets
    {
        public Transform bottom, leftFoot, rightFoot;
    }

    public partial class AvatarNPCCommander : CharacterCommander
    {
        [SerializeField] internal Animator animator;
        [SerializeField] private AvatarTargets avatarTargets;
        private AvatarNPCCommanderSyncer avSyncer { get { return (AvatarNPCCommanderSyncer)syncer; } }

        private void Reset()
        {            
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

            if (!animator)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
            StartCoroutine(AnimationVariator());
        }
    }
}