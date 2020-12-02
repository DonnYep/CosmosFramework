using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.Input;
using Cosmos.Controller;

public class ControllerOwner : MonoBehaviour
{
    [SerializeField] GameObject controller;
    private void Awake()
    {
        GameManager.GetModule<IInputManager>().SetInputDevice(new StandardInputDevice());
    }
    void Start()
    {
        TempController c=default;
        if (controller != null)
        {
            c= controller.GetComponent<TempController>();
        }
        GameManager.GetModule<IControllerManager>().RegisterController(typeof(TempController), c);
    }
}
