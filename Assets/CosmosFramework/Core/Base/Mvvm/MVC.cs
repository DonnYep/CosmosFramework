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
        static Dictionary<Type,Queue< Command>> cmdQueueDict = new Dictionary<Type, Queue< Command>>();
        public static void RegisterView<T>(T view)
          where T : View
        {
            view.RegisterEvent();
            viewDict.AddOrUpdate(view.Name, view);
        }
        public static void RegisterModel<T>(T model)
            where T : Model
        {
            modelDict.AddOrUpdate(model.Name, model);
        }
        public static void RegisterCommand<T>( string cmdName)
where T : Command
        {
            var cmdType = typeof(T);
            cmdTypeDict.AddOrUpdate(cmdName, cmdType);
            if (!cmdQueueDict.ContainsKey(cmdType))
            {
                cmdQueueDict.Add(cmdType, new Queue<Command>());
            }
        }
        public static void RegisterCommand(Type cmdType, string cmdName)
        {
            if (typeof(Command).IsAssignableFrom(cmdType))
            {
                cmdTypeDict.AddOrUpdate(cmdName, cmdType);
                if (!cmdQueueDict.ContainsKey(cmdType))
                {
                    cmdQueueDict.Add(cmdType, new Queue<Command>());
                }
            }
            else
            {
                throw new ArgumentException($"Command :{cmdType} is not inherit form Command!");
            }
        }
        public static T GetView<T>(string viewName) where T : View
        {
            var result = viewDict.TryGetValue(viewName, out var view);
            if (result)
                return view as T;
            else
                return null;
        }
        public static T GetModel<T>(string modelName) where T : Model
        {
            var result = modelDict.TryGetValue(modelName, out var model);
            if (result)
                return model as T;
            else
                return null;
        }
        public static T GetCommand<T>(string cmdName) where T : Command
        {
            var result = cmdTypeDict.TryGetValue(cmdName, out var vm);
            if (result)
                return vm as T;
            else
                return null;
        }
        public static void SendEvent(string cmdName, object data = null)
        {
            if( cmdTypeDict.TryGetValue(cmdName, out var cmdType))
            {
                cmdQueueDict.TryGetValue(cmdType, out var cmdQueue);
                var cmd= cmdQueue.Dequeue();
                if (cmd == null)
                    cmd = Utility.Assembly.GetTypeInstance(cmdType) as Command;
                cmd.Execute(data);
                //执行完进行回收；
                cmdQueue.Enqueue(cmd);
            }
            foreach (var view in viewDict.Values)
            {
                view.ExecuteEvent(cmdName, data);
            }
        }
        public static void AutoRegisterCommand()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyLength = assemblies.Length;
            for (int h = 0; h < assemblyLength; h++)
            {
                var cmdTypes = Utility.Assembly.GetDerivedTypesByAttribute<CommandAttribute, Command>(assemblies[h]);
                var length = cmdTypes.Length;
                for (int i = 0; i < length; i++)
                {
                    var cmdAtt = cmdTypes[i].GetCustomAttribute<CommandAttribute>();
                    RegisterCommand(cmdTypes[i], cmdAtt.CommandName);
                }
            }
        }
    }
}
