using System.Collections;
/// <summary>
/// UI implement事件工具类，记录了具体事件的Key
/// </summary>
public sealed class UIIEventDefine
{
    /// <summary>
    /// UI implement 事件，保存inventory数据
    /// </summary>
    public const string UI_IMPL_SAVE_INV = "UI_Impl_SaveInventory";
    /// <summary>
    /// UI implement 事件，读取inventory数据
    /// </summary>
    public const string UI_IMPL_LOAD_INV = "UI_Impl_LoadInventory";
    /// <summary>
    /// 更新slot事件，生成对应数量的slot
    /// </summary>
    public const string UI_IMPL_UPD_SLOT = "UI_Impl_UpdateSlot";
    /// <summary>
    /// 显示item按下显示描述信息事件
    /// </summary>
    public const string UI_IMPL_ITEM_DESC = "UI_Impl_ItemDescription";
    /// <summary>
    /// 更新item交换后数据更新事件，将更新同步到dataSet
    /// </summary>
    public const string UI_IMPL_UPD_ITEM = "UI_Impl_UpdateItem";
}
