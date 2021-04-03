using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Test
{
    [RequireComponent(typeof( Rigidbody))]
    public class SimulatePlayerController:MonoBehaviour
    {
        Rigidbody m_Rigidbody;
        [SerializeField] float moveSpeed = 5;
        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            var h = CosmosEntry.InputManager.GetAxis(InputAxisType._Horizontal);
            var v = CosmosEntry.InputManager.GetAxis(InputAxisType._Vertical);
            transform.position += new Vector3(h, 0, v)*Time.deltaTime*moveSpeed;
        }
    }
}
