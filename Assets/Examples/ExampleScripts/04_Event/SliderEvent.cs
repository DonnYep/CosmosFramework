using Cosmos;

public class SliderEvent : GameEventArgs
{
    public float MaxValue;
    public float Value;
    public override void Release()
    {
        MaxValue = 0;
        Value = 0;
    }
}
