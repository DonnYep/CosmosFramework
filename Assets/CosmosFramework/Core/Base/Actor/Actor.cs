using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    /// <summary>
    /// 此类是一个容器；
    /// 能够缓存T类型的对象数据，并为T类型对象追加附加数据；
    /// 当为T类型对象追加数据时，无需打开T类进行代码重构；
    /// 例如为Peer对象追加数据，则无需修改peer，只需添加Variable即可
    /// </summary>
    /// <typeparam name="T">引用类型约束</typeparam>
    public class Actor<T> : ActorBase,IActor<T>,IReference
        where T : class
    {
        public override Type OwnerType { get { return (typeof(T)); } }
        /// <summary>
        /// 当前Actor是否处于激活状态
        /// </summary>
        public override bool IsActivated { get { return Owner!= null; } }
        public override byte ActorType { get;protected set; }
        public override int ActorID { get ;protected  set ; }
        public T Owner { get; private set; }
        /// <summary>
        /// 当前Actor的数据缓存	[0 ,65535]
        /// </summary>
        Dictionary<ushort, Variable> actorDataDict = new Dictionary<ushort, Variable>();
        public TData GetData<TData>(ushort dataKey)
            where TData : Variable
        {
            if (actorDataDict.ContainsKey(dataKey))
                return actorDataDict[dataKey] as TData;
            else
                return null;
        }
        public bool HasData(ushort dataKey)
        {
            return actorDataDict.ContainsKey(dataKey);
        }
        public bool RemoveData(ushort dataKey)
        {
            if (actorDataDict.ContainsKey(dataKey))
                return actorDataDict.Remove(dataKey);
            else
                return false;
        }
        /// <summary>
        /// 设置Actor的数据；
        /// 当存在数据，则更新；
        /// 当不存在数，则添加；
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="dataKey">数据的key</param>
        /// <param name="data">具体数据</param>
        public void SetData<TData>(ushort dataKey, TData data)
            where TData : Variable
        {
            if (!actorDataDict.ContainsKey(dataKey))
                actorDataDict.Add(dataKey,data);
            else
                actorDataDict[dataKey]= data;
        }
        public void Clear()
        {
            actorDataDict.Clear();
        }
        /// <summary>
        /// 生成 Actor
        /// </summary>
        /// <param name="actorType">Actor枚举约束的类型;</param>
        /// <param name="actorID">系统分配的ID</param>
        /// <param name="owner">Actor的拥有者</param>
        /// <returns>生成后的Actor</returns>
        public static Actor<T>Create(byte actorType,int actorID,T owner)
        {
            Actor<T> actor = Facade.SpawnReference<Actor<T>>();
            actor.Owner = owner;
            actor.ActorID = actorID;
            actor.ActorType = actorType;
            return actor;
        }
    }
}
