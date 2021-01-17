namespace Cosmos.Config
{
    public struct ConfigData
    {
        readonly bool boolValue;
        readonly int intValue;
        readonly float floatValue;
        readonly string stringValue;
        public bool BoolValue { get { return boolValue; } }
        public int IntValue { get { return intValue; } }
        public float FloatValue { get { return floatValue; } }
        public string StringValue { get { return stringValue; } }
        public ConfigData(bool boolValue, int intValue, float floatValue, string stringValue)
        {
            this.boolValue = boolValue;
            this.intValue = intValue;
            this.floatValue = floatValue;
            this.stringValue = stringValue;
        }
    }
}
