using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos{
    /// <summary>
    /// 原则上打包发布后时不允许修改的，因此所有继承此类的子类都只有只读属性
    /// </summary>
    public abstract class ObjectPoolEventObject : CFScriptableObject
    {
        [SerializeField]
        protected GameObject spawnObject;
        public virtual GameObject  SpawnObject { get { return spawnObject; } }
        [SerializeField]
        [Range(0,100,order =1)]
        protected short objectAdds = 25;
        public bool ObjectAddsResult
        {
            get
            {
                if (objectAdds == 0)
                    return false;
                var result = Utility.Random(1, 100);
                if (result <=objectAdds)
                    return true;
                else
                    return false;
            }
        }

        [SerializeField]
        protected bool groupSpawn = false;

        [SerializeField]
        [Range(0, 20)]
        protected short MinSpawnCount=0;
        [SerializeField]
        [Range(0, 20)]
        protected short MaxSpawnCount=1;

        /// <summary>
        /// 产生对象数量，非群组产生则默认数量为1
        /// </summary>
        public virtual int  SpawnCount
        {
            get
            {
                if (groupSpawn)
                {
                    if (MinSpawnCount > MaxSpawnCount)
                        MinSpawnCount = MaxSpawnCount;
                    var result = Utility.Random(MinSpawnCount, MaxSpawnCount);
                    return result;
                }
                else
                    return 1;
            }
        }
        [SerializeField]
        protected bool randomRotationOnSpawn = false;
        public bool RandomRotationOnSpawn { get { return randomRotationOnSpawn; } }

        public Transform SpawnTransorm { get; set; }
    }
}