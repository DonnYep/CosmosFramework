namespace Cosmos
{
    public partial class AOIZone<T>
    {
        public interface IObjectHelper
        {
            float GetCenterX(T obj);
            float GetCenterY(T obj);
        }
    }
}
