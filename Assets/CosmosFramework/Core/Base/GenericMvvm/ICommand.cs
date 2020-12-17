using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.GenericMvvm
{
    public interface ICommand
    {
        void Execute(object data);
    }
}
