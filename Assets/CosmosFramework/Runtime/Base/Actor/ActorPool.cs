using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// Actor池
    /// </summary>
    public class ActorPool : IEnumerable<ActorBase>
    {
        public int ActorCount { get { return actorDict.Count; } }
        public Type ActorType { get { return actorType; } }
        Type actorType;
        Dictionary<int, ActorBase> actorDict = new Dictionary<int, ActorBase>();
        public ActorPool(){}
        public ActorPool(Type type){ actorType = type; }
        public bool TryGetValue(int id,out ActorBase act)
        {
            act = null;
            if (!actorDict.ContainsKey(id))
                return false;
            else
            {
                act = actorDict[id];
                return true;
            }
        }
        public bool TryAdd(int id,ActorBase act)
        {
            if (act == null)
                return false;
            if (actorDict.ContainsKey(id))
                return false;
            else
            {
                actorDict.Add(id, act);
                return true;
            }
        }
        public bool Contains(int id)
        {
            return actorDict.ContainsKey(id);
        }
        public bool AddOrUpdate(int id,ActorBase act)
        {
            if (act == null)
                return false;
            if (!actorDict.ContainsKey(id))
            {
                actorDict.Add(id, act);
            }
            else
            {
                actorDict[id]=act;
            }
            return true;
        }
        public IEnumerator<ActorBase> GetEnumerator()
        {
            return actorDict.Values.GetEnumerator();

        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
