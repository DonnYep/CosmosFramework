using UnityEngine;
using System.Collections;
namespace Cosmos
{
    public abstract class CharacterStatusDataSet : CFDataSet
    {
        [SerializeField] protected string statusName = "NewCharacterStatus";
        public string StatusName { get { return statusName; } }
        //参考OW的生命条设计，仅为初始设置
        [SerializeField] [Range(150,600)] protected int heath=200;
        public int Health { get { return heath; } }
        //待测试
        [SerializeField] [Range(0,20)] protected float moveSpeed=10;
        public float MoveSpeed { get { return moveSpeed; } }
    }
}