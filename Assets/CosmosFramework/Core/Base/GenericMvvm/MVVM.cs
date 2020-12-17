using Cosmos.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cosmos.GenericMvvm
{
    public class MVVM
    {
        static Dictionary<string, IView> viewDict = new Dictionary<string, IView>();
        static Dictionary<string, IModel> modelDict = new Dictionary<string, IModel>();
        static Dictionary<string, Type> cmdTypeDict = new Dictionary<string, Type>();
        static Dictionary<Type, Queue<ICommand>> cmdQueueDict = new Dictionary<Type, Queue<ICommand>>();
        public static void BindView<T>(T view)
          where T : class, IView
        {
            view.BindEvent();
            viewDict.AddOrUpdate(view.Name, view);
        }
        public static void UnbindView(string viewName)
        {
            viewDict.Remove(viewName, out var view);
            view?.OnUnbind();
        }
        public static void BindModel<T>(T model)
            where T : class, IModel
        {
            modelDict.AddOrUpdate(model.Name, model);
        }
        public static void UnbindModel(string modelName)
        {
            modelDict.TryRemove(modelName, out var model);
            model.OnUnbind();
        }
        public static void BindCommand<T>(string cmdName)
where T : class, ICommand
        {
            var cmdType = typeof(T);
            cmdTypeDict.AddOrUpdate(cmdName, cmdType);
            if (!cmdQueueDict.ContainsKey(cmdType))
            {
                cmdQueueDict.Add(cmdType, new Queue<ICommand>());
            }
        }
        public static void BindCommand(Type cmdType, string cmdName)
        {
            if (typeof(ICommand).IsAssignableFrom(cmdType))
            {
                cmdTypeDict.AddOrUpdate(cmdName, cmdType);
                if (!cmdQueueDict.ContainsKey(cmdType))
                {
                    cmdQueueDict.Add(cmdType, new Queue<ICommand>());
                }
            }
            else
            {
                throw new ArgumentException($"Command :{cmdType} is not inherit form Command!");
            }
        }
        public static bool UnbindCommand(string cmdName)
        {
            var result=false;
            if (cmdTypeDict.TryRemove(cmdName, out var cmdType))
            {
                if (cmdQueueDict.TryRemove(cmdType, out _))
                    result = true;
            }
            else
            {
                throw new ArgumentException($"Command name :{cmdName} has not bind any command!");
            }
            return result;
        }
        public static T GetView<T>(string viewName) where T : class, IView
        {
            var result = viewDict.TryGetValue(viewName, out var view);
            if (result)
                return view as T;
            else
                return null;
        }
        public static T GetModel<T>(string modelName) where T : class, IModel
        {
            var result = modelDict.TryGetValue(modelName, out var model);
            if (result)
                return model as T;
            else
                return null;
        }
        public static T GetCommand<T>(string cmdName) where T : class, ICommand
        {
            var result = cmdTypeDict.TryGetValue(cmdName, out var vm);
            if (result)
                return vm as T;
            else
                return null;
        }
        public static void SendEvent(string cmdName, object data = null)
        {
            if (cmdTypeDict.TryGetValue(cmdName, out var cmdType))
            {
                cmdQueueDict.TryGetValue(cmdType, out var cmdQueue);
                var cmd = cmdQueue.Dequeue();
                if (cmd == null)
                    cmd = Utility.Assembly.GetTypeInstance(cmdType) as ICommand;
                cmd.Execute(data);
                cmdQueue.Enqueue(cmd);
            }
            foreach (var view in viewDict.Values)
            {
                view.ExecuteEvent(cmdName, data);
            }
        }
        public static void AutoBindCommand()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyLength = assemblies.Length;
            for (int h = 0; h < assemblyLength; h++)
            {
                var cmdTypes = Utility.Assembly.GetDerivedTypesByAttribute<CommandAttribute, ICommand>(assemblies[h]);
                var length = cmdTypes.Length;
                for (int i = 0; i < length; i++)
                {
                    var cmdAtt = cmdTypes[i].GetCustomAttribute<CommandAttribute>();
                    BindCommand(cmdTypes[i], cmdAtt.CommandName);
                }
            }
        }
    }
}