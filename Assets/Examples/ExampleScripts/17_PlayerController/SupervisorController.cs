using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
namespace Cosmos.Test
{
    public class SupervisorController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 20;

        private void Start()
        {
            CosmosEntry.InputManager.SetInputHelper(new StandardInputHelper());
        }
        private void Update()
        {
            var h = CosmosEntry.InputManager.GetAxis(InputAxisType._Horizontal);
            var v = CosmosEntry.InputManager.GetAxis(InputAxisType._Vertical);
            var pos = new Vector3(h, 0, v);
            if (pos != Vector3.zero)
            {
                transform.position += Vector3.Normalize(pos) * Time.deltaTime * moveSpeed;
            }
        }
    }
}
