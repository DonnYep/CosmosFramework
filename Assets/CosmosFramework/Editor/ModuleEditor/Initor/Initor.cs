using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
namespace Cosmos.Editor
{
    public class Initor
    {
        [MenuItem("Window/Cosmos/Init")]
        public static void InitCosmosFramework()
        {
            //初始化框架
            var resourceSettings = UnityEngine.Resources.Load<ResourceSettings>("ResourceSettings");
            if(resourceSettings == null)
            {
                resourceSettings = EditorUtil.CreateScriptableObject<ResourceSettings>("Assets/Resources/ResourceSettings.asset");
            }

        }
    }
}
