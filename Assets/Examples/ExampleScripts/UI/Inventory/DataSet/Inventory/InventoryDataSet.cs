using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
/// <summary>
/// 暂时不抽象，这个可能是背包，也可能是仓库，具体遇到再实现
/// </summary>
[CreateAssetMenu(fileName = "NewInventory", menuName = "CosmosFramework/Implement/InventoryDataSet/Inventory")]
public class InventoryDataSet : CFDataSet
{
    [SerializeField]
     int inventoryCapacity = 0;
    public int InventoryCapacity { get { return inventoryCapacity; } set { inventoryCapacity = value; } }
    [SerializeField]
    List<ItemDataSet> itemDataSets = new List<ItemDataSet>();
    public List<ItemDataSet> ItemDataSets { get { return itemDataSets; } set { itemDataSets = value; } }
    public override void Reset()
    {
        itemDataSets.Clear();
        inventoryCapacity = 0;
    }
    public void AddItemDataSet(ItemDataSet item)
    {
        if(itemDataSets.Contains(item))
        {
            item.IncrementItemNumber();
        }
        else
        {
            if (itemDataSets.Count >= inventoryCapacity)
                return;
            itemDataSets.Add(item);
            item.IncrementItemNumber();
        }
    }
}
