namespace Cosmos.Lockstep
{
    public class GameEntry:CosmosEntry
    {
        public static IServiceManager  ServiceManager{ get { return GetModule<IServiceManager>(); } }
        public static IMultiplayManager   MultiplayManager{ get { return GetModule<IMultiplayManager>(); } }
    }
}
