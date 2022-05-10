using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.Editor.AutocompleteSearchField
{
    public class GitHubSearch : EditorWindow
    {
        [MenuItem("Window/Cosmos/Searchbar/GitHub Search")]
        static void Init()
        {
            GetWindow<GitHubSearch>("GitHub Search").Show();
        }
        [SerializeField]
        AutocompleteSearchField autocompleteSearchField;

#if UNITY_2019_1_OR_NEWER
        UnityWebRequest activeWWW;
#elif UNITY_2017_1_OR_NEWER
		WWW activeWWW;
#endif
        void OnEnable()
        {
            if (autocompleteSearchField == null) autocompleteSearchField = new AutocompleteSearchField();
            autocompleteSearchField.onInputChanged = OnInputChanged;
            autocompleteSearchField.onConfirm = OnConfirm;
            EditorApplication.update += OnUpdate;
        }

        void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        void OnGUI()
        {
            GUILayout.Label("Search GitHub", EditorStyles.boldLabel);
            autocompleteSearchField.OnGUI();
        }

        void OnInputChanged(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return;

            autocompleteSearchField.ClearResults();
            var query = string.Format("https://api.github.com/search/repositories?q={0}&sort=stars&order=desc", searchString);

#if UNITY_2019_1_OR_NEWER
            activeWWW = new UnityWebRequest(query);
#elif UNITY_2018_1_OR_NEWER
			activeWWW = new WWW(query);
#endif
        }

        void OnConfirm(string result)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(autocompleteSearchField.searchString);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        void OnUpdate()
        {
            if (activeWWW != null && activeWWW.isDone)
            {
                if (string.IsNullOrEmpty(activeWWW.error))
                {
                    const string url = "html_url";

                    autocompleteSearchField.ClearResults();

#if UNITY_2019_1_OR_NEWER
                    var text = activeWWW.downloadHandler.text;
#elif UNITY_2018_1_OR_NEWER
	                var text = activeWWW.text;
#endif

                    // Hacky json "parsing"
                    foreach (var line in text.Split('\n'))
                    {
                        var nameIndex = line.IndexOf(url, StringComparison.InvariantCulture);
                        if (nameIndex > 0)
                        {
                            var result = line.Substring(nameIndex + url.Length + 1).Split('"')[1];
                            autocompleteSearchField.AddResult(result);
                        }
                    }

                    Debug.Log(text.Split('\n').Length);
                }
                else
                {
                    Debug.LogError("Error: " + activeWWW.error);
                }
                activeWWW = null;
            }
        }
    }
}