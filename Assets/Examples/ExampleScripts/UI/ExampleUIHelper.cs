using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.UI;
public class ExampleUIHelper : IUIFormHelper
{
    public void HideUIForm(UIFormBase uiForm)
    {
        uiForm.gameObject.SetActive(false);
    }
    public void ShowUIForm(UIFormBase uiForm)
    {
        uiForm.gameObject.SetActive(true);
    }
}
