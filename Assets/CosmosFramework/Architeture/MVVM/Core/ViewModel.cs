using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Mvvm
{
    public abstract class ViewModel
    {
        public abstract void Execute( object data);
        protected T GetModel<T>() where T : Model
        {
            return MVVM.GetModel<T>();
        }
        protected Model GetModel(Type modelType) 
        {
            return MVVM.GetModel(modelType);
        }
        protected T GetView<T>() where T : View
        {
            return MVVM.GetView<T>();
        }
        protected View GetView(Type viewType) 
        {
            return MVVM.GetView(viewType);
        }
        protected void BindView(View view)
        {
            MVVM.BindView(view);
        }
        protected void BindModel(Model model)
        {
            MVVM.BindModel(model);
        }
        protected void BindViewModel<T>(string vmKey)
            where T :ViewModel
        {
            MVVM.BindViewModel<T>(vmKey);
        }
    }
}