using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 旧输入管理器（<see cref="UnityEngine.Input"/>）的安全封装。
    /// <para>1、工程把 Active Input Handling 设为 “Input System Package (New)” 时，
    /// <see cref="UnityEngine.Input"/> 的每次调用都会抛 InvalidOperationException；
    /// 这里用 <see cref="Available"/> 统一拦截（该宏由 Unity 按 Player Settings 自动定义）。</para>
    /// <para>2、工程把 Project Settings &gt; Input Manager 里的轴删掉时，GetAxis 会抛异常；
    /// 这里捕获并只报一次错，避免每帧刷屏。</para>
    /// </summary>
    internal static class LegacyInputSafe
    {
        /// <summary>
        /// 当前工程是否启用了旧输入管理器
        /// </summary>
        public static bool Available
        {
            get
            {
#if ENABLE_LEGACY_INPUT_MANAGER
                return true;
#else
                return false;
#endif
            }
        }
        static bool reportedUnavailable;
        static readonly HashSet<string> reportedAxisNames = new HashSet<string>();
        /// <summary>
        /// 若旧输入管理器不可用，报一次错（由各帮助体在初始化时调用）
        /// </summary>
        /// <param name="helperName">帮助体名称</param>
        public static void ReportUnavailable(string helperName)
        {
            if (Available || reportedUnavailable)
                return;
            reportedUnavailable = true;
            Utility.Debug.LogError(
                $"{helperName} needs the legacy Input Manager, but the project is set to 'Input System Package (New)' only.\n" +
                "Fix: Player Settings > Active Input Handling = 'Both' (or 'Input Manager (Old)'), " +
                "or implement IInputHelper on top of the new Input System.");
        }
        public static bool GetKey(KeyCode key)
        {
            return Available && UnityEngine.Input.GetKey(key);
        }
        public static bool GetKeyDown(KeyCode key)
        {
            return Available && UnityEngine.Input.GetKeyDown(key);
        }
        public static bool GetKeyUp(KeyCode key)
        {
            return Available && UnityEngine.Input.GetKeyUp(key);
        }
        public static bool GetMouseButton(int button)
        {
            return Available && UnityEngine.Input.GetMouseButton(button);
        }
        public static bool GetMouseButtonDown(int button)
        {
            return Available && UnityEngine.Input.GetMouseButtonDown(button);
        }
        public static bool GetMouseButtonUp(int button)
        {
            return Available && UnityEngine.Input.GetMouseButtonUp(button);
        }
        public static Vector3 MousePosition
        {
            get { return Available ? UnityEngine.Input.mousePosition : Vector3.zero; }
        }
        public static int TouchCount
        {
            get { return Available ? UnityEngine.Input.touchCount : 0; }
        }
        public static Touch GetTouch(int index)
        {
            return Available ? UnityEngine.Input.GetTouch(index) : default(Touch);
        }
        /// <summary>
        /// 安全读取轴线：轴不存在时返回 0 并只报一次错
        /// </summary>
        /// <param name="axisName">轴名称（Project Settings &gt; Input Manager）</param>
        /// <returns>轴值</returns>
        public static float GetAxis(string axisName)
        {
            if (!Available || string.IsNullOrEmpty(axisName))
                return 0f;
            try
            {
                return UnityEngine.Input.GetAxis(axisName);
            }
            catch (System.Exception)
            {
                if (reportedAxisNames.Add(axisName))
                    Utility.Debug.LogError($"Input axis is not configured : {axisName}（Project Settings > Input Manager 中不存在该轴）");
                return 0f;
            }
        }
    }
}
