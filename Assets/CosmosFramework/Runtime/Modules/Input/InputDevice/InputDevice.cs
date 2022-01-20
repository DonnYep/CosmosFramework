namespace Cosmos.Input
{
    public abstract class InputDevice : IInputDevice
    {
        public abstract void OnStart();
        public abstract void OnRun();
        public abstract void OnShutdown();
    }
}