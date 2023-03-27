using System;
namespace LiteMVC.Core
{
    public interface INotificationBinder
    {
        Type ValueType { get; }
        int BindCount { get; }
        void Execute(object data);
        void Clear();
    }
}
