using System;

namespace LiteMVC.Core
{
    public interface IBinder
    {
        Type ValueType { get; }
        int BindCount { get; }
        void Execute(object data);
        void Clear();
    }
}