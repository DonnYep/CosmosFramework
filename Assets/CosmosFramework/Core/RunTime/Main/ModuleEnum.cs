namespace Cosmos
{
    /// <summary>
    /// 使用常量存储了常用的事件
    /// 分部类，可以进行后期拓展
    /// 常量名等于内容string
    /// </summary>
    public enum ModuleEnum:byte
    {
        Audio=0,
        Mono=1,
        ObjectPool=2,
        Resource=3,
        UI=4,
        Event=5,
        Entity=6,
        Input=7,
        FSM=8,
        Network=9,
        Scene=10,
        Config=11,
        Data=12,
        Controller=13,
        ReferencePool=14,
        Hotfix=15
    }
}