using UnityEngine;
using Cosmos;
using Cosmos.Input;

public class CustomButtonSubscriber : MonoBehaviour
{
    IInputManager inputManager;
    void Start()
    {
        inputManager = CosmosEntry.InputManager;
        //演示：替换为自定义绑定表（Space/W/A/S/D → 同名虚拟按键）
        inputManager.SetInputHelper(new CustomInputHelper());
    }
    void Update()
    {
        if (inputManager.GetButtonDown(CustomButtonName._Space))
        {
            Utility.Debug.LogInfo($"{CustomButtonName._Space} button down");
        }
        if (inputManager.GetButtonDown(CustomButtonName._W))
        {
            Utility.Debug.LogInfo($"{CustomButtonName._W} button down");
        }
        if (inputManager.GetButtonDown(CustomButtonName._A))
        {
            Utility.Debug.LogInfo($"{CustomButtonName._A} button down");
        }
        if (inputManager.GetButtonDown(CustomButtonName._S))
        {
            Utility.Debug.LogInfo($"{CustomButtonName._S} button down");
        }
        if (inputManager.GetButtonDown(CustomButtonName._D))
        {
            Utility.Debug.LogInfo($"{CustomButtonName._D} button down");
        }
    }
}
