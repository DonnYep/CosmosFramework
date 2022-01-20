﻿using UnityEngine;
using System.Collections;
using System;
[CreateAssetMenu(fileName = "NewNormalItem", menuName = "CosmosFramework/Implement/ItemDataSet/NormalItem")]
[Serializable]
public class NormalItemDataSet : ItemDataSet
{
    public override void Dispose()
    {
        itemImage = null;
        itemNumber = 0;
    }
}
