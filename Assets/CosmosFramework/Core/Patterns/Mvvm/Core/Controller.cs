using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    public abstract class Controller
    {
        public abstract void Execute( object data);
        protected T GetModel<T>(string modelKey) where T : Model
        {
            return MVC.GetModel<T>(modelKey);
        }
        protected T GetView<T>(string viewKey) where T : View
        {
            return MVC.GetView<T>(viewKey);
        }
        protected void BindView(View view)
        {
            MVC.BindView(view);
        }
        protected void BindModel(Model model)
        {
            MVC.BindModel(model);
        }
        protected void BindCommand<T>(string cmdKey)
            where T :Controller
        {
            MVC.BindCommand<T>(cmdKey);
        }
    }
}