using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class ResourceSettingSO : ScriptableObject
    {
        /// <summary>
        /// 版本信息
        /// </summary>
        public string Version;
        /// <summary>
        /// 使用中的预设
        /// </summary>
        public string ProfileInUse;
        /// <summary>
        /// 预设信息
        /// </summary>
        public List<Profile> profiles;
    }
}
