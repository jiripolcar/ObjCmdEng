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
                yield return StartCoroutine(CommitWalk((WalkCommand)command));
            }
            else if (command is SitCommand)
            {
                SitCommand sitCommand = (SitCommand)command;
                yield return StartCoroutine(CommitSit(sitCommand));
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

        protected virtual IEnumerator CommitWalk(WalkCommand cmd)
        {
            // if sitting, must standup
            if (SeatState != null)
            {
                // is this neccessary?
                Log.Write(name + " received Walk, but is sitting. StandingUp.", LogRecordType.Commander);
                yield return StartCoroutine(StandUp());

            }
            yield return StartCoroutine(Walk(cmd));
        }

        protected virtual IEnumerator CommitSit(SitCommand cmd)
        {
            // already sitting somewhere
            if (SeatState != null)
            {
                // if sitting on the place where supposed to sit, do nothing
                if (SeatState == cmd.target)
                {
                    Log.Write(name + " received Sit at " + cmd.target.gameObject.GetObjectIdentifier().identifyAs + " but already sitting there.", LogRecordType.Commander);
                    yield break;
                }
                // otherwise standup and go to the seat
                else
                    yield return StartCoroutine(StandUp());
            }

            // check distance, if too far, walk there
            float distance = (transform.position - cmd.target.ConstraintStandUp.transform.position).magnitude;
            if (distance > Command.DefaultStoppingDistance)
            {
                Log.Write(name + " received Sit at " + cmd.target.gameObject.GetObjectIdentifier().identifyAs + " and is too far (" + distance.ToString("0.00") + "). Going there.", LogRecordType.Commander);
                yield return StartCoroutine(Walk(cmd, cmd.target.ConstraintStandUp, WalkCommand.WalkCommandEndingStyle.None, false, 0.1f, false));
            }

            yield return StartCoroutine(Sit(cmd.target));
        }

        protected virtual IEnumerator CommitStandUp(StandUpCommand cmd)
        {
            if (SeatState != null)
                yield return StartCoroutine(StandUp());
            else
                Log.Write(name + " received StandUp, already standing.", LogRecordType.Commander);
        }

        protected virtual IEnumerator AnimationVariator()
        {
            float nextVariation = Random.Range(0.5f, 2f) * animationVariatorDurationCoefficient;
            float intended = Random.Range(0f, 1f);
            while (true)
            {
                if (Busy)
                {
                    nextVariation = -1;
                    intended = 0;
                }
                else if (nextVariation < 0)
                {
                    nextVariation = Random.Range(0.5f, 2f) * animationVariatorDurationCoefficient;
                    intended = Random.Range(0f, 1f);
                }
                else
                    nextVariation -= Time.deltaTime;

                if (Mathf.Abs(intended - animationVariation) > 0.001f)
                {
                    animationVariation += Time.deltaTime / animationVariatorDurationCoefficient * Mathf.Sign(intended - animationVariation);
                    animator.SetFloat("Variant", animationVariation);
                }
                yield return null;
            }
        }

    }





}