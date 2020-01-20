using UnityEngine;
using System.Collections;
namespace Cosmos
{
    public class FSMStateBase
    {
        public int ID { get; set; }
        public FSMStateBase(int id)
        {
            this.ID = id;
        }
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnStay() { }
    }
}