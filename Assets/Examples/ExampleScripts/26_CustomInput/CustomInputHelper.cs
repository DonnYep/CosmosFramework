using UnityEngine;
using Cosmos.Input;
using Cosmos;

public class CustomInputHelper : IInputHelper
{
    IInputManager inputManager = CosmosEntry.InputManager;
    public void OnInitialization()
    {
        inputManager.RegisterVirtualButton(CustomButtonName._Space);
        inputManager.RegisterVirtualButton(CustomButtonName._W);
        inputManager.RegisterVirtualButton(CustomButtonName._A);
        inputManager.RegisterVirtualButton(CustomButtonName._S);
        inputManager.RegisterVirtualButton(CustomButtonName._D);
    }

    public void OnRefresh()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            inputManager.SetButtonDown(CustomButtonName._Space);
        else if (UnityEngine.Input.GetKeyUp(KeyCode.Space))
            inputManager.SetButtonUp(CustomButtonName._Space);

        if (UnityEngine.Input.GetKeyDown(KeyCode.W))
            inputManager.SetButtonDown(CustomButtonName._W);
        else if (UnityEngine.Input.GetKeyUp(KeyCode.W))
            inputManager.SetButtonUp(CustomButtonName._W);

        if (UnityEngine.Input.GetKeyDown(KeyCode.A))
            inputManager.SetButtonDown(CustomButtonName._A);
        else if (UnityEngine.Input.GetKeyUp(KeyCode.A))
            inputManager.SetButtonUp(CustomButtonName._A);

        if (UnityEngine.Input.GetKeyDown(KeyCode.S))
            inputManager.SetButtonDown(CustomButtonName._S);
        else if (UnityEngine.Input.GetKeyUp(KeyCode.S))
            inputManager.SetButtonUp(CustomButtonName._S);

        if (UnityEngine.Input.GetKeyDown(KeyCode.D))
            inputManager.SetButtonDown(CustomButtonName._D);
        else if (UnityEngine.Input.GetKeyUp(KeyCode.D))
            inputManager.SetButtonUp(CustomButtonName._D);

    }
    public void OnTermination()
    {
        inputManager.DeregisterVirtualButton(CustomButtonName._Space);
        inputManager.DeregisterVirtualButton(CustomButtonName._W);
        inputManager.DeregisterVirtualButton(CustomButtonName._A);
        inputManager.DeregisterVirtualButton(CustomButtonName._S);
        inputManager.DeregisterVirtualButton(CustomButtonName._D);

    }
}
