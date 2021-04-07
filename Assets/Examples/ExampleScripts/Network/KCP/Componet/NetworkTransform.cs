using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class NetworkTransform : NetworkBehaviour
    {
        DataPoint start;
        DataPoint goal;
        public override NetworkdComponetType NetworkdComponetType { get; set; } = NetworkdComponetType.Trasform;
        public class DataPoint
        {
            public float timeStamp;
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;
            public float movementSpeed;
        }
        #region NetworkTransform
        public void DeserializeNetworkTransform(FixTransform fixTransform)
        {
            DataPoint temp = new DataPoint
            {
                localPosition = fixTransform.Position.GetVector(),
                localRotation = Quaternion.Euler(fixTransform.Rotation.GetVector()),
                localScale = fixTransform.Scale.GetVector(),
                timeStamp = Time.time
            };
            if (start == null)
            {
                start = new DataPoint
                {
                    timeStamp = Time.time - NetworkSimulateConsts.SyncInterval,
                    localPosition = transform.localPosition,
                    movementSpeed = temp.movementSpeed
                };
            }
            start = goal;

            temp.movementSpeed = EstimateMovementSpeed(goal, temp, transform, NetworkSimulateConsts.SyncInterval);

            goal = temp;
        }
        public void SerializeNetworkTransform(out FixTransform fixTransform)
        {
            fixTransform = new FixTransform(transform.position, transform.rotation.eulerAngles, transform.localScale);
        }
        #endregion
        public static void SerializeIntoWriter(NetworkWriter writer, Vector3 position)
        {
            writer.WriteVector3(position);
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            DataPoint temp = new DataPoint
            {
                localPosition = reader.ReadVector3(),
            };
            temp.movementSpeed = EstimateMovementSpeed(goal, temp, transform, NetworkSimulateConsts.SyncInterval);
        }
        private void Update()
        {
            if (!IsAuthority)
            {
                if (goal != null)
                {
                    if (NeedsTeleport())
                    {
                        ApplyTransform(goal.localPosition,goal.localRotation,goal.localScale);
                        start = null;
                        goal = null;
                    }
                    else
                    {
                        ApplyTransform(InterpolatePosition(start, goal, transform.localPosition),
                            InterpolateRotation(start,goal,transform.rotation),
                            InterpolateScale(start,goal,transform.localScale)
                            );
                    }
                }
            }
        }
        bool NeedsTeleport()
        {
            float startTime = start != null ? start.timeStamp : Time.time - NetworkSimulateConsts.SyncInterval;
            float goalTime = goal != null ? goal.timeStamp : Time.time;
            float difference = goalTime - startTime;
            float timeSinceGoalReceived = Time.time - goalTime;
            return timeSinceGoalReceived > difference * 5;
        }
        Vector3 InterpolateScale(DataPoint start, DataPoint goal, Vector3 currentScale)
        {
            if (start != null )
            {
                float t = CurrentInterpolationFactor(start, goal);
                return Vector3.Lerp(start.localScale, goal.localScale, t);
            }
            else
            {
                return currentScale;
            }
        }
        void ApplyTransform(Vector3 position,Quaternion rotation,Vector3 scale)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
            transform.localScale = scale;
        }
        static Vector3 InterpolatePosition(DataPoint start, DataPoint goal, Vector3 currentPosition)
        {
            if (start != null)
            {
                float speed = Mathf.Max(start.movementSpeed, goal.movementSpeed);
                return Vector3.MoveTowards(currentPosition, goal.localPosition, speed * Time.deltaTime);
            }
            return currentPosition;
        }
        static Quaternion InterpolateRotation(DataPoint start, DataPoint goal, Quaternion defaultRotation)
        {
            if (start != null)
            {
                float t = CurrentInterpolationFactor(start, goal);
                return Quaternion.Slerp(start.localRotation, goal.localRotation, t);
            }
            return defaultRotation;
        }
        static float CurrentInterpolationFactor(DataPoint start, DataPoint goal)
        {
            if (start != null)
            {
                float difference = goal.timeStamp - start.timeStamp;

                // the moment we get 'goal', 'start' is supposed to
                // start, so elapsed time is based on:
                float elapsed = Time.time - goal.timeStamp;
                // avoid NaN
                return difference > 0 ? elapsed / difference : 0;
            }
            return 0;
        }
        static float EstimateMovementSpeed(DataPoint from, DataPoint to, Transform transform, float sendInterval)
        {
            Vector3 delta = to.localPosition - (from != null ? from.localPosition : transform.localPosition);
            float elapsed = from != null ? to.timeStamp - from.timeStamp : sendInterval;
            // avoid NaN
            return elapsed > 0 ? delta.magnitude / elapsed : 0;
        }
    }
}