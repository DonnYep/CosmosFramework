using UnityEngine;
using System.Collections;
namespace Cosmos
{
    public class EventCenterDispatcher : MonoBehaviour, Event.IEventDispatcher
    {
        [SerializeField] string eventKey;
        public string DispatcherName { get { return this.gameObject.name; } }
        public void DispatchEvent()
        {
            //Facade.Instance.DispatchEvent(eventKey,)
        }
        public void RegisterEvent()
        {
            Facade.Instance.RegisterEvent(eventKey);
        }
    }
}