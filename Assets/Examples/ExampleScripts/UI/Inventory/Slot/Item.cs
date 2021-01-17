using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cosmos;
using Cosmos.UI;
public class Item : UILogicResident, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image imgItem;
    Text txtNumber;
    ItemDataSet itemDataSet;
    public ItemDataSet ItemDataSet { get { return itemDataSet; } }
    LogicEventArgs<string> uip;
    Transform previouseParent;
    Transform dragParent;
    public Transform DragParent { get { return dragParent; } set { dragParent = value; } }
    string itemDescription;
    public Transform PreviouseParent { get { return previouseParent; } set { previouseParent = value; } }
    public bool Dragable{ get { return !(imgItem.color.a == 0 || txtNumber.enabled == false); } }
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
        }else
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
        DispatchUIEvent(UIEventDefine.UI_IMPL_UPD_ITEM, null, null);
    }
    public void SetItem(ItemDataSet item)
    {
        if (item == null)
        {
            itemDataSet =null;
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
    protected override void OnInitialization()
    {
        uip = Facade.SpawnReference<LogicEventArgs<string>>();
        GetUIPanel<Button>("Item").onClick.AddListener(IItemClick);
        imgItem = GetUIPanel<Image>("Item");
        txtNumber= GetUIPanel<Text>("TxtNumber");
        PreviouseParent = transform.parent;
    }
    void IItemClick()
    {
        uip.SetData(itemDescription);
        DispatchUIEvent(UIEventDefine.UI_IMPL_ITEM_DESC, this, uip);
    }
}
