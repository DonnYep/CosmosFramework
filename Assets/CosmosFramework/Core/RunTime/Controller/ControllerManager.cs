using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public enum ControlMode : int
    {
        /// <summary>
        /// 自由控制
        /// </summary>
        FreeControl = 0,
        /// <summary>
        /// 第一人称控制
        /// </summary>
        FirstPerson = 1,
        /// <summary>
        /// 第三人称控制
        /// </summary>
        ThirdPerson = 2
    }
}
namespace Cosmos.Controller
{

    /// <summary>
    /// 控制器模块，客户端本地玩家的主要控制器。
    /// 从InputManager获取值后，由此控制输入值
    /// </summary>
    [Module]
     internal sealed class ControllerManager : Module, IControllerManager
    {
        #region Properties
        Dictionary<Type, List<IController>> controllerDict;
        List<IController> controllerCache;
        Type controllerType = typeof(IController);
        /// <summary>
        /// 相机跟随对象
        /// 当操纵Vehicle，Deveice时，通过事件中心由使用者切换跟随对象
        /// </summary>
        ControlMode currentControlMode = ControlMode.ThirdPerson;
        //Controller类型的数量，根据type计算
        short controllerTypeCount = 0;
        #endregion

        #region Methods
        public override void OnInitialization()
        {
            controllerDict = new Dictionary<Type, List<IController>>();
            controllerCache = new List<IController>();
        }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            if (controllerDict.Count <= 0)
                return;
            controllerCache.Clear();
            foreach (var controllerSet in controllerDict.Values)
            {
                controllerCache.AddRange(controllerSet);
            }
            int count = controllerCache.Count;
            for (int i = 0; i < count; i++)
            {
                controllerCache[i].OnRefresh();
            }
        }
         public bool RegisterController<T>(T controller)
            where T : class, IController
        {
            return RegisterController(typeof(T), controller);
        }
         public bool RegisterController(Type type, IController controller)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            bool result = false;
            if (!controllerDict.ContainsKey(type))
            {
                controllerDict.Add(type, new List<IController>() { controller });
                controllerTypeCount++;
                result = true;
            }
            else
            {
                var exist = controllerDict[type].Contains(controller);
                if (exist)
                    throw new ArgumentException("ControllerManager-->>" + "Controller : " + controller.ControllerName + " has  already registered");
                else
                {
                    controllerDict[type].Add(controller);
                    result = true;
                }
            }
            return result;
        }
         public bool DeregisterController<T>(T controller)
            where T : class, IController
        {
            return DeregisterController(typeof(T), controller);
        }
         public bool DeregisterController(Type type, IController controller)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            bool result = true;
            if (controllerDict.ContainsKey(type))
            {
                if (controllerDict[type].Contains(controller))
                {
                    result = controllerDict[type].Remove(controller);
                    if (controllerDict[type].Count == 0)
                    {
                        controllerDict.Remove(type);
                        controllerTypeCount--;
                    }
                }
                else
                    result = false;
            }
            else
            {
                result = false;
                throw new ArgumentNullException("ControllerManager-- >> " + "Controller: " + controller.ControllerName + "  has not registered");
            }
            return result;
        }
         public bool HasController<T>()
            where T : class, IController
        {
            return controllerDict.ContainsKey(typeof(T));
        }
         public bool HasController(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            return controllerDict.ContainsKey(type);
        }
         public bool HasControllerItem<T>(T controller)
            where T : class, IController
        {
            if (!HasController<T>())
                return false;
            else
                return controllerDict[typeof(T)].Contains(controller);
        }
         public bool HasControllerItem(Type type, IController controller)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (!HasController(type))
                return false;
            else
                return controllerDict[type].Contains(controller);
        }
         public T GetController<T>(Predicate<T> predicate)
            where T : class, IController
        {
            var key = typeof(T);
            IController temp = default(T);
            List<IController> ctrlSet;
            bool result = controllerDict.TryGetValue(key, out ctrlSet);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + key.FullName + " has not registered");
            int count = ctrlSet.Count;
            for (int i = 0; i < count; i++)
            {
                if(predicate(ctrlSet[i] as T))
                {
                    temp = ctrlSet[i];
                    return temp as T;
                }
            }
            return temp as T;
        }
         public T[] GetControllers<T>(Predicate<T> predicate)
            where T : class, IController
        {
            var key = typeof(T);
            if (controllerDict.ContainsKey(key))
            {
                List<T> list = new List<T>();
                foreach (var item in controllerDict[key])
                {
                    if (predicate(item as T))
                    {
                        list.Add(item as T);
                    }
                }
                return list.ToArray();
            }
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + key.ToString() + "  has not  registered");
        }
         public short GetControllerItemCount<T>()
            where T : class, IController
        {
            var key = typeof(T);
            if (controllerDict.ContainsKey(key))
                return (short)controllerDict[key].Count;
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + key.ToString() + "  has not  registered");
        }
         public short GetControllerItemCount(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (controllerDict.ContainsKey(type))
                return (short)controllerDict[type].Count;
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + type.ToString() + "  has not  registered");
        }
         public short GetControllerTypeCount()
        {
            return controllerTypeCount;
        }
         public void ClearAllController()
        {
            controllerDict.Clear();
            controllerTypeCount = 0;
        }
         public void ClearControllerItem<T>()
            where T : class, IController
        {
            if (HasController<T>())
                controllerDict[typeof(T)].Clear();
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + typeof(T).ToString() + "  has not  registered");
        }
         public void ClearControllerItem(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (HasController(type))
                controllerDict[type].Clear();
            else
                throw new ArgumentNullException ("ControllerManager-->>" + "Controller Type: " + type.ToString() + "  has not  registered");
        }
        /// <summary>
        /// 更改控制状态
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="callback">回调函数，与具体mode无关</param>
         public void SetControlMode(ControlMode mode, Action callback = null)
        {
            switch (mode)
            {
                case ControlMode.FirstPerson:
                    break;
                case ControlMode.FreeControl:
                    break;
                case ControlMode.ThirdPerson:
                    break;
            }
            currentControlMode = mode;
            callback?.Invoke();
        }
         public ControlMode GetControlMode()
        {
            return currentControlMode;
        }
        #endregion
    }
}