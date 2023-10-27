using System;

namespace Cosmos.Procedure
{
    public class PorcedureNodeRemovedEventArgs : GameEventArgs
    {
        public Type RemovedProcedureNodeType { get; private set; }
        public string RemovedProcedureNodeName { get; private set; }
        public override void Release()
        {
            RemovedProcedureNodeType = null;
            RemovedProcedureNodeName = string.Empty;
        }
        public static PorcedureNodeRemovedEventArgs Create(Type type)
        {
            var eventArgs = ReferencePool.Acquire<PorcedureNodeRemovedEventArgs>();
            eventArgs.RemovedProcedureNodeType = type;
            if (type != null)
                eventArgs.RemovedProcedureNodeName = type.Name;
            return eventArgs;
        }
        public static void Release(PorcedureNodeRemovedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
