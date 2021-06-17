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
        Dictionary<int, Action> lateActionDict;
        Dictionary<int, Action> tickActionDict;
        Dictionary<int, Action> fixedActionDict;
        Action lateRefresh;
        Action tickRefresh;
        Action fixedRefresh;
        public int ControllerCount { get { return controllerDict.Count; } }
        public ControllerGroup()
        {
            controllerDict = new Dictionary<int, IController>();
            tickActionDict = new Dictionary<int, Action>();
            lateActionDict = new Dictionary<int, Action>();
            fixedActionDict = new Dictionary<int, Action>();
        }
        public void TickRefresh()
        {
            tickRefresh?.Invoke();
        }
        public void FixedRefresh()
        {
            fixedRefresh?.Invoke();
        }
        public void LateRefresh()
        {
            lateRefresh?.Invoke();
        }
        public bool AddController(IController controller)
        {
            var controllerId = controller.Id;
            if (controllerDict.TryAdd(controllerId, controller))
            {
                TickRefreshAttribute.GetRefreshAction(controller, true, out var tickAction);
                if (tickAction != null)
                {
                    tickActionDict.Add(controllerId, tickAction);
                    tickRefresh += tickAction;
                }
                LateRefreshAttribute.GetRefreshAction(controller, true, out var lateAction);
                if (lateAction != null)
                {
                    lateActionDict.Add(controllerId, lateAction);
                    lateRefresh += lateAction;
                }
                FixedRefreshAttribute.GetRefreshAction(controller, true, out var fixedAction);
                if (fixedAction != null)
                {
                    fixedActionDict.Add(controllerId, fixedAction);
                    fixedRefresh += fixedAction;
                }
                return true;
            }
            return false;
        }
        public bool RemoveController(int controllerId)
        {
            if (controllerDict.Remove(controllerId, out var controller))
            {
                if (tickActionDict.Remove(controllerId, out var tickAction))
                    tickRefresh -= tickAction;
                if (lateActionDict.Remove(controllerId, out var lateAction))
                    lateRefresh -= lateAction;
                if (fixedActionDict.Remove(controllerId, out var fixedAction))
                    fixedRefresh -= fixedAction;
                return true;
            }
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
        /// 清理并销毁controller
        /// </summary>
        public void ClearControllers()
        {
            controllerDict.Clear();
            tickActionDict.Clear();
            lateActionDict.Clear();
            fixedActionDict.Clear();
            lateRefresh=null;
            tickRefresh=null;
            fixedRefresh=null;
        }
    }
}
