using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    public class View:Singleton<View>
    {
        Dictionary<string, Mediator> mediatorDict = new Dictionary<string, Mediator>();


        protected HashSet<string> vmKeyList = new HashSet<string>();
        public void ExecuteEvent(string vmKey, object data)
        {
            if (vmKeyList.Contains(vmKey))
            {
                HandleEvent(vmKey, data);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        public  void BindVMKey() { }
        protected void BindKey(string vmKey)
        {
            vmKeyList.Add(vmKey);
        }
        protected void UnbindKey(string vmKey)
        {
            vmKeyList.Remove(vmKey);
        }
        protected void HandleEvent(string vmKey, object data = null) { }
        protected T GetViewModel<T>(string vmKey)
            where T : ViewModel
        {
            return MVVM.GetViewModel<T>(vmKey);
        }
        protected void Fire(string vmKey, object data = null)
        {
            MVVM.Fire(vmKey, data);
        }
    }
}