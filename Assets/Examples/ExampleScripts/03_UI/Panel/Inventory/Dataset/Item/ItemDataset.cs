using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System;
[Serializable]
public abstract class ItemDataset : DatasetBase
{
    [SerializeField]protected Sprite itemImage;
    public Sprite ItemImage { get { return itemImage; } set { itemImage = value; } }
    [SerializeField]protected int itemNumber;
    public int ItemNumber { get { return itemNumber; }set { itemNumber = value; } }
    [TextArea]
    [SerializeField]
    protected string description;
    public string Description { get { return description; } set { description = value; } }
    /// <summary>
    /// 增量操作
    /// </summary>
    public void IncrementItemNumber()
    {
        itemNumber++;
        description = null;
    }
}
