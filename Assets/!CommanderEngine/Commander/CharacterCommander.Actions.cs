using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class CharacterCommander : Commander
    {
        protected virtual IEnumerator Walk(WalkCommand cmd)
        {
            if (cmd.destination == null)
            {
                Debug.LogError("Has no destination: " + cmd.ToString());
                yield break;
            }
            yield return StartCoroutine(Walk(cmd, cmd.destination, cmd.endingStyle, cmd.updatePosition, cmd.stoppingDistance, cmd.lerpAtEndPrecisely));
        }

        protected virtual IEnumerator Walk(Command cmd, Transform destination, WalkCommand.WalkCommandEndingStyle endStyle,
            bool updatePosition = false, float stoppingDistance = Command.DefaultStoppingDistance, bool precisionAlignAtEnd = false, float? maxSpeed = null
            )
        {
            float speedBuffer = nmAgent.speed;
            nmAgent.speed = maxSpeed == null ? (cmd is WalkCommand ? ((WalkCommand)cmd).speed : nmAgent.speed) : maxSpeed.Value;

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

            while (((targetPosition - transform.position).Magnitude2D() > stoppingDistance) && (cmd == null ? true : cmd.State == CommandState.Pending))
            {
                walkWatchTime -= Time.deltaTime;
                if (walkWatchTime < 0)
                {
                    walkWatchTime += WalkUpdateInterval;
                    if (updatePosition && (targetPosition - destination.position).magnitude > WalkTargetPositionUpdateThresholdDistance)
                    {
                        targetPosition = destination.position;
                        nmAgent.SetDestination(targetPosition);
                    }
                    stuckWatch.Add(transform.position);
                    if (stuckWatch.Count > WalkStuckWatchListLength * 2)
                    {
                        if (stuckWatch.PathLength() < WalkStuckWatchListLength * WalkUpdateInterval * nmAgent.speed)
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

                yield return 0;
            }

            nmAgent.isStopped = true;
            nmAgent.speed = speedBuffer;

            if (precisionAlignAtEnd || endStyle != WalkCommand.WalkCommandEndingStyle.None)
            {

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
                yield return 0;
                nmObstacle.enabled = true;

            }
        }



    }
}