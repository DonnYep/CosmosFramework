using UnityEngine;
using System.Collections;
using Cosmos;
/// <summary>
/// UI implement事件工具类，记录了具体事件的Key
/// </summary>
public sealed class UIImplementCodeParams
{
    /// <summary>
    /// UI implement 事件，保存inventory数据
    /// </summary>
    public const string UIIMPLEMENT_SAVEINVENTORY = "UIImplement_SaveInventory";
    /// <summary>
    /// UI implement 事件，读取inventory数据
    /// </summary>
    public const string UIIMPLEMENT_LOADINVENTORY = "UIImplement_LoadInventory";
    /// <summary>
    /// 更新slot事件，生成对应数量的slot
    /// </summary>
    public const string UIIMPLEMENT_UPDATESLOT = "UIImplement_UpdateSlot";
    /// <summary>
    /// 显示item按下显示描述信息事件
    /// </summary>
    public const string UIIMPLEMENT_ITEMDESCRIPTION = "UIImplement_ItemDescription";
    /// <summary>
    /// 更新item交换后数据更新事件，将更新同步到dataSet
    /// </summary>
    public const string UIIMPLEMENT_UPDATEITEM = "UIImplement_UpdateItem";
}
