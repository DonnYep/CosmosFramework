using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cosmos.Mvvm
{
    /// <summary>
    /// MVC调度类；
    /// 此结构为：M<->C<->V
    /// <->表示双向可操控；M与V解耦；
    /// </summary>
    public static class MVC
    {
        static Dictionary<string, View> viewDict = new Dictionary<string, View>();
        static Dictionary<string, Model> modelDict = new Dictionary<string, Model>();
        static Dictionary<string, Type> cmdTypeDict = new Dictionary<string, Type>();
        static Dictionary<Type,Queue< Controller>> cmdQueueDict = new Dictionary<Type, Queue< Controller>>();
        /// <summary>
        /// 绑定View
        /// </summary>
        /// <typeparam name="T">继承自View的子类</typeparam>
        /// <param name="view">view对象</param>
        public static void BindView<T>(T view)
          where T : View
        {
            view.BindCommand();
            viewDict.AddOrUpdate(view.CommandKey, view);
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <typeparam name="T">派生自Model的子类</typeparam>
        /// <param name="model">数据模型</param>
        public static void BindModel<T>(T model)
            where T : Model
        {
            modelDict.AddOrUpdate(model.CommandKey, model);
        }
        /// <summary>
        /// 绑定command；
        /// </summary>
        /// <typeparam name="T">继承自command的子类</typeparam>
        /// <param name="cmdKey">command key</param>
        public static void BindCommand<T>( string cmdKey)
where T : Controller
        {
            var cmdType = typeof(T);
            cmdTypeDict.AddOrUpdate(cmdKey, cmdType);
            if (!cmdQueueDict.ContainsKey(cmdType))
            {
                cmdQueueDict.Add(cmdType, new Queue<Controller>());
            }
        }
        /// <summary>
        /// 绑定command；
        /// </summary>
        /// <param name="cmdType">继承自command的子类</param>
        /// <param name="cmdKey">command key</param>
        public static void BindCommand(Type cmdType, string cmdKey)
        {
            if (typeof(Controller).IsAssignableFrom(cmdType))
            {
                cmdTypeDict.AddOrUpdate(cmdKey, cmdType);
                if (!cmdQueueDict.ContainsKey(cmdType))
                {
                    cmdQueueDict.Add(cmdType, new Queue<Controller>());
                }
            }
            else
            {
                throw new ArgumentException($"Command :{cmdType} is not inherit form Command!");
            }
        }
        public static T GetView<T>(string viewKey) where T : View
        {
            var result = viewDict.TryGetValue(viewKey, out var view);
            if (result)
                return view as T;
            else
                return null;
        }
        public static T GetModel<T>(string modelKey) where T : Model
        {
            var result = modelDict.TryGetValue(modelKey, out var model);
            if (result)
                return model as T;
            else
                return null;
        }
        public static T GetCommand<T>(string cmdKey) where T : Controller
        {
            var result = cmdTypeDict.TryGetValue(cmdKey, out var vm);
            if (result)
                return vm as T;
            else
                return null;
        }
        /// <summary>
        /// 抛出事件，线程安全；
        /// </summary>
        /// <param name="cmdKey">command key</param>
        /// <param name="data">附带的数据</param>
        public static void Fire(string cmdKey, object data = null)
        {
            if (cmdTypeDict.TryGetValue(cmdKey, out var cmdType))
            {
                cmdQueueDict.TryGetValue(cmdType, out var cmdQueue);
                var cmd = cmdQueue.Dequeue();
                if (cmd == null)
                    cmd = Utility.Assembly.GetTypeInstance(cmdType) as Controller;
                cmd.Execute(data);
                //执行完进行回收；
                lock (cmdQueueDict)
                {
                    cmdQueue.Enqueue(cmd);
                }
            }
            foreach (var view in viewDict.Values)
            {
                view.ExecuteEvent(cmdKey, data);
            }
        }
        /// <summary>
        /// 自动绑定所有标记CommandAttribute标签的类
        /// </summary>
        public static void AutoBindCommand()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyLength = assemblies.Length;
            for (int h = 0; h < assemblyLength; h++)
            {
                var cmdTypes = Utility.Assembly.GetDerivedTypesByAttribute<CommandAttribute, Controller>(assemblies[h]);
                var length = cmdTypes.Length;
                for (int i = 0; i < length; i++)
                {
                    var cmdAtt = cmdTypes[i].GetCustomAttribute<CommandAttribute>();
                    BindCommand(cmdTypes[i], cmdAtt.CommandKey);
                }
            }
        }
    }
}
