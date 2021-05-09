using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    public class HierachyFinderWindow : EditorWindow{
        [MenuItem("Window/Cosmos/HierachyFinder")]
        public static void OpenWindow()
        {
            var window = GetWindow<HierachyFinderWindow>();
            ((EditorWindow)window).maxSize = CosmosEditorUtility.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = CosmosEditorUtility.CosmosDevWinSize;
        }
       public HierachyFinderWindow()
        {
            this.titleContent = new GUIContent("HierachyFinder");
        }
        Vector2 scriptScrollPos;
        MonoScript scriptObj;
        Transform root;
        List<Transform> result = new List<Transform>();
        int ResultCount = 0;
        Transform findChildRoot;
        int childCount = 0;

        Vector2 monoScrollPos;
        Transform findMonoRoot;
        List<Transform> monoResult = new List<Transform>();
        int monoObjCount;

        Vector2 findTransByNameScroll;
        Transform findTransByNameRoot;
        string findTransName;
        List<Transform> matchedTrans = new List<Transform>();
        List<Transform> nameSubTrans = new List<Transform>();
        int nameMatchedCount = 0;
        bool nameCaseSensitive = false;

        Vector2 findMissingCompScroll;
        Transform findMissingRoot;
        List<Transform> missingCompTrans = new List<Transform>();
        int missCompTransCount;

        int selectedBar =0;
        string[] barArray = new string[] { "MonoReference", "ChildCount", "MonoCount", "FindByName", "MissingComp" };

        private void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray, GUILayout.Height(24));
            GUILayout.Space(16);
            switch (selectedBar)
            {
                case 0:
                    DrawMonoReference();
                    break;
                case 1:
                    DrawGetChild();
                    break;
                case 2:
                    DrawMonoCount();
                    break;
                case 3:
                    DrawFindTransformByName();
                    break;
                case 4:
                    DrawFindMissingComponet();
                    break;
            }
        }
        void DrawMonoReference()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("查找一个脚本在Hierarchy中的引用，这个类必须是继承Mono", MessageType.Info);
            GUILayout.Space(8);
            scriptObj = (MonoScript)EditorGUILayout.ObjectField("SourceScript", scriptObj, typeof(MonoScript), true);
            GUILayout.Space(8);
            root = (Transform)EditorGUILayout.ObjectField("Root", root, typeof(Transform), true);
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find", GUILayout.Height(32)))
            {
                ClearReference();
                FindTarget(root);
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Clear", GUILayout.Height(32)))
            {
                ClearReference();
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("ReferenceCountInScene: " + ResultCount);
            if (result.Count > 0)
            {
                scriptScrollPos = EditorGUILayout.BeginScrollView(scriptScrollPos);

                foreach (Transform t in result)
                {
                    EditorGUILayout.ObjectField(t, typeof(Transform), false);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
       void DrawGetChild()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("查找一个对象拥有的子物体以及孙物体总数", MessageType.Info);
            findChildRoot = (Transform)EditorGUILayout.ObjectField("GetChildRoot", findChildRoot, typeof(Transform), true);
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find", GUILayout.Height(32)))
            {
                ClearChild();
                GetChild(findChildRoot);
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Clear", GUILayout.Height(32)))
            {
                ClearChild();
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("ChildCount: " + childCount);
            GUILayout.EndVertical();
        }
       void DrawMonoCount()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("查找对象上子物体以及孙物体所有挂载的mono脚本", MessageType.Info);
            findMonoRoot = (Transform)EditorGUILayout.ObjectField("GetMonoRoot", findMonoRoot, typeof(Transform), true);
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find", GUILayout.Height(32)))
            {
                monoChildTrans.Clear();
                monoObjCount = 0;
                monoResult.Clear();
                GetMonoCount(findMonoRoot);
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Clear", GUILayout.Height(32)))
            {
                monoObjCount = 0;
                monoResult.Clear();
                monoChildTrans.Clear();
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("MonoCount: " + monoObjCount);

            if (monoResult.Count > 0)
            {
                monoScrollPos = EditorGUILayout.BeginScrollView(monoScrollPos);
                foreach (Transform t in monoResult)
                {
                    EditorGUILayout.ObjectField(t, typeof(Transform), false);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
        void DrawFindTransformByName()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("查找对象上所有与输入字段匹配的对象", MessageType.Info);
            GUILayout.Space(8);
            findTransName = EditorGUILayout.TextField(" find name", findTransName);
            GUILayout.Space(8);
            findTransByNameRoot = (Transform)EditorGUILayout.ObjectField("GetMonoRoot", findTransByNameRoot, typeof(Transform), true);
            GUILayout.Space(8);
            nameCaseSensitive = EditorGUILayout.Toggle("CaseSensitive", nameCaseSensitive);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find", GUILayout.Height(32)))
            {
                matchedTrans.Clear();
                nameSubTrans.Clear();
                nameMatchedCount = 0;
                MatchingName(findTransByNameRoot, findTransName);
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Clear", GUILayout.Height(32)))
            {
                matchedTrans.Clear();
                nameSubTrans.Clear();
                nameMatchedCount = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("MonoCount: " +nameMatchedCount);
            if (matchedTrans.Count > 0)
            {
               findTransByNameScroll = EditorGUILayout.BeginScrollView(findTransByNameScroll);
                foreach (Transform t in matchedTrans)
                {
                    EditorGUILayout.ObjectField(t, typeof(Transform), false);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
        void DrawFindMissingComponet()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("查找对象上子物体以及孙物体所有丢失组件的对象", MessageType.Info);
           findMissingRoot = (Transform)EditorGUILayout.ObjectField("FindMissingRoot", findMissingRoot, typeof(Transform), true);
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find", GUILayout.Height(32)))
            {
                missCompTransCount = 0;
                missingCompTrans.Clear();
                FindMissingCompsIncludeRoot(findMissingRoot);
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Clear", GUILayout.Height(32)))
            {
                missingCompTrans.Clear();
                missCompTransCount = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("MissingCount: " + missCompTransCount);

            if (missingCompTrans.Count > 0)
            {
               findMissingCompScroll = EditorGUILayout.BeginScrollView(findMissingCompScroll);
                foreach (Transform t in missingCompTrans)
                {
                    EditorGUILayout.ObjectField(t, typeof(Transform), false);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
        List<string> targetName = new List<string>();
        void FindTarget(Transform root)
        {
            if(scriptObj == null||root==null)
            {
                Utility.Debug.LogError("Reference script or root empty");
                return;
            }
            MonoScript ms = scriptObj as MonoScript;
            if(ms==null)
            {
                Utility.Debug.LogError("source scripts is not derives form mono");
                return;
            }
            if (root.GetComponent(scriptObj.GetClass()) != null)
            {
                result.Add(root);
                ResultCount++;
            }
            foreach (Transform t in root)
            {
                FindTarget(t);
            }
        }
        void FindTarget()
        {
            if (scriptObj == null)
            {
                Utility.Debug.LogError("Reference script or root empty");
                return;
            }
            var res =(Transform[]) FindObjectsOfType(scriptObj.GetClass());
            foreach (Transform item in res)
            {
                FindTarget(item);
            }
        }
        void ClearReference()
        {
            result.Clear();
            ResultCount = 0;
        }
        void GetChild(Transform root)
        {
            if (root == null)
            {
                Utility.Debug.LogError("root is empty!");
                return;
            }
            foreach (Transform subTrans in root)
            {
                childCount++;
                if (subTrans.childCount > 0)
                {
                    GetChild(subTrans);
                }
            }
        }
        void GetSubChild(Transform root)
        {
            foreach (Transform subTrans in root)
            {
                monoChildTrans.Add(subTrans);
                if (subTrans.childCount > 0)
                {
                    GetSubChild(subTrans);
                }
            }
        }
        List<Transform> monoChildTrans = new List<Transform>();
        void ClearChild()
        {
            childCount = 0;
        }
        void GetMonoCount(Transform root)
        {
            if (root == null)
            {
                Utility.Debug.LogError("root is empty!");
                return;
            }
            List<MonoBehaviour> temp = new List<MonoBehaviour>();
            var rootMonos = root.GetComponents<MonoBehaviour>();
            for (int i = 0; i < rootMonos.Length; i++)
            {
                string typeName = rootMonos[i].GetType().FullName;
                if (!typeName.Contains("UnityEngine"))
                {
                    monoObjCount++;
                    monoResult.Add(rootMonos[i].gameObject.transform);
                }
            }
            GetSubChild(root);
            for (int i = 0; i < monoChildTrans.Count; i++)
            {
                var monos = monoChildTrans[i].GetComponents<MonoBehaviour>();
                for (int j = 0; j < monos.Length; j++)
                {
                    string typeName = monos[j].GetType().FullName;
                    if (!typeName.Contains("UnityEngine"))
                    {
                        monoObjCount++;
                        monoResult.Add(monos[j].gameObject.transform);
                    }
                }
            }
        }
        void GetNameMathchingSubTrans(Transform root)
        {
            foreach (Transform subTrans in root)
            {
                nameSubTrans.Add(subTrans);
                if (subTrans.childCount > 0)
                {
                    GetNameMathchingSubTrans(subTrans);
                }
            }
        }
        void MatchingName(Transform root,string name)
        {
            if (root == null || string.IsNullOrEmpty(name))
            {
                Utility.Debug.LogError("root or name empty!");
                return;
            }
            GetNameMathchingSubTrans(root);
            for (int i = 0; i < nameSubTrans.Count; i++)
            {
                if (nameCaseSensitive)
                {
                    if (nameSubTrans[i].name.Contains(name))
                    {
                        nameMatchedCount++;
                        matchedTrans.Add(nameSubTrans[i]);
                    }
                }
                else
                {
                    if (nameSubTrans[i].name.ToLower().Contains(name.ToLower()))
                    {
                        nameMatchedCount++;
                        matchedTrans.Add(nameSubTrans[i]);
                    }
                }
               
            }
        }


        void FindMissingCompsIncludeRoot(Transform root)
        {
            var rootComps = root.GetComponents<Component>();
            foreach (var rc in rootComps)
            {
                if (rc == null)
                {
                    missCompTransCount++;
                    missingCompTrans.Add(root);
                    break;
                }
            }
            FindMissingComps(root);
        }
        void FindMissingComps(Transform root)
        {
            foreach (Transform subTrans in root)
            {
                var comps = subTrans.GetComponents<Component>();
                foreach (var c in comps)
                {
                    if (c == null)
                    {
                        missCompTransCount++;
                        missingCompTrans.Add(subTrans);
                        break;
                    }
                }
                if (subTrans.childCount > 0)
                {
                    FindMissingComps(subTrans);
                }
            }
        }
    }
}
#endif
