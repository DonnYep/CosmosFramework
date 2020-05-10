using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 所有可操控角色的基类
    /// </summary>
    public abstract class Actor : MonoBehaviour, IControllable
    {
        public abstract void OnPause();
        public abstract void OnUnPause();
    }
}