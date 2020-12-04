using System;

namespace Cosmos
{
    public interface IControllerManager:IModuleManager
    {
        bool RegisterController<T>(T controller) where T : class, IController;
        bool RegisterController(Type type, IController controller);
        bool DeregisterController<T>(T controller) where T : class, IController;
        bool DeregisterController(Type type, IController controller);
        bool HasController<T>() where T : class, IController;
        bool HasController(Type type);
        bool HasControllerItem<T>(T controller) where T : class, IController;
        bool HasControllerItem(Type type, IController controller);
        T GetController<T>(Predicate<T> predicate) where T : class, IController;
        T[] GetControllers<T>(Predicate<T> predicate) where T : class, IController;
        short GetControllerItemCount<T>() where T : class, IController;
        short GetControllerItemCount(Type type);
        short GetControllerTypeCount();
        void ClearAllController();
        void ClearControllerItem<T>() where T : class, IController;
        void ClearControllerItem(Type type);
        /// <summary>
        /// 更改控制状态
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="callback">回调函数，与具体mode无关</param>
        void SetControlMode(ControlMode mode, Action callback = null);
        ControlMode GetControlMode();
    }
}
