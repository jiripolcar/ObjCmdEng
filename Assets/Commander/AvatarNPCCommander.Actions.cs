using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class AvatarNPCCommander : CharacterCommander
    {
        protected override IEnumerator Walk(Command cmd, Transform destination, WalkCommand.WalkCommandEndingStyle endStyle,
            bool updatePosition = false, float stoppingDistance = Command.DefaultStoppingDistance, bool precisionAlignAtEnd = false, float? maxSpeed = null
            )
        {
            float speed = maxSpeed == null ? (cmd is WalkCommand ? ((WalkCommand)cmd).speed : Command.DefaultWalkSpeed) : maxSpeed.Value;

            if (nmObstacle.enabled || !nmAgent.enabled)
            {
                nmObstacle.enabled = false;
                yield return 0;
                nmAgent.enabled = true;
            }

            List<Vector3> stuckWatch = new List<Vector3>() { transform.position };
            int stucks = 0;
            float walkWatchTime = WalkUpdateInterval;

            Vector3 targetPosition = destination.position;
            nmAgent.SetDestination(targetPosition);
            nmAgent.updatePosition = false;

            while (((targetPosition - transform.position).Magnitude2D() > stoppingDistance) && (cmd == null ? true : cmd.State == CommandState.Pending))
            {
                walkWatchTime -= Time.deltaTime;
                if (walkWatchTime < 0)
                {
                    walkWatchTime += WalkUpdateInterval;
                    if (updatePosition || (targetPosition - destination.position).magnitude > WalkTargetPositionUpdateThresholdDistance)
                    {
                        targetPosition = destination.position;
                        nmAgent.SetDestination(targetPosition);
                    }
                    stuckWatch.Add(transform.position);
                    if (stuckWatch.Count > WalkStuckWatchListLength * 2)
                    {
                        if (stuckWatch.PathLength() < WalkStuckWatchListLength * WalkUpdateInterval * 0.25f)
                            stucks++;
                        else stucks = 0;
                        stuckWatch.RemoveAt(0);
                    }
                    if (stucks > WalkStuckWatchListLength)
                    {
                        ConsoleLog.Log.Write(name + " STUCK at " + transform.position + " dist in " + (WalkStuckWatchListLength * WalkUpdateInterval).ToString("0.00") + " is " + stuckWatch.PathLength().ToString("0.000"));
                        break;
                    }
                }
                nmAgent.nextPosition = transform.position;
                UpdateAnimatorWalking(nmAgent.desiredVelocity, speed);
                yield return 0;
            }
            print("S");
            nmAgent.isStopped = true;


            float lerp = 0;
            Vector3 beforeAligning = transform.position;
            float startEAy = transform.eulerAngles.y;
            float endEAy;
            if (endStyle == WalkCommand.WalkCommandEndingStyle.AlignWith)
                endEAy = destination.eulerAngles.y;
            else if (endStyle == WalkCommand.WalkCommandEndingStyle.Face)
            {
                GameObject atLooker = new GameObject(name + "'s atLooker");
                atLooker.transform.position = beforeAligning;
                atLooker.transform.LookAt(destination);
                endEAy = atLooker.transform.eulerAngles.y;
                Destroy(atLooker, 0.2f);
            }
            else
                endEAy = startEAy;

            while (lerp < 1 && (cmd == null ? true : cmd.State == CommandState.Pending))
            {
                lerp += Time.deltaTime / walkAlignDuration;
                animator.SetFloat("Forward", 0, walkAlignDuration, Time.deltaTime);
                animator.SetFloat("Turn", 0, walkAlignDuration, Time.deltaTime);

                if (precisionAlignAtEnd) transform.position = Vector3.Lerp(beforeAligning, destination.position, lerp);
                if (endStyle != WalkCommand.WalkCommandEndingStyle.None)
                {
                    Vector3 ea = transform.eulerAngles;
                    ea.y = Mathf.LerpAngle(startEAy, endEAy, lerp);
                    transform.eulerAngles = ea;
                }
                yield return 0;
            }

            nmAgent.enabled = false;
            animator.SetFloat("Forward", 0);
            animator.SetFloat("Turn", 0);
            yield return 0;
            nmObstacle.enabled = true;


        }

        private void UpdateAnimatorWalking(Vector3 move, float speed)
        {
            if (move.magnitude > speed) { move.Normalize(); move *= speed; }
            move = transform.InverseTransformDirection(move);
            Vector3 groundNormal = Vector3.up;
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, 1))
                groundNormal = hitInfo.normal;
            move = Vector3.ProjectOnPlane(move, groundNormal);
            float turnAmount = Mathf.Atan2(move.x, move.z);
            float forwardAmount = move.z;
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        }



    }
}