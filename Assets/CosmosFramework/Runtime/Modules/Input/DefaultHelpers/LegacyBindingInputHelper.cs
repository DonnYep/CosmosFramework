using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;

namespace Cosmos
{
    /// <summary>
    /// 绑定类型
    /// </summary>
    public enum InputBindingKind : byte
    {
        /// <summary>
        /// 按钮
        /// </summary>
        Button = 0,
        /// <summary>
        /// 轴线
        /// </summary>
        Axis = 1,
    }
    /// <summary>
    /// 一条绑定：虚拟名称 ← 旧输入管理器的键 / 轴。
    /// <para>用绑定表代替“一个按键写一段 if”的样板代码，新增按键只需要加一行数据。</para>
    /// </summary>
    public struct LegacyInputBinding
    {
        /// <summary>
        /// 虚拟按键/轴线名（查询时使用的名字）
        /// </summary>
        public string Name;
        /// <summary>
        /// 绑定类型
        /// </summary>
        public InputBindingKind Kind;
        /// <summary>
        /// 按钮绑定的 KeyCode（Kind == Button 时有效）
        /// </summary>
        public KeyCode Key;
        /// <summary>
        /// 轴线绑定的轴名（Kind == Axis 时有效，需存在于 Project Settings &gt; Input Manager）
        /// </summary>
        public string AxisName;
        /// <summary>
        /// 轴值缩放/取反（默认 1；<see cref="Axis"/> 工厂会把 0 视为 1）
        /// </summary>
        public float Scale;
        /// <summary>
        /// 创建一条按钮绑定
        /// </summary>
        /// <param name="name">虚拟按键名</param>
        /// <param name="key">物理按键</param>
        /// <returns>绑定</returns>
        public static LegacyInputBinding Button(string name, KeyCode key)
        {
            return new LegacyInputBinding()
            {
                Name = name,
                Kind = InputBindingKind.Button,
                Key = key,
                Scale = 1f,
            };
        }
        /// <summary>
        /// 创建一条轴线绑定
        /// </summary>
        /// <param name="name">虚拟轴线名</param>
        /// <param name="axisName">Input Manager 中的轴名</param>
        /// <param name="scale">轴值缩放（-1 可反向）</param>
        /// <returns>绑定</returns>
        public static LegacyInputBinding Axis(string name, string axisName, float scale = 1f)
        {
            return new LegacyInputBinding()
            {
                Name = name,
                Kind = InputBindingKind.Axis,
                AxisName = axisName,
                Scale = scale == 0f ? 1f : scale,
            };
        }
    }
    /// <summary>
    /// 数据驱动的旧输入管理器帮助体。
    /// <para>用法：</para>
    /// <code>
    /// var helper = new LegacyBindingInputHelper(
    ///     LegacyInputBinding.Button("Jump", KeyCode.Space),
    ///     LegacyInputBinding.Button("Attack", KeyCode.Mouse0),
    ///     LegacyInputBinding.Axis("MoveX", "Horizontal"),
    ///     LegacyInputBinding.Axis("LookY", "Mouse Y", -1f));
    /// CosmosEntry.InputManager.SetInputHelper(helper);
    /// </code>
    /// <para>需要手柄/触屏/新输入系统时，实现自己的 IInputHelper，或在此基础上包一层。</para>
    /// </summary>
    public class LegacyBindingInputHelper : IInputHelper
    {
        readonly LegacyInputBinding[] bindings;
        IInputManager inputManager;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="bindings">绑定表</param>
        public LegacyBindingInputHelper(params LegacyInputBinding[] bindings)
            : this((IEnumerable<LegacyInputBinding>)bindings)
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="bindings">绑定表</param>
        public LegacyBindingInputHelper(IEnumerable<LegacyInputBinding> bindings)
        {
            this.bindings = bindings == null ? new LegacyInputBinding[0] : new List<LegacyInputBinding>(bindings).ToArray();
        }
        /// <inheritdoc/>
        public void OnInitialization(IInputManager inputManager)
        {
            this.inputManager = inputManager;
            LegacyInputSafe.ReportUnavailable(nameof(LegacyBindingInputHelper));
            if (inputManager == null)
                return;
            for (int i = 0; i < bindings.Length; i++)
            {
                var binding = bindings[i];
                if (string.IsNullOrEmpty(binding.Name))
                    continue;
                if (binding.Kind == InputBindingKind.Button)
                    inputManager.RegisterVirtualButton(binding.Name);
                else
                    inputManager.RegisterVirtualAxis(binding.Name);
            }
        }
        /// <inheritdoc/>
        public void OnRefresh()
        {
            if (inputManager == null || !LegacyInputSafe.Available)
                return;
            for (int i = 0; i < bindings.Length; i++)
            {
                var binding = bindings[i];
                if (string.IsNullOrEmpty(binding.Name))
                    continue;
                if (binding.Kind == InputBindingKind.Button)
                {
                    //电平推送：按下/抬起边沿由模块推导
                    inputManager.SetButtonPressed(binding.Name, LegacyInputSafe.GetKey(binding.Key));
                }
                else
                {
                    var scale = binding.Scale == 0f ? 1f : binding.Scale;
                    inputManager.SetAxis(binding.Name, LegacyInputSafe.GetAxis(binding.AxisName) * scale);
                }
            }
        }
        /// <inheritdoc/>
        public void OnTermination()
        {
            if (inputManager != null)
            {
                for (int i = 0; i < bindings.Length; i++)
                {
                    var binding = bindings[i];
                    if (string.IsNullOrEmpty(binding.Name))
                        continue;
                    if (binding.Kind == InputBindingKind.Button)
                        inputManager.DeregisterVirtualButton(binding.Name);
                    else
                        inputManager.DeregisterVirtualAxis(binding.Name);
                }
            }
            inputManager = null;
        }
    }
}
