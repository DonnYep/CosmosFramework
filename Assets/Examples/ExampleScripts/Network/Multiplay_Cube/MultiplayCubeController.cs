using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos.Input;
namespace Cosmos.Test
{
    [RequireComponent(typeof(Rigidbody))]
    public class MultiplayCubeController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5;
        [SerializeField] float rotSpeed = 10;
        private void Update()
        {
            var h = CosmosEntry.InputManager.GetAxis(InputAxisType._Horizontal);
            var v = CosmosEntry.InputManager.GetAxis(InputAxisType._Vertical);
            var pos = new Vector3(h, 0, v);
            if (pos != Vector3.zero)
            {
                if (transform.forward != pos)
                {
                    transform.forward = Vector3.Slerp(transform.forward, Vector3.Normalize(pos), Time.deltaTime * rotSpeed);
                }
                transform.position += Vector3.Normalize(pos) * Time.deltaTime * moveSpeed;
            }
        }
    }
}
