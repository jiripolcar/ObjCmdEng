using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConsoleLog;

namespace CommanderEngine
{
    public partial class AvatarNPCCommander : CharacterCommander
    {
        private bool AnimatorSit { get { return animator.GetBool("Sitting"); } set { animator.SetBool("Sitting", value); } }
        private bool IsAnimatorInTransition { get { return animator.GetBool("isInTransition"); } }
        [Range(0, 1)] [SerializeField] private float animationVariation = 0;

        protected const float animationVariatorDurationCoefficient = 5;

        protected override IEnumerator Commit(Command command)
        {
            if (command is WalkCommand)
            {
                yield return StartCoroutine(Walk((WalkCommand)command));
            }
            else if (command is SitCommand)
            {
                SitCommand sitCommand = (SitCommand)command;
                yield return StartCoroutine(SitDown(sitCommand.target));
            }
            else if (command is StandUpCommand)
            {
                yield return StartCoroutine(StandUp());
            }
            else
            {
                Log.Write("Command is not supported by AvatarCommander! " + command);
            }
        }


        protected virtual IEnumerator AnimationVariator()
        {
            float nextVariation = Random.Range(0.5f, 2f) * animationVariatorDurationCoefficient;
            float intended = Random.Range(0f, 1f);
            while (true)
            {
                if (nextVariation < 0 && !Busy)
                {
                    nextVariation = 5;//Random.Range(0.5f, 2f) * animationVariatorDurationCoefficient;
                    intended = Random.Range(0f, 1f);
                }
                else if (Busy)
                {
                    nextVariation = -1;
                    intended = 0;
                }

                nextVariation -= Time.deltaTime;

                if (Mathf.Abs(intended - animationVariation) > 0.001f)
                {
                    animationVariation += Time.deltaTime / animationVariatorDurationCoefficient * Mathf.Sign(intended - animationVariation);
                }

                animator.SetFloat("Variant", animationVariation);

                yield return null;

            }
        }

    }





}