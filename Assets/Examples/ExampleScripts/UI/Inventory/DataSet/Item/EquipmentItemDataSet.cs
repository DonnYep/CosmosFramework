using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewEquipmentItem", menuName = "CosmosFramework/Implement/ItemDataSet/EquipmentItem")]
public class EquipmentItemDataSet : ItemDataSet
{
    public override void Reset()
    {
        itemImage = null;
        itemNumber = 0;
    }
}
