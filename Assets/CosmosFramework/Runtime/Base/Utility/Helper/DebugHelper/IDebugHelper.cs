namespace Cosmos
{
    public interface IDebugHelper
    {
        void LogInfo(object msg, object context);
        void LogInfo(object msg, string msgColor, object context);
        void LogWarning(object msg, object context);
        void LogError(object msg, object context);
        void LogFatal(object msg, object context);
    }
}
