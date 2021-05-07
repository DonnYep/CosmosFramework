using UnityEngine;
using System.Collections;
using System;
[CreateAssetMenu(fileName = "NewEquipmentItem", menuName = "CosmosFramework/Implement/ItemDataSet/EquipmentItem")]
[Serializable]
public class EquipmentItemDataSet : ItemDataSet
{
    public override void Dispose()
    {
        itemImage = null;
        itemNumber = 0;
    }
}
