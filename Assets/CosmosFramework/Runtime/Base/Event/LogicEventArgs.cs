namespace Cosmos
{
    public class LogicEventArgs : GameEventArgs
    {
        public object Data { get; private set; }
        public LogicEventArgs(object data)
        {
            SetData(data);
        }
        public LogicEventArgs() { }
        public object SetData(object data)
        {
            Data = data;
            return this;
        }
        public override void Clear()
        {
            Data = null;
        }
    }
}
