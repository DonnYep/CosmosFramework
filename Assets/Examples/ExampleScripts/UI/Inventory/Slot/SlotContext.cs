using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.Resource;

namespace Cosmos.Test
{
    public class SlotContext : MonoBehaviour
    {
        GameObject slotPrefab;
        List<Slot> slotList = new List<Slot>();
        IResourceManager resourceManager;
        public void UpdateDataSet(InventoryDataset invDataSet)
        {
            var slots = GetComponentsInChildren<Slot>();
            for (int i = 0; i < slots.Length; i++)
            {
                invDataSet.ItemDataSets[i] = slots[i].GetDateSet();
            }
        }
        public void UpdateSlot(InventoryDataset invDataSet)
        {
            if (slotList.Count == 0)
            {
                for (int i = 0; i < invDataSet.InventoryCapacity; i++)
                {
                    var go = Instantiate(slotPrefab, transform);
                    go.name = "Slot";
                    slotList.Add(go.GetComponent<Slot>());
                }
            }
            else if (slotList.Count > invDataSet.InventoryCapacity)
            {
                for (int i = 0; i < slotList.Count; i++)
                {
                    if (i >= invDataSet.ItemDataSets.Count)
                    {
                        GameObject.Destroy(slotList[i].gameObject);
                    }
                }
                slotList.RemoveRange(invDataSet.ItemDataSets.Count, slotList.Count - invDataSet.ItemDataSets.Count);
            }
            else if (slotList.Count < invDataSet.InventoryCapacity)
            {
                for (int i = 0; i < invDataSet.InventoryCapacity; i++)
                {
                    if (i >= slotList.Count)
                    {
                        var go = Instantiate(slotPrefab, transform);
                        go.name = "Slot";
                        slotList.Add(go.GetComponent<Slot>());
                    }
                }
            }
        }
        public void UpdateItem(InventoryDataset invDataSet)
        {
            for (int i = 0; i < invDataSet.ItemDataSets.Count; i++)
            {
                slotList[i].SetupSlot(invDataSet.ItemDataSets[i]);
            }
        }
        protected void Awake()
        {
            resourceManager = CosmosEntry.ResourceManager;
            slotPrefab = resourceManager.LoadPrefab(typeof(Slot));
        }
    }
}