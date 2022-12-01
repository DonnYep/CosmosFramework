using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PureMVC;
using Cosmos;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image imgItem;
    Text txtNumber;
    ItemDataset itemDataSet;
    public ItemDataset ItemDataSet { get { return itemDataSet; } }
    Transform previouseParent;
    Transform dragParent;
    Transform root;
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
        DragParent = root;
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
        bool invalidDrag = false;
        var raycastTarget = eventData.pointerCurrentRaycast.gameObject;
        if (raycastTarget == null)
        {
            transform.SetParent(PreviouseParent);
            transform.ResetLocalTransform();
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            return;
        }
        if (raycastTarget.name == "Item")
        {
            transform.SetParent(raycastTarget.transform.parent);
            raycastTarget.transform.SetParent(PreviouseParent);
            raycastTarget.gameObject.transform.ResetLocalTransform();
            transform.ResetLocalTransform();
            invalidDrag = true;

        }
        else if (raycastTarget.name == "Slot")
        {
            transform.SetParent(raycastTarget.gameObject.transform);
            transform.ResetLocalTransform();
            invalidDrag = true;
        }
        else
        {
            transform.SetParent(PreviouseParent);
            transform.ResetLocalTransform();
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (invalidDrag)
            MVC.Dispatch(new NotifyArgs(MVCEventDefine.CMD_Inventory, new InventoryEventArgs(InvCmd.Flush)));
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
        root = transform.FindParent("Root");
    }
    void IItemClick()
    {
        MVC.Dispatch(new NotifyArgs(MVCEventDefine.CMD_Inventory, new InventoryEventArgs(InvCmd.ShowDescription, itemDescription)));
    }
}
