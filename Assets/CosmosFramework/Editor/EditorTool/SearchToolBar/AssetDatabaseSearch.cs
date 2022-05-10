using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.AutocompleteSearchField
{
    public class AssetDatabaseSearch : EditorWindow
    {
        [MenuItem("Window/Cosmos/Searchbar/AssetDatabase Search")]
        static void Init()
        {
            GetWindow<AssetDatabaseSearch>("AssetDatabase Search").Show();
        }

        [SerializeField]
        AutocompleteSearchField autocompleteSearchField;

        void OnEnable()
        {
            if (autocompleteSearchField == null) autocompleteSearchField = new AutocompleteSearchField();
            autocompleteSearchField.onInputChanged = OnInputChanged;
            autocompleteSearchField.onConfirm = OnConfirm;
        }

        void OnGUI()
        {
            GUILayout.Label("Search AssetDatabase", EditorStyles.boldLabel);
            autocompleteSearchField.OnGUI();
        }

        void OnInputChanged(string searchString)
        {
            autocompleteSearchField.ClearResults();
            if (!string.IsNullOrEmpty(searchString))
            {
                foreach (var assetGuid in AssetDatabase.FindAssets(searchString))
                {
                    var result = AssetDatabase.GUIDToAssetPath(assetGuid);
                    if (result != autocompleteSearchField.searchString)
                    {
                        autocompleteSearchField.AddResult(result);
                    }
                }
            }
        }

        void OnConfirm(string result)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(autocompleteSearchField.searchString);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }
}