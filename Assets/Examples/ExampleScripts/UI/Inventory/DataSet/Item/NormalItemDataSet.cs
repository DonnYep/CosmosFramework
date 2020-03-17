using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewNormalItem", menuName = "CosmosFramework/Implement/ItemDataSet/NormalItem")]
public class NormalItemDataSet : ItemDataSet
{
    public override void Reset()
    {
        itemImage = null;
        itemNumber = 0;
    }
}
