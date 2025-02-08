using Cosmos.Input;
using UnityEngine;
using Protocol;
using MessagePack;
using MessagePack.Resolvers;
namespace Cosmos.Lockstep
{
    /// <summary>
    /// 帧同步输入组件；
    /// </summary>
    public class MultiplayInputComponent : MonoBehaviour
    {
        IInputManager inputManager;
        IMultiplayManager multiplayManager;
        int sendInterval { get { return MultiplayConfig.Instance.SendIntervalMS; } }
        long latestTime;
        [SerializeField] float walkSpeed = 1.8f;// 1.5f;
        [SerializeField] float runSpeed = 5;//5;
        [SerializeField] float rotSpeed = 20;
        float currentSpeed;
        void Awake()
        {
            inputManager = GameEntry.InputManager;
            multiplayManager = GameEntry.MultiplayManager;
            latestTime = Utility.Time.MillisecondNow();
        }
        void Update()
        {
            var v = inputManager.GetAxis(InputAxisType._Vertical);
            var h = inputManager.GetAxis(InputAxisType._Horizontal);
            Vector3 input = new Vector3(h, 0, v);
            OpInput opInput = new OpInput();
            if (inputManager.GetButton(InputButtonType._LeftShift))
            {
                opInput.LeftShiftDown = true;
                currentSpeed = runSpeed;
            }
            else
                currentSpeed = walkSpeed;

            if (inputManager.GetButtonDown(InputButtonType._MouseLeft))
            {
                opInput.MouseLeftDown = true;
                opInput.Conv = multiplayManager.AuthorityConv;
                var bytes = MessagePackSerializer.Serialize(opInput);
                multiplayManager.SendInputData(bytes);
            }

            var now = Utility.Time.MillisecondNow();
            opInput.Position = transform.position + Vector3.Normalize(input)*currentSpeed;
            opInput.Forward= Vector3.Normalize(input);
            opInput.Rotation = transform.eulerAngles;
            opInput.Input = input;
            if (now >= latestTime)
            {
                latestTime = now + sendInterval;
                opInput.Conv = multiplayManager.AuthorityConv;
                var bytes = MessagePackSerializer.Serialize(opInput);
                multiplayManager.SendInputData(bytes);
            }
        }
    }
}