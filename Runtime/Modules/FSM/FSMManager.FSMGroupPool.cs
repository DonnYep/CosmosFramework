namespace Cosmos.FSM
{
    internal sealed partial class FSMManager
    {
        private class FSMGroupPool
        {
            Pool<FSMGroup> pool;
            public FSMGroupPool()
            {
                pool = new Pool<FSMGroup>(OnGenerate,OnDespawn);
            }
            public void Despawn(FSMGroup group)
            {
                pool.Despawn(group);
            }
            public FSMGroup Spawn()
            {
                return pool.Spawn();
            }
            public void Clear()
            {
                pool.Clear();
            }
            void OnDespawn(FSMGroup group)
            {
                group.Clear();
            }
            FSMGroup OnGenerate()
            {
                return new FSMGroup();
            }
        }
    }
}
