namespace Cosmos.Input
{
    internal sealed class InputVirtualAxis
    {
        public string Name { get; private set; }
        float value;
        public InputVirtualAxis(string name)
        {
            Name = name;
        }
        public void Update(float value)
        {
            this.value = value;
        }
        public float GetValue
        {
            get { return value; }
        }
        public float GetValueYaw
        {
            get
            {
                if (value < 0) return -1;
                else if (value > 0) return 1;
                else return 0;
            }
        }
    }
}
