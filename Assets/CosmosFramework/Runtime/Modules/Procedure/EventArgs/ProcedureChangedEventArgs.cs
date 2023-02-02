using System;

namespace Cosmos.Procedure
{
    public class ProcedureChangedEventArgs : GameEventArgs
    {
        public Type ExitedProcedureType { get; private set; }
        public string ExitedProcedureName { get; private set; }
        public Type EnteredProcedureType { get; private set; }
        public string EnteredProcedureName { get; private set; }
        public override void Release()
        {
            ExitedProcedureType = null;
            ExitedProcedureName = string.Empty;
            EnteredProcedureType = null;
            EnteredProcedureName = string.Empty;
        }
        public static ProcedureChangedEventArgs Create(Type exitedType, Type enteredType)
        {
            var eventArgs = ReferencePool.Acquire<ProcedureChangedEventArgs>();
            eventArgs.ExitedProcedureType = exitedType;
            if (exitedType != null)
                eventArgs.ExitedProcedureName = exitedType.Name;
            eventArgs.EnteredProcedureType = enteredType;
            if (enteredType != null)
                eventArgs.EnteredProcedureName = enteredType.Name;
            return eventArgs;
        }
        public static void Release(ProcedureChangedEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
