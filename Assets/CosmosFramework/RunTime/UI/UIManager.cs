using UnityEngine;
using System.Collections;
namespace Cosmos.UI
{
    public sealed class UIManager : Module<UIManager>
    {
        public static string MainUICanvasName { get; set; }
        static GameObject mainUICanvas;
        public static GameObject MainUICanvas { get { /*if (mainUICanvas == null) */return mainUICanvas; } }
        protected override void InitModule()
        {
            RegisterModule(CFModule.UI);
        }
    }
}
