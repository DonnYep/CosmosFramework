using System.Collections.Generic;
using UnityEngine;

public class PathfindingPlayer : MonoBehaviour
{
    Queue<Vector3> ptrQueue = new Queue<Vector3>();

    bool canMove;
    Vector3 nextPoint;
    [SerializeField] float stopDistance = 0.1f;
    [SerializeField]float speed = 3;
    public void SetPath(IList<Vector3> pathPointers)
    {
        ptrQueue.Clear();
        var length = pathPointers.Count;
        for (int i = 0; i < length; i++)
        {
            ptrQueue.Enqueue(pathPointers[i]);
        }
        canMove = true;
    }
    void Update()
    {
        if (canMove)
        {
            var step = speed* Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, nextPoint, step);
            if (Vector3.Distance(transform.position, nextPoint) <= stopDistance)
            {
                nextPoint = GetNextPoint();
            }
        }
    }
    Vector3 GetNextPoint()
    {
        if (ptrQueue.Count > 0)
        {
            var ptr = ptrQueue.Dequeue();
            return ptr;
        }
        else
        {
            canMove = false;
            return transform.position;
        }
    }
}
