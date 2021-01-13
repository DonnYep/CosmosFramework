using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cosmos.Mvvm
{
    /// <summary>
    /// MVC调度类；
    /// 此结构为：M<->C<->P
    /// <->表示双向可操控；M与V解耦；
    /// </summary>
    public static class MVVM
    {
        static Dictionary<Type, View> viewDict;
        static Dictionary<Type, Model> modelDict;
        static Dictionary<string, Type> vmTypeDict;
        static Dictionary<Type, Queue<ViewModel>> vmQueueDict;
        /// <summary>
        /// 绑定View
        /// </summary>
        /// <typeparam name="T">继承自View的子类</typeparam>
        /// <param name="view">view对象</param>
        public static void BindView<T>(T view)
          where T : View
        {
            var type = typeof(T);
            view.BindVMKey();
            viewDict.AddOrUpdate(type, view);
        }
        public static void BindView(Type viewType,View view)
        {
            if (!typeof(View).IsAssignableFrom(viewType))
            {
                throw new ArgumentException($"View :{viewType} is not inherit form view");
            }
            view.BindVMKey();
            viewDict.AddOrUpdate(viewType, view);
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <typeparam name="T">派生自Model的子类</typeparam>
        /// <param name="model">数据模型</param>
        public static void BindModel<T>(T model)
            where T : Model
        {
            var type = typeof(T);
            modelDict.AddOrUpdate(type, model);
        }
        public static void BindModel (Type modelType,Model model)
        {
            if (!typeof(Model).IsAssignableFrom(modelType))
            {
                throw new ArgumentException($"Model :{modelType} is not inherit form Model");
            }
            modelDict.AddOrUpdate(modelType, model);
        }
        /// <summary>
        /// 绑定command；
        /// </summary>
        /// <typeparam name="T">继承自command的子类</typeparam>
        /// <param name="vmKey">command key</param>
        public static void BindViewModel<T>( string vmKey)
where T : ViewModel
        {
            var vmType = typeof(T);
            vmTypeDict.AddOrUpdate(vmKey, vmType);
            if (!vmQueueDict.ContainsKey(vmType))
            {
                vmQueueDict.Add(vmType, new Queue<ViewModel>());
            }
        }
        /// <summary>
        /// 绑定command；
        /// </summary>
        /// <param name="vmType">继承自command的子类</param>
        /// <param name="vmKey">command key</param>
        public static void BindViewModel(Type vmType, string vmKey)
        {
            if (typeof(ViewModel).IsAssignableFrom(vmType))
            {
                vmTypeDict.AddOrUpdate(vmKey, vmType);
                if (!vmQueueDict.ContainsKey(vmType))
                {
                    vmQueueDict.Add(vmType, new Queue<ViewModel>());
                }
            }
            else
            {
                throw new ArgumentException($"ViewModel :{vmType} is not inherit form ViewModel!");
            }
        }
        public static T GetView<T>() where T : View
        {
            var type = typeof(T);
            var result = viewDict.TryGetValue(type, out var view);
            if (result)
                return view as T;
            else
                return null;
        }
        public static  View GetView(Type viewType) 
        {
            if (!typeof(View).IsAssignableFrom(viewType))
            {
                throw new ArgumentException($"View :{viewType} is not inherit form view");
            }
            var result = viewDict.TryGetValue(viewType, out var view);
            if (result)
                return view ;
            else
                return null;
        }
        public static T GetModel<T>() where T : Model
        {
            var type = typeof(T);
            var result = modelDict.TryGetValue(type, out var model);
            if (result)
                return model as T;
            else
                return null;
        }
        public static Model GetModel(Type modelType)  
        {
            if (!typeof(Model).IsAssignableFrom(modelType))
            {
                throw new ArgumentException($"Model :{modelType} is not inherit form Model");
            }
            var result = modelDict.TryGetValue(modelType, out var model);
            if (result)
                return model;
            else
                return null;
        }
        public static T GetViewModel<T>(string vmKey) where T : ViewModel
        {
            var result = vmTypeDict.TryGetValue(vmKey, out var vm);
            if (result)
                return vm as T;
            else
                return null;
        }
        /// <summary>
        /// 抛出事件，线程安全；
        /// </summary>
        /// <param name="vmKey">command key</param>
        /// <param name="data">附带的数据</param>
        public static void Fire(string vmKey, object data = null)
        {
            if (vmTypeDict.TryGetValue(vmKey, out var vmType))
            {
                vmQueueDict.TryGetValue(vmType, out var vmQueue);
                ViewModel vm = null;
                if (vmQueue.Count > 0)
                    vm = vmQueue.Dequeue();
                else
                    vm = Utility.Assembly.GetTypeInstance(vmType) as ViewModel;
                vm.Execute(data);
                //执行完进行回收；
                lock (vmQueueDict)
                {
                    vmQueue.Enqueue(vm);
                }
            }
            foreach (var view in viewDict.Values)
            {
                view.ExecuteEvent(vmKey, data);
            }
        }
        /// <summary>
        /// 自动绑定所有标记ViewModelAttribute标签的类
        /// </summary>
        public static void AutoBindViewModel()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyLength = assemblies.Length;
            for (int h = 0; h < assemblyLength; h++)
            {
                var cmdTypes = Utility.Assembly.GetDerivedTypesByAttribute<ViewModelAttribute, ViewModel>(assemblies[h]);
                var length = cmdTypes.Length;
                for (int i = 0; i < length; i++)
                {
                    var cmdAtt = cmdTypes[i].GetCustomAttribute<ViewModelAttribute>();
                    BindViewModel(cmdTypes[i], cmdAtt.ViewModelKey);
                }
            }
        }
        static MVVM()
        {
            viewDict = new Dictionary<Type, View>();
            modelDict = new Dictionary<Type, Model>();
            vmTypeDict = new Dictionary<string, Type>();
            vmQueueDict = new Dictionary<Type, Queue<ViewModel>>();
        }
    }
}
