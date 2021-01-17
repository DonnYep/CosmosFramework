using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    /// <summary>
    /// MVC调度类；
    /// 此结构为：M<->C<->V
    /// <->表示双向可操控；M与V解耦；
    /// </summary>
    public static class MVC
    {
        static Dictionary<TypeStringPair, View> viewDict = new Dictionary<TypeStringPair, View>();
        static Dictionary<TypeStringPair, Model> modelDict = new Dictionary<TypeStringPair, Model>();
        static Dictionary<string, Command> cmdDict = new Dictionary<string, Command>();
        public static void RegisterView<T>(T view)
            where T : View
        {
            view.RegisterEvent();
            var typeString = new TypeStringPair(typeof(T), view.Name);
            viewDict.AddOrUpdate(typeString, view);
        }
        public static void RegisterModel<T>(T model)
            where T : Model
        {
            var typeString = new TypeStringPair(typeof(T), model.Name);
            modelDict.AddOrUpdate(typeString, model);
        }
        public static void RegisterCommand<T>( string cmdName)
where T : Command
        {
            var cmd= Utility.Assembly.GetTypeInstance(typeof(T)) as Command;
            cmdDict.AddOrUpdate(cmdName, cmd);
        }
        public static void RegisterCommand(Type cmdType, string cmdName)
        {
            if (typeof(Command).IsAssignableFrom(cmdType))
            {
                var cmd= Utility.Assembly.GetTypeInstance(cmdType) as Command;
                cmdDict.AddOrUpdate(cmdName, cmd);
            }
            else
            {
                throw new ArgumentException($"Command :{cmdType} is not inherit form Command!");
            }
        }
        public static T GetView<T>() where T : View
        {
            var typeString = new TypeStringPair(typeof(T));
            var result = viewDict.TryGetValue(typeString, out var view);
            if (result)
                return view as T;
            else
                return null;
        }
        public static T GetView<T>(string viewName) where T : View
        {
            var typeString = new TypeStringPair(typeof(T), viewName);
            var result = viewDict.TryGetValue(typeString, out var view);
            if (result)
                return view as T;
            else
                return null;
        }
        public static T GetModel<T>() where T : Model
        {
            var typeString = new TypeStringPair(typeof(T));
            var result = modelDict.TryGetValue(typeString, out var model);
            if (result)
                return model as T;
            else
                return null;
        }
        public static T GetModel<T>(string modelName) where T : Model
        {
            var typeString = new TypeStringPair(typeof(T), modelName);
            var result = modelDict.TryGetValue(typeString, out var model);
            if (result)
                return model as T;
            else
                return null;
        }
        public static T GetCommand<T>(string cmdName) where T : Command
        {
            var result = cmdDict.TryGetValue(cmdName, out var vm);
            if (result)
                return vm as T;
            else
                return null;
        }
        public static void SendEvent(string cmdName, object data = null)
        {
            if( cmdDict.TryGetValue(cmdName, out var cmd))
            {
                cmd.Execute(data);
            }
            foreach (var view in viewDict.Values)
            {
                view.ExecuteEvent(cmdName, data);
            }
        }
    }
}
