using System;
namespace Cosmos.Event
{
    public static class EventObserverExtensions
    {
        public static void AddListener(this IEventObserver @this, string eventKey, EventHandler<GameEventArgs> handler)
        {
            GameManager.GetModule<IEventManager>().AddListener(eventKey, handler);
        }
        public static void RemoveListener(this IEventObserver @this, string eventKey, EventHandler<GameEventArgs> handler)
        {
            GameManager.GetModule<IEventManager>().RemoveListener(eventKey, handler);
        }
        public static void DispatchEvent(this IEventObserver @this, string eventKey, object sender, GameEventArgs args)
        {
            GameManager.GetModule<IEventManager>().DispatchEvent(eventKey, sender, args);
        }
        public static void DispatchEvent(this IEventObserver @this, string eventKey, GameEventArgs args)
        {
            GameManager.GetModule<IEventManager>().DispatchEvent(eventKey, null, args);
        }
        public static void DispatchEvent(this IEventObserver @this, string eventKey)
        {
            GameManager.GetModule<IEventManager>().DispatchEvent(eventKey, null, null);
        }
    }
}
