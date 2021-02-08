using System.Collections;
/// <summary>
/// 事件定义类，记录了具体事件的Key
/// </summary>
public sealed class MVVMDefine
{



    public const string CMD_Inventory = "CMD_INV";
    public const string CMD_Slot = "CMD_SLOT";
    public const string CMD_Item = "CMD_ITEM";
    public const string CMD_Navigate = "CMD_NAV";

    public const string PRX_Inventory = "PRX_Inventory";

    public const string MED_Inventory = "MED_Inventory";
    public const string MED_Slot = "MED_Slot";


    /// <summary>
    /// 显示item按下显示描述信息事件
    /// </summary>
    public const string UI_ITEM_DESC = "UI_ItemDescription";
}
