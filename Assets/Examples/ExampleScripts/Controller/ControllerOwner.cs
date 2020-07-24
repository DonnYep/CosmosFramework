using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class ControllerOwner : MonoBehaviour
{
    [SerializeField] GameObject controller;
    private void Awake()
    {
        Facade.SetInputDevice(new StandardInputDevice());
    }
    void Start()
    {
        TempController c=default;
        if (controller != null)
        {
            c= controller.GetComponent<TempController>();
        }
        Facade.RegisterController(typeof(TempController), c);
    }
}
