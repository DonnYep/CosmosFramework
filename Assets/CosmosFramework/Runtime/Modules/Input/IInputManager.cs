using UnityEngine;
namespace Cosmos.Input
{
    //================================================
    /*
     * 1、输入模块，适配不同平台的按键输入事件。
     *
     * 2、只做“后端无关的薄门面”：虚拟按键/轴线 + 输入闸门（上下文屏蔽）+ 判定边沿。
     *    具体按键怎么映射由 IInputHelper 决定（旧 Input Manager / 新 Input System / 插件都可以接）。
     *
     * 3、输入闸门用于“UI 打开时屏蔽游戏输入”“过场动画屏蔽全部输入”这类框架级需求：
     *    PushInputBlock/PopInputBlock 成对使用，可嵌套，查询时统一返回中性值。
     */
    //================================================
    public interface IInputManager : IModuleManager
    {
        /// <summary>
        /// 虚拟轴线数量。
        /// </summary>
        int VirtualAxisCount { get; }
        /// <summary>
        /// 虚拟按钮数量。
        /// </summary>
        int VirtualButtonCount { get; }
        /// <summary>
        /// 鼠标位置（不受输入闸门影响：位置不是“动作”，UI 跟随仍需要它）。
        /// </summary>
        Vector3 MousePosition { get; }
        /// <summary>
        /// 是否启用虚拟输入。
        /// <para>关闭时会清空所有按键/轴线状态，重新开启后由帮助体重新推送，避免出现“卡住的按键”。</para>
        /// </summary>
        bool IsEnableInputDevice { get; set; }
        /// <summary>
        /// 当前是否被输入闸门屏蔽。
        /// </summary>
        bool IsInputBlocked { get; }
        /// <summary>
        /// 当前闸门数量。
        /// </summary>
        int InputBlockCount { get; }
        /// <summary>
        /// 设置自定义的输入帮助体。
        /// </summary>
        /// <param name="helper">自定义的输入帮助体</param>
        void SetInputHelper(IInputHelper helper);
        /// <summary>
        /// 压入一个输入闸门（同一个 owner 重复压入只算一次）。
        /// <para>闸门开启期间：轴线读 0、按键读 false，帮助体仍会继续刷新原始状态。</para>
        /// </summary>
        /// <param name="owner">闸门持有者（通常是发起屏蔽的 UI/流程对象）</param>
        void PushInputBlock(object owner);
        /// <summary>
        /// 弹出一个输入闸门。
        /// </summary>
        /// <param name="owner">闸门持有者</param>
        /// <returns>是否弹出成功</returns>
        bool PopInputBlock(object owner);
        /// <summary>
        /// 清空所有输入闸门（场景切换等兜底场景使用）。
        /// </summary>
        void ClearInputBlocks();
        /// <summary>
        /// 清空所有虚拟按键与轴线状态。
        /// </summary>
        void ResetAllVirtualStates();
        /// <summary>
        /// 虚拟轴线是否存在。
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualAxis(string name);
        /// <summary>
        /// 虚拟按键是否存在。
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualButton(string name);
        /// <summary>
        /// 注册虚拟按键。
        /// </summary>
        /// <param name="name">按键名称</param>
        void RegisterVirtualButton(string name);
        /// <summary>
        /// 注销虚拟按键。
        /// </summary>
        /// <param name="name">按键名称</param>
        void DeregisterVirtualButton(string name);
        /// <summary>
        /// 注册虚拟按键。
        /// </summary>
        /// <param name="name">按键名称</param>
        void RegisterVirtualAxis(string name);
        /// <summary>
        /// 注销虚拟轴线。
        /// </summary>
        /// <param name="name">按键名称</param>
        void DeregisterVirtualAxis(string name);


        /// <summary>
        /// 获得轴线。
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>数值</returns>
        float GetAxis(string name);
        /// <summary>
        /// 未插值的输入 -1，0 ，1。
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>数值</returns>
        float GetAxisRaw(string name);
        /// <summary>
        /// 按钮按下：仅在状态由抬起到按下的那一帧为 true。
        /// <para>帮助体必须逐帧推送电平（<see cref="SetButtonPressed"/>），边沿由模块自行推导；</para>
        /// <para>模块在 Update 阶段最早执行（MonoGameManager 执行顺序被提前），因此普通脚本的 Update 也能读到同一帧的按下。</para>
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        bool GetButtonDown(string name);
        /// <summary>
        /// 按钮按住。
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        bool GetButton(string name);
        /// <summary>
        /// 按钮抬起：仅在状态由按下到抬起的那一帧为 true。
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        bool GetButtonUp(string name);

        /// <summary>
        /// 电平式设置按钮状态（推荐帮助体使用）：内部自动推导按下/抬起边沿。
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="pressed">是否按下</param>
        void SetButtonPressed(string name, bool pressed);
        /// <summary>
        /// 设置按钮按下。
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonDown(string name);
        /// <summary>
        /// 设置按钮抬起。
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonUp(string name);
        /// <summary>
        /// 设置虚拟鼠标位置。
        /// </summary>
        /// <param name="value">鼠标位置</param>
        void SetVirtualMousePosition(Vector3 value);
        /// <summary>
        /// 设置虚拟鼠标位置。
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        void SetVirtualMousePosition(float x, float y, float z);
        /// <summary>
        /// 设置轴线值为正方向1。
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisPositive(string name);
        /// <summary>
        /// 设置轴线值为负方向-1。
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisNegative(string name);
        /// <summary>
        /// 设置轴线值为0。
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisZero(string name);
        /// <summary>
        /// 设置轴线值。
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        void SetAxis(string name, float value);
    }
}
