using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    internal class ControllerGroup
    {
        Dictionary<int, IController> controllerDict;
        internal int[] ControllerIds { get { return controllerDict.Keys.ToArray(); } }
        public int ControllerCount { get { return controllerDict.Count; } }
        public ControllerGroup()
        {
            controllerDict = new Dictionary<int, IController>();
        }
        public bool AddController(IController controller)
        {
            var controllerId = controller.Id;
            if (controllerDict.TryAdd(controllerId, controller))
                return true;
            return false;
        }
        public bool RemoveController(int controllerId)
        {
            if (controllerDict.Remove(controllerId, out var controller))
                return true;
            return false;
        }
        public bool HasController(int controllerId)
        {
            return controllerDict.ContainsKey(controllerId);
        }
        public IController GetController(int controllerId)
        {
            controllerDict.TryGetValue(controllerId, out var controller);
            return controller;
        }
        public IController GetController(Predicate<IController> predicate)
        {
            foreach (var ctrl in controllerDict)
            {
                var c = ctrl.Value;
                if (predicate(c))
                    return c;
            }
            return default;
        }
        /// <summary>
        /// 条件查找多个Controller；
        /// </summary>
        /// <param name="predicate">条件函数</param>
        /// <returns>查找到的Controller数组</returns>
        public IController[] GetControllers(Predicate<IController> predicate)
        {
            List<IController> list = new List<IController>();
            foreach (var ctrl in controllerDict)
            {
                var c = ctrl.Value;
                if (predicate(c))
                    list.Add(c);
            }
            return list.ToArray();
        }
        /// <summary>
        /// 获取此controllerGroup的所有Controller；
        /// </summary>
        /// <returns>所有Controller数组</returns>
        public IController[] GetAllControllers()
        {
            return controllerDict.Values.ToArray();
        }
        /// <summary>
        /// 清理并销毁controller
        /// </summary>
        public void ClearControllers()
        {
            controllerDict.Clear();
        }
        public void PauseControllers()
        {
            foreach (var ctrl  in controllerDict)
            {
                ctrl.Value.Pause = true;
            }
        }
        public void UnPauseControllers()
        {
            foreach (var ctrl in controllerDict)
            {
                ctrl.Value.Pause = false;
            }
        }
    }
}
