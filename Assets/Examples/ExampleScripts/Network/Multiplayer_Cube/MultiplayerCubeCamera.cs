using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Test
{
    public class MultiplayerCubeCamera : MonoBehaviour
    {
        Vector3 originalWorldPos;
        Vector3 originalOffset;
        Transform target;
        public void ReleaseTarget()
        {
            this.target = null;
            transform.position = originalWorldPos;
        }
        public void SetCameraTarget(Transform target)
        {
            this.target = target;
        }
        private void Start()
        {
            originalWorldPos = transform.position;
            originalOffset = transform.position - Vector3.zero;
        }
        private void LateUpdate()
        {
            if (!Utility.Assert.IsNull(target))
                transform.position = Vector3.Lerp(transform.position, target.position+ originalOffset, Time.deltaTime * 10);
        }
    }
}
