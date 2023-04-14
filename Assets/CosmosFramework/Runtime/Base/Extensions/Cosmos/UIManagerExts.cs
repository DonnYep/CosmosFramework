using Cosmos.UI;
using System;
using System.Threading.Tasks;

namespace Cosmos.Extensions
{
    //async/await在线上存在奔溃，因此隔离Coroutine与Task，保持module的纯净
    public static class UIManagerExts
    {
        public static async Task<T> OpenUIFormAsync<T>(this IUIManager @this, UIAssetInfo assetInfo)
            where T : class, IUIForm
        {
            T uiForm = null;
            await @this.OpenUIFormAsync<T>(assetInfo, pnl => uiForm = pnl);
            return uiForm;
        }
        public static async Task<IUIForm> OpenUIFormAsync(this IUIManager @this, UIAssetInfo assetInfo, Type uiType)
        {
            IUIForm uiForm = null;
            await @this.OpenUIFormAsync(assetInfo, uiType, pnl => uiForm = pnl);
            return uiForm;
        }
    }
}
