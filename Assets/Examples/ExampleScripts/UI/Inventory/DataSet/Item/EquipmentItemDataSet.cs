using UnityEngine;
using System.Collections;
using System;
[CreateAssetMenu(fileName = "NewEquipmentItem", menuName = "CosmosFramework/Implement/ItemDataSet/EquipmentItem")]
[Serializable]
public class EquipmentItemDataset : ItemDataset
{
    public override void Dispose()
    {
        itemImage = null;
        itemNumber = 0;
    }
}
