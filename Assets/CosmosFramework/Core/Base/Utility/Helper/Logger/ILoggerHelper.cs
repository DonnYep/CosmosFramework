using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface ILoggerHelper
    {
        void Info(string msg);
        void Warring(string msg);
        void Error(Exception exception, string msg);
        void Fatal(string msg);
    }
}
