using Cosmos.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Test
{
    public class VM_Navigate : ViewModel
    {
        public override void Execute(object data)
        {
            Utility.Debug.LogInfo($"Run {UIEventDefine.VM_Navigate}",MessageColor.RED);

            BindModel(new M_Inventory());


            BindView(new V_Inventory());


            BindViewModel<VM_Inventory>(UIEventDefine.VM_Inventory);
        }
    }
}