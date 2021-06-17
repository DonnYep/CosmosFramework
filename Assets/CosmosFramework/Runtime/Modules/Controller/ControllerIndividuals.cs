using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Controller
{
    public class ControllerIndividuals
    {
        public int ControllerCount { get { return controllerDict.Count; } }
        Dictionary<Type, IController> controllerDict;
        Dictionary<int, Action> lateActionDict;
        Dictionary<int, Action> tickActionDict;
        Dictionary<int, Action> fixedActionDict;
        Action lateRefresh;
        Action tickRefresh;
        Action fixedRefresh;
        public ControllerIndividuals()
        {
            controllerDict = new Dictionary<Type, IController>();
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
        public bool HasController(Type type)
        {
            return controllerDict.ContainsKey(type);
        }
        public bool AddController(Type type,IController controller)
        {
            var controllerId = controller.Id;
            if (controllerDict.TryAdd(type, controller))
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
        public bool RemoveController(Type type)
        {
            if (controllerDict.Remove(type, out var controller))
            {
                var controllerId = controller.Id;
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
        public IController GetController(Type type)
        {
            controllerDict.TryGetValue(type, out var controller);
            return controller;
        }
        public void ClearControllers()
        {
            controllerDict.Clear();
            tickActionDict.Clear();
            lateActionDict.Clear();
            fixedActionDict.Clear();
            lateRefresh = null;
            tickRefresh = null;
            fixedRefresh = null;
        }
    }
}
