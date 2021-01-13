using System.Collections;
/// <summary>
/// 事件定义类，记录了具体事件的Key
/// </summary>
public sealed class UIEventDefine
{



    public const string VM_Inventory = "VM_INV";
    public const string VM_Slot = "VM_SLOT";
    public const string VM_Item = "VM_ITEM";
    public const string VM_Navigate = "VM_NAV";



    /// <summary>
    /// UI implement 事件，保存inventory数据
    /// </summary>
    public const string UI_SAVE_INV = "UI_SaveInventory";
    /// <summary>
    /// UI implement 事件，读取inventory数据
    /// </summary>
    public const string UI_LOAD_INV = "UI_LoadInventory";
    /// <summary>
    /// 更新slot事件，生成对应数量的slot
    /// </summary>
    public const string UI_UPD_SLOT = "UI_UpdateSlot";
    /// <summary>
    /// 显示item按下显示描述信息事件
    /// </summary>
    public const string UI_ITEM_DESC = "UI_ItemDescription";
    /// <summary>
    /// 更新item交换后数据更新事件，将更新同步到dataSet
    /// </summary>
    public const string UI_UPD_ITEM = "UI_UpdateItem";

    public const string UI_Navigate = "UI_Navigate";

}
