using Cosmos.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
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
        /// 通过特性UIResourceAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">UILogicBase派生类</typeparam>
        /// <param name="callBack">加载完毕后的回调</param>
        void LoadPanel<T>(Action<T> callBack = null) where T : UILogicBase;
        /// <summary>
        /// 通过特性UIResourceAttribute加载Panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callBack"></param>
        void OpenUI<T>(Action<T> callBack = null) where T : UILogicBase;
        void HideUI(string panelName);
        void RemoveUI(string panelName);
        void RemoveUl<T>() where T : UILogicBase;
        bool HasUI(string panelName);
    }
}
