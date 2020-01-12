using UnityEngine;
using System.Collections;
using Cosmos.Event;
namespace Cosmos.Mono
{
    public interface IMonoController
    {
        /// <summary>
        /// 输入类型，返回对应的update的数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        short MonoActionCount(UpdateType type);
        /// <summary>
        /// 当前mono的ID
        /// </summary>
        short MonoID { get; set; }
        /// <summary>
        /// 是否当前对象的所有action都空了
        /// </summary>
        bool IsAllActionEmpty { get; }
        /// <summary>
        /// 输入对应的type，检测自身对象是否符合，是则返回自身的ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        short UseableMono(UpdateType type);
        void AddListener(CFAction act,UpdateType type);
        void RemoveListener(CFAction act,UpdateType type);
    }
}