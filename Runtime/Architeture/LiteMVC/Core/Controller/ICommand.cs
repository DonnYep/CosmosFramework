using System;
namespace LiteMVC.Core
{
    public interface ICommand
    {
        Type BindType { get; }
    }
}
