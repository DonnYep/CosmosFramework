namespace Cosmos.Input
{
    /// <summary>
    /// 虚拟轴线
    /// </summary>
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
        /// <summary>
        /// 未插值取值：只给出 -1 / 0 / 1
        /// </summary>
        public float GetValueYaw
        {
            get
            {
                if (value < 0) return -1;
                else if (value > 0) return 1;
                else return 0;
            }
        }
        /// <summary>
        /// 清空状态
        /// </summary>
        public void Reset()
        {
            value = 0f;
        }
    }
}
