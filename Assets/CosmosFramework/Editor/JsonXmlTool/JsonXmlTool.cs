using UnityEngine;
using System.Collections;
using System.Collections.Generic;
# if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cosmos.CosmosEditor
{
    /// <summary>
    /// 创建一个XML文件，将 Resources文件夹中的文件进行记录
    /// </summary>
    public class JsonXmlTool : EditorWindow
    {
        [MenuItem("Cosmos/JsonXmlTool")]
        public static void ResourceBuildWindow()
        {
            OpenWindow();
        }
        JsonXmlTool()
        {
            this.titleContent = new GUIContent("JsonXml");
        }
        public static void OpenWindow()
        {
            var window = GetWindow<JsonXmlTool>();
            ((EditorWindow)window).maxSize = DebugTool.CosmosDevWinSize;
            ((EditorWindow)window).minSize = DebugTool.CosmosDevWinSize;
        }
        string xmlResourcePath;
        bool showBuildXmlButton;
        string xmlOutputPath;
        string xmlName;

        string jsonResourcePath;
        bool showBuildJsonButton;
        string jsonOutputPath;
        string jsonFileName;

        IO.FileOperator fo = new IO.FileOperator();
        int selectBar;
        void OnGUI()
        {
            selectBar= GUILayout.Toolbar(selectBar, new[] { "XmlFileCreater", "JsonFileCreater" }, GUILayout.Height(24));
            GUILayout.Space(16);
            switch (selectBar)
            {
                case 0:
                    DrawXml();
                    break;
                case 1:
                    DrawJson();
                    break;
            }
        }

        void DrawXml()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            GUI.color = Color.green;
            EditorGUILayout.HelpBox("XMLFileCreater", MessageType.None, true);
            GUI.color = Color.white;
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("指定一个文件夹，生成一个xml文件映射表，若为空，则默认从Assets根目录读取。示范Cosmos\\Resources或Cosmos", MessageType.Info);
            EditorGUILayout.Space();
            xmlResourcePath = EditorGUILayout.TextField("LoadPath", xmlResourcePath);
            GUILayout.Space(6);
            EditorGUILayout.HelpBox("指定一个文件夹，将生成的XML文件保存到指定文件夹。若为空，则路径为Asset\\Resources。示范：Cosmos\\Resources", MessageType.Info);
            xmlOutputPath = EditorGUILayout.TextField("OutputPath", xmlOutputPath);
            EditorGUILayout.HelpBox("输出的xml文件名称，默认文件名为ResourcesConfig.xml，仅输入名称，无需输入扩展名", MessageType.Info);
            GUILayout.BeginHorizontal();
            xmlName = EditorGUILayout.TextField("FileName", xmlName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(512), GUILayout.MinWidth(256));
            if (GUILayout.Button("Build", GUILayout.Height(32)))
            {
                BuildXml();
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Clear", GUILayout.Height(32)))
            {
                ClearXML();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        void DrawJson()
        {

            GUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox("JosnFileCreater", MessageType.None, true);
            GUI.color = Color.white;
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("临时调试的Json工具，开发中", MessageType.Warning);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("指定，生成一个json文件映射表", MessageType.Info);
            jsonResourcePath = EditorGUILayout.TextField("ResourcePath", jsonResourcePath);
            GUILayout.Space(6);
            EditorGUILayout.HelpBox("指定一个文件夹，将生成的Json文件保存到指定文件夹。若为空，则路径为Asset\\Resources。示范：Cosmos\\Resources", MessageType.Info);
            jsonOutputPath = EditorGUILayout.TextField("JsonOutputPath", jsonOutputPath);
            GUILayout.Space(6);
            EditorGUILayout.HelpBox("输出的Json文件名称，仅输入名称，无需输入扩展名", MessageType.Info);
            jsonFileName = EditorGUILayout.TextField("JsonFileName", jsonFileName);
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(512), GUILayout.MinWidth(256));
            if (GUILayout.Button("Build", GUILayout.Height(32)))
            {
                BuildJson();
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Clear", GUILayout.Height(32)))
            {
                ClearJson();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        #region XML
        void BuildXml()
        {
            fo.ParseDefaultFileExtentionsList();
            //执行的时候会默认删除原有的默认名称的xml文件
            fo.LoadRelativePathAsset(xmlResourcePath);
            fo.OutputXmlFile(xmlOutputPath, xmlName);
            Utility.RefreshEditor();
        }
        void ClearXML()
        {
            fo.DeleteXmlFile(xmlOutputPath, xmlName);
            Utility.RefreshEditor();
        }
        #endregion
        void BuildJson()
        {
            string fullJsonFileName = jsonFileName + ".json";
            fo.CreateEmptyJsonFile(Utility.CombineAbsoluteFilePath(fullJsonFileName, jsonOutputPath));
            Utility.DebugLog("JsonFile is created !");
            Utility.RefreshEditor();
        }
        void ClearJson()
        {
            string fullJsonFileName = jsonFileName + ".json";
            IO.FileOperator.DeleteFile(Utility.CombineAbsoluteFilePath(fullJsonFileName, jsonOutputPath));
            Utility.DebugLog("JsonFile is deleted !");
            Utility.RefreshEditor();
        }
    }
}