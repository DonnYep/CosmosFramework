using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public sealed partial class Utility
    {
        /// <summary>
        /// 框架相关工具类
        /// </summary>
        public static class Framework
        {
            public static string GetModuleTypeFullName(string moduleName)
            {
               return Utility.Text.Format("Cosmos." + moduleName + "." + moduleName + "Manager");
            }
            public static ModuleEnum GetModuleEnum(string moduleName)
            {
                //var fullName = Utility.Text.Format("Cosmos." + moduleName);
                var result = (ModuleEnum)Enum.Parse(typeof(ModuleEnum), moduleName);
                return result;
            }
        }
    }
}
