using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Cosmos.UI{
    public abstract  class UILogicBase:MonoBehaviour{
        /// <summary>
        /// UI的映射表，名字作为主键，具有一个list容器
        /// </summary>
        Dictionary<string, List<UIBehaviour>> uiMap = new Dictionary<string, List<UIBehaviour>>();
        protected T GetUIPanel<T>(string name)
            where T:UIBehaviour
        {
            if (uiMap.ContainsKey(name))
            {
                short listCount = (short)uiMap[name].Count;
                for (short i = 0; i <listCount ; i++)
                {
                    if (uiMap[name][i].gameObject.name == name)
                        return uiMap[name][i] as T;
                }
            }
            return null;
        }
        void GetUIPanel<T>()
            where T : UIBehaviour
        {
            T[] uiPanels = GetComponentsInChildren<T>();
            string panelName;
            short panelCount = (short)uiPanels.Length;
            for (short i = 0; i < panelCount; i++)
            {
                panelName = uiPanels[i].gameObject.name;
                if (uiMap.ContainsKey(panelName))
                {
                    uiMap[panelName].Add(uiPanels[i]);
                }
                else
                {
                    uiMap.Add(panelName, new List<UIBehaviour>() { uiPanels[i] });
                }
            }
        }
        public virtual void ShowPanel()
        {

        }
        public virtual void HidePanel()
        {

        }
    }
}