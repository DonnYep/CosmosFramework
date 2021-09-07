using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.Test
{
    [PrefabAsset("UI/Slot")]
    public class Slot : MonoBehaviour
    {
        public void SetupSlot(ItemDataset item)
        {
            var itemComp = transform.Find("Item").GetComponent<Item>();
            itemComp.SetItem(item);
        }
        public ItemDataset GetDateSet()
        {
            var itemComp = transform.Find("Item").GetComponent<Item>();
            return itemComp.ItemDataSet;
        }
    }
}