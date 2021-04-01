using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class NetworkTransform : NetworkBehaviour
    {
        DataPoint start;
        DataPoint goal;

        public class DataPoint
        {
            public float timeStamp;
            public Vector3 localPosition;
            public float movementSpeed;
        }
        public static void SerializeIntoWriter(NetworkWriter writer, Vector3 position)
        {
            writer.WriteVector3(position);
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            DataPoint temp = new DataPoint
            {
                localPosition = reader.ReadVector3()
            };
            temp.movementSpeed = EstimateMovementSpeed(goal, temp, transform, NetworkSimulateConsts.SyncInterval);
        }
        private void Update()
        {
            if (!IsAuthority)
            {
                InterpolatePosition(start, goal, transform.localPosition);
            }
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
        static float EstimateMovementSpeed(DataPoint from, DataPoint to, Transform transform, float sendInterval)
        {
            Vector3 delta = to.localPosition - (from != null ? from.localPosition : transform.localPosition);
            float elapsed = from != null ? to.timeStamp - from.timeStamp : sendInterval;
            // avoid NaN
            return elapsed > 0 ? delta.magnitude / elapsed : 0;
        }
    }
}