using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Cosmos.CosmosEditor
{
    enum Mode
    {
        Browser,
        Builder,
        Inspect,
    }
    public class IntegrateWindow : EditorWindow
    {

        Mode m_Mode;
        bool multiDataSource;
        SearchField m_searchField;
        string searchStr;
        string[] labels = new string[3] { "Configure", "Build", "Inspect" };

        public IntegrateWindow()
        {
            this.titleContent = new GUIContent("Integrate");
        }
        [MenuItem("Window/Cosmos/Integrate")]
        public static void OpenIntegrateWindow()
        {
            var window = GetWindow<IntegrateWindow>();
            ((EditorWindow)window).maxSize = CosmosEditorUtility.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = CosmosEditorUtility.CosmosDevWinSize;
        }
        private void OnEnable()
        {
            m_searchField = new SearchField();
        }
        private Rect GetSubWindowArea()
        {
            float padding = 32;
            if (multiDataSource)
                padding += 32 * 0.5f;
            Rect subPos = new Rect(0, padding, position.width, position.height - padding);
            return subPos;
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            float toolbarWidth = position.width - 64;
            m_Mode = (Mode)GUILayout.Toolbar((int)m_Mode, labels, "LargeButton", GUILayout.Width(toolbarWidth));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("Bundle Data Source:");
                GUILayout.FlexibleSpace();
                var c = new GUIContent("cosmos");
                if (GUILayout.Button(c, EditorStyles.toolbarPopup))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int index = 0; index < 16; index++)
                    {



                    }

                    menu.ShowAsContext();

                }
                searchStr = m_searchField.OnGUI(new Rect(), "Search");
                GUILayout.EndHorizontal();
            }
        }
    }
}