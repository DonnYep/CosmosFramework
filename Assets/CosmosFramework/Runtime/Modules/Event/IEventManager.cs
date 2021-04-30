using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IEventManager: IModuleManager
    {
        void AddListener(string eventKey, EventHandler<GameEventArgs> handler);
        /// <summary>
        /// 移除事件，假如事件在字典中已经为空，则自动注销，无需手动
        /// </summary>
        /// <param name="eventKey">事件的key，可以是对象，字符</param>
        /// <param name="hander">事件处理者</param>
        void RemoveListener(string eventKey, EventHandler<GameEventArgs> hander);
        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="sender">事件的触发者</param>
        /// <param name="args">事件处理类</param>
        void DispatchEvent(string eventKey, object sender, GameEventArgs args);
        /// <summary>
        /// 注销并移除事件
        /// </summary>
        void DeregisterEvent(string eventKey);
        /// <summary>
        /// 在事件中心注册一个空的事件
        /// 当前设计是为事件的触发者设计，空事件可以使其他订阅者订阅
        /// </summary>
        void RegisterEvent(string eventKey);
        /// <summary>
        /// 清空已经注册的事件
        /// </summary>
        void ClearEvent(string eventKey);
        /// <summary>
        /// 注销所有除框架以外的事件
        /// </summary>
        void ClearAllEvent();
        /// <summary>
        /// 判断事件是否注册
        /// </summary>
        bool HasEvent(string eventKey);
    }
}
