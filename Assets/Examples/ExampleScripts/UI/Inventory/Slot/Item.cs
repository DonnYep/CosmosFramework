using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PureMVC;

using Cosmos.UI;
namespace Cosmos.Test
{
    public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        Image imgItem;
        Text txtNumber;
        ItemDataset itemDataSet;
        public ItemDataset ItemDataSet { get { return itemDataSet; } }
        Transform previouseParent;
        Transform dragParent;
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
            MVC.Dispatch(new NotifyArgs(MVCEventDefine.CMD_Inventory, new InvMsg(InvCmd.Flush)));
        }
        public void SetItem(ItemDataset item)
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
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(IItemClick);
            imgItem = GetComponent<Image>();
            txtNumber = GetComponentInChildren<Text>();
            PreviouseParent = transform.parent;
        }
        void IItemClick()
        {
            MVC.Dispatch(new NotifyArgs(MVCEventDefine.CMD_Inventory, new InvMsg(InvCmd.ShowDescription, itemDescription)));
        }
    }
}
