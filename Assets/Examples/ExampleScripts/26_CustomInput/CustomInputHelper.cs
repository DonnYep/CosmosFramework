using UnityEngine;
using Cosmos;
using Cosmos.Input;

/// <summary>
/// 自定义输入示例：用“绑定表”声明「虚拟名 ← 物理键」，不再手写轮询与注册/注销。
/// <para>新增一个键只需要加一行 <see cref="LegacyInputBinding.Button"/>；</para>
/// <para>需要手柄/触屏/新输入系统时，改去实现 IInputHelper（或在其上包一层）。</para>
/// </summary>
public class CustomInputHelper : LegacyBindingInputHelper
{
    public CustomInputHelper()
        : base(
            LegacyInputBinding.Button(CustomButtonName._Space, KeyCode.Space),
            LegacyInputBinding.Button(CustomButtonName._W, KeyCode.W),
            LegacyInputBinding.Button(CustomButtonName._A, KeyCode.A),
            LegacyInputBinding.Button(CustomButtonName._S, KeyCode.S),
            LegacyInputBinding.Button(CustomButtonName._D, KeyCode.D))
    {
    }
}
