using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    public abstract class Command
    {
        public abstract void Execute( object data);
        protected T GetModel<T>() where T : Model
        {
            return MVC.GetModel<T>();
        }
        protected T GetModel<T>(string modelName) where T : Model
        {
            return MVC.GetModel<T>(modelName);
        }
        protected T GetView<T>()where T :View
        {
            return MVC.GetView<T>();
        }
        protected T GetView<T>(string viewName) where T : View
        {
            return MVC.GetView<T>(viewName);
        }
        protected void RegisterView(View view)
        {
            MVC.RegisterView(view);
        }
        protected void RegisterModel(Model model)
        {
            MVC.RegisterModel(model);
        }
        protected void RegisterCommand<T>(string cmdName)
            where T :Command
        {
            MVC.RegisterCommand<T>(cmdName);
        }
    }
}