using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.UI
{
    public interface IUIManager:IModuleManager
    {
        GameObject MainUICanvas { get; set; }

        GameObject InitMainCanvas(string path);
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        /// <param name="name">生成后重命名的名称</param>
        GameObject InitMainCanvas(string path, string name);
        /// <summary>
        /// 载入面板，若字典中已存在，则返回且不使用回调。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        /// <typeparam name="T"> UILogicBase</typeparam>
        /// <param name="panelName">相对完整路径</param>
        /// <param name="callBack">仅在载入时回调</param>
        void LoadPanel<T>(string panelName, Action<T> callBack = null) where T : UILogicBase;
        /// <summary>
        /// 通过特性UIResourceAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">UILogicBase派生类</typeparam>
        /// <param name="callBack">加载完毕后的回调</param>
        void LoadPanel<T>(Action<T> callBack = null) where T : UILogicBase;
        /// <summary>
        /// 载入面板，若字典中已存在，则使用回调，并返回。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        /// <typeparam name="T"> UILogicBase</typeparam>
        /// <param name="panelName">相对完整路径</param>
        /// <param name="callBack">仅在载入时回调</param>
        void ShowPanel<T>(string panelName, Action<T> callBack = null) where T : UILogicBase;
        /// <summary>
        /// 通过特性UIResourceAttribute加载Panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callBack"></param>
        void ShowPanel<T>(Action<T> callBack = null) where T : UILogicBase;
        void HidePanel(string panelName);
        void RemovePanel(string panelName);
        void RemovePanel<T>() where T : UILogicBase;
        bool HasPanel(string panelName);
    }
}
