using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Mvvm;
public class V_Navigate : View
{
    public override string CommandKey => UIEventDefine.UI_Navigate;
    protected override void HandleEvent(string cmdKey, object data = null)
    {

    }
}
