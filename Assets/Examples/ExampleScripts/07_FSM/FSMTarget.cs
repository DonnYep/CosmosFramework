using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMTarget : MonoBehaviour
{
    [SerializeField] float offset = 20;
    [SerializeField] float moveSpeed = 5;
    int dirAbs = 1;
    float startZPos;
    void Start()
    {
        startZPos = transform.position.z;
    }
    void Update()
    {
        transform.Translate(Vector3.forward * dirAbs * moveSpeed * Time.deltaTime);
        if (System.Math.Abs(transform.position.z - startZPos) <= -offset || System.Math.Abs(transform.position.z - startZPos) >= offset)
        { dirAbs = -dirAbs; }
    }
}