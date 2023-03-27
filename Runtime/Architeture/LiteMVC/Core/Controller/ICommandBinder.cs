using System;

namespace LiteMVC.Core
{
    public interface ICommandBinder: IBinder
    {
        void Bind(Type cmdType);
        void Unbind(Type cmdType);
        bool HasBind(Type cmdType);
    }
}
