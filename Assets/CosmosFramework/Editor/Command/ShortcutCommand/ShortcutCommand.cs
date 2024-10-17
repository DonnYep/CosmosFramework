using UnityEditor;
using UnityEngine;
namespace Krypton.Editor
{
    /// <summary>
    /// 快捷键命令
    /// </summary>
    public class ShortcutCommand
    {
        [MenuItem("Window/Cosmos/Command/OpenPersistentDataPath")]
        public static void OpenPersistentDataPath()
        {
            string path = Application.persistentDataPath;
            EditorUtility.RevealInFinder(path);
        }
    }
}
