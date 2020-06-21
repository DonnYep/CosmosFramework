using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    public abstract class MonoObjectBase : MonoBehaviour, IObject
    {
        public abstract Type Type { get; }
        public virtual void OnDespawn() { }
        public virtual void OnInitialization() { }
        public virtual void OnSpawn() { }
        public virtual void OnTermination() { }
    }
}