using System;

namespace Cosmos.Procedure
{
    public class ProcedureNodeAddedEventArgs : GameEventArgs
    {
        public Type AddedProcedureNodeType { get; private set; }
        public string AddedProcedureNodeName { get; private set; }
        public override void Release()
        {
            AddedProcedureNodeType = null;
            AddedProcedureNodeName = string.Empty;
        }
        public static ProcedureNodeAddedEventArgs Create(Type type)
        {
            var eventArgs = ReferencePool.Acquire<ProcedureNodeAddedEventArgs>();
            eventArgs.AddedProcedureNodeType= type;
            if (type != null)
                eventArgs.AddedProcedureNodeName= type.Name;
            return eventArgs;
        }
        public static void Release(ProcedureNodeAddedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
