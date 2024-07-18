using UnityEngine;
using UnityEditor;
namespace Cosmos.Editor
{
    /// <summary>
    /// 子类也继承按钮
    /// </summary>
    [CustomEditor(typeof(DatasetBase), true)]
    public class DatasetBaseEditor : UnityEditor.Editor
    {
        DatasetBase dataset;
        private void OnEnable()
        {
            dataset = target as DatasetBase;
        }
        public override void OnInspectorGUI()
        {
            OnGUI();
            base.OnInspectorGUI();
        }
        /// <summary>
        /// 绘制公共功能按钮
        /// </summary>
        protected virtual void OnGUI()
        {
            EditorGUILayout.HelpBox("Preview预览，Reset按钮执行清空下列所有数值", MessageType.Info, true);
            //GUILayout.Label("Preview预览，Reset按钮执行清空下列所有数值");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview", GUILayout.Height(20)))
            {
                dataset.Preview();
            }
            if (GUILayout.Button("Dispose", GUILayout.Height(20)))
            {
                var canReset = UnityEditor.EditorUtility.DisplayDialog("Reset ScriptableObject", "You will reset ScriptableObject Properties", "Reset", "Cancel");
                if (canReset)
                    dataset.Dispose();
            }
            GUILayout.EndHorizontal();
        }
    }
}