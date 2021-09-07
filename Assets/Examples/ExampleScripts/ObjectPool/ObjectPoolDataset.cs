using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos{
    /// <summary>
    /// 原则上打包发布后时不允许修改的，因此所有继承此类的子类都只有只读属性
    /// </summary>
    public abstract class ObjectPoolDataset : DatasetBase
    {
        public virtual GameObject  SpawnObject { get; }
        [SerializeField]
        [Range(0,100,order =1)]
        protected short objectOdds = 25;
        public bool ObjectAddsResult
        {
            get
            {
                if (objectOdds == 0)
                    return false;
                var result = Utility.Unity.Random(1, 100);
                if (result <=objectOdds)
                    return true;
                else
                    return false;
            }
        }
        [Header("是否使用群组生成，false默认返回1")]
        [SerializeField]
        protected bool groupSpawn = false;
        [SerializeField]
        [Range(0, 20)]
        protected short minSpawnCount=0;
        [SerializeField]
        [Range(0, 20)]
        protected short maxSpawnCount=1;
        [Header("是否使用回收区间，false默认使用最小回收时间")]
        [SerializeField] protected bool rangeCollectDelay = false;
        [SerializeField]
        [Range(0.1f,900f,order =1)]
        protected float minCollectDelay = 3;
        [SerializeField]
        [Range(0.1f, 900f, order = 1)]
        protected float maxCollectDelay = 3;
        public float CollectDelay
        {
            get
            {
                if (rangeCollectDelay)
                {
                    if (minCollectDelay >= maxCollectDelay)
                    {
                        minCollectDelay = maxCollectDelay;
                        return maxCollectDelay;
                    }
                    else return Utility.Unity.Random(minCollectDelay, maxCollectDelay);
                }
                else return minCollectDelay;
            }
        }
        /// <summary>
        /// 产生对象数量，非群组产生则默认数量为1
        /// </summary>
        public virtual int  SpawnCount
        {
            get
            {
                if (groupSpawn)
                {
                    if (minSpawnCount >= maxSpawnCount)
                    {
                        minSpawnCount = maxSpawnCount;
                        return maxSpawnCount;
                    }
                    else return  Utility.Unity.Random(minSpawnCount, maxSpawnCount);
                }
                else return 1;
            }
        }
        [Header("是否在生成完毕后再进行相对父物体的随机旋转")]
        [SerializeField]
        protected bool randomRotationOnSpawn = false;
        public bool RandomRotationOnSpawn { get { return randomRotationOnSpawn; } }
        [Header("生成对象后对齐的类型")]
        [SerializeField]
        protected ObjectSpawnAlignType alignType;
        public ObjectSpawnAlignType AlignType { get { return alignType; } protected set { alignType = value; } }
        public Transform SpawnTransorm { get; set; }
    }
    /// <summary>
    /// 对象生成时对齐的类型
    /// </summary>
    public enum ObjectSpawnAlignType:int
    {
        /// <summary>
        /// AlignTransform表示全部对齐
        /// </summary>
        AlignTransform = 0,
        AlignPosition=1,
        AlignPositionRotation=2,
        AlignPositionScale=3,
        AlignRotationScale=4,
    }
}