using System;

namespace Cosmos.Procedure
{
    public class ProcedureNodeChangedEventArgs : GameEventArgs
    {
        public Type ExitedProcedureNodeType { get; private set; }
        public string ExitedProcedureNodeName { get; private set; }
        public Type EnteredProcedureNodeType { get; private set; }
        public string EnteredProcedureNodeName { get; private set; }
        public override void Release()
        {
            ExitedProcedureNodeType = null;
            ExitedProcedureNodeName = string.Empty;
            EnteredProcedureNodeType = null;
            EnteredProcedureNodeName = string.Empty;
        }
        public static ProcedureNodeChangedEventArgs Create(Type exitedType, Type enteredType)
        {
            var eventArgs = ReferencePool.Acquire<ProcedureNodeChangedEventArgs>();
            eventArgs.ExitedProcedureNodeType = exitedType;
            if (exitedType != null)
                eventArgs.ExitedProcedureNodeName = exitedType.Name;
            eventArgs.EnteredProcedureNodeType = enteredType;
            if (enteredType != null)
                eventArgs.EnteredProcedureNodeName = enteredType.Name;
            return eventArgs;
        }
        public static void Release(ProcedureNodeChangedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
