using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    internal class ControllerPool
    {
        Dictionary<string, IController> controllerDict;
        public int ControllerCount { get { return controllerDict.Count; } }
        Action refresh;
        event Action Refresh
        {
            add { refresh += value; }
            remove { refresh -= value; }
        }
        public ControllerPool()
        {
            controllerDict = new Dictionary<string, IController>();
        }
        public void OnRefresh()
        {
            refresh?.Invoke();
        }
        public bool AddController(IController controller)
        {
            if (controllerDict.TryAdd(controller.ControllerName, controller))
            {
                Refresh += controller.OnRefresh;
                return true;
            }
            else
            {
                //throw new ArgumentException("ControllerManager-->>" + "Controller : " + controller.ControllerName + " has  already registered");
                Utility.Debug.LogError("ControllerManager-->>" + "Controller : " + controller.ControllerName + " has  already registered");
            }
            return false;
        }
        public bool RemoveController(string controllerName)
        {
            if (controllerDict.Remove(controllerName, out var controller))
            {
                Refresh -= controller.OnRefresh;
                return true;
            }
            else
                Utility.Debug.LogError("ControllerManager-- >> " + "Controller: " + controller.ControllerName + "  has not registered");
            return false;
        }
        public bool RemoveController(IController controller)
        {
            if (controllerDict.Remove(controller.ControllerName))
            {
                Refresh -= controller.OnRefresh;
                return true;
            }
            else
                Utility.Debug.LogError("ControllerManager-- >> " + "Controller: " + controller.ControllerName + "  has not registered");
            return false;
        }
        public bool HasController(string controllerName)
        {
            return controllerDict.ContainsKey(controllerName);
        }
        public IController GetController(string controllerName)
        {
            controllerDict.TryGetValue(controllerName, out var controller);
            return controller ;
        }
        public T GetController<T>(Predicate<T> predicate)
            where T : Component, IController
        {
            foreach (var ctrl in controllerDict)
            {
                var c = ctrl as T;
                if (predicate(c))
                    return c;
            }
            return default;
        }
        public T[] GetControllers<T>(Predicate<T> predicate)
    where T : Component, IController
        {
            List<T> list = new List<T>();
            foreach (var ctrl in controllerDict)
            {
                var c = ctrl as T;
                if (predicate(c))
                    list.Add(c);
            }
            return list.ToArray();
        }
        public T CreateController<T>(GameObject go, string controllerName = null)
    where T : Component, IController
        {
            T ctrl=default;
            if (!HasController(controllerName))
            {
                ctrl = go.GetOrAddComponent<T>();
                ctrl.ControllerName = controllerName;
                Refresh += ctrl.OnRefresh;
            }
            else
                Utility.Debug.LogError("ControllerManager-->>" + "Controller : " + ctrl.ControllerName + " has  already registered");
            return ctrl;
        }
        public void DestoryController(string controllerName)
        {
            if (controllerDict.TryRemove(controllerName, out var ctrl))
            {
                Refresh -= ctrl.OnRefresh;
                GameObject.Destroy(ctrl as Component);
            }else
                Utility.Debug.LogError("ControllerManager-- >> " + "Controller: " + ctrl.ControllerName + "  has not registered");
        }
        /// <summary>
        /// 清理并销毁controller
        /// </summary>
        public void DestroyControllers()
        {
            refresh = null;
            foreach (var ctrl in controllerDict.Values)
            {
                GameObject.Destroy(ctrl as Component);
            }
            controllerDict.Clear();
        }
    }
}
