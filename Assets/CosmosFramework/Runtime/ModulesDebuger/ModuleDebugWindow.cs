using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// cosmos module debug window base 
    /// </summary>
    [DisallowMultipleComponent]
    public class ModuleDebugWindow : MonoSingleton<ModuleDebugWindow>
    {
        Rect windowRect;
        Rect dragRect = new Rect(0f, 0f, float.MaxValue, 25f);
        float windowScale;
        float tempWindowScale;
        float wndDataRefreshInterval = 1f;
        float currentTime = 0;
        float rectWidth;
        float rectHeight;
        void Awake()
        {
            gameObject.name = "CosmosDebugWindow";
        }
        private void OnGUI()
        {

        }
        void InitWindow()
        {
            var width = Screen.width * 0.3f;
            var height = Screen.height * 0.3f;
            rectWidth = width;
            rectHeight = height;
            windowRect = new Rect(10f, 32f, width, height);
            windowScale = 2f;
            tempWindowScale = windowScale;
            wndDataRefreshInterval = 1;
        }
    }
}