using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cosmos.Mvvm;
using Cosmos.UI;
namespace Cosmos.Test
{
    public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        Image imgItem;
        Text txtNumber;
        ItemDataSet itemDataSet;
        public ItemDataSet ItemDataSet { get { return itemDataSet; } }
        Transform previouseParent;
        Transform dragParent;
        MED_Inventory med_Inventory;
        public Transform DragParent { get { return dragParent; } set { dragParent = value; } }
        string itemDescription;
        public Transform PreviouseParent { get { return previouseParent; } set { previouseParent = value; } }
        public bool Dragable { get { return !(imgItem.color.a == 0 || txtNumber.enabled == false); } }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Dragable)
                return;
            transform.position = eventData.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            DragParent = transform.parent.parent.parent;
            PreviouseParent = transform.parent;
            transform.parent = DragParent;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!Dragable)
                return;
            transform.position = eventData.position;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!Dragable)
                return;
            if (eventData.pointerCurrentRaycast.gameObject.name == "Item")
            {
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform.parent);
                eventData.pointerCurrentRaycast.gameObject.transform.SetParent(PreviouseParent);
                eventData.pointerCurrentRaycast.gameObject.transform.ResetLocalTransform();
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                transform.ResetLocalTransform();
            }
            else
            if (eventData.pointerCurrentRaycast.gameObject.name == "Slot")
            {
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                transform.ResetLocalTransform();
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                transform.SetParent(PreviouseParent);
                transform.ResetLocalTransform();
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            med_Inventory.FlushDataSet();
        }
        public void SetItem(ItemDataSet item)
        {
            if (item == null)
            {
                itemDataSet = null;
                imgItem.sprite = null;
                imgItem.color = Color.clear;
                txtNumber.text = null;
                txtNumber.enabled = false;
                itemDescription = null;
            }
            else
            {
                itemDataSet = item;
                imgItem.sprite = item.ItemImage;
                imgItem.color = Color.white;
                txtNumber.text = item.ItemNumber.ToString();
                txtNumber.enabled = true;
                itemDescription = item.Description;
            }
        }
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(IItemClick);
            imgItem = GetComponent<Image>();
            txtNumber = GetComponentInChildren<Text>();
            PreviouseParent = transform.parent;
             med_Inventory=MVVM.PeekMediator<MED_Inventory>(MVVMDefine.MED_Inventory);
        }
        void IItemClick()
        {
            med_Inventory.DisplayItemDesc(itemDescription);
        }
    }
}