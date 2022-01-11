using System.Collections.Generic;
using UnityEngine;
using Cosmos;
#if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace CosmosEditor
{
    public class HierachyFinderWindow : EditorWindow{
        [MenuItem("Window/Cosmos/HierachyFinder")]
        public static void OpenWindow()
        {
            var window = GetWindow<HierachyFinderWindow>();
            ((EditorWindow)window).maxSize = EditorUtil.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = EditorUtil.CosmosDevWinSize;
        }
       public HierachyFinderWindow()
        {
            this.titleContent = new GUIContent("HierachyFinder");
        }
        Vector2 m_ScrollPos;

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
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
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
            EditorGUILayout.EndScrollView();
        }
        void DrawMonoReference()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.HelpBox("查找一个脚本在Hierarchy中的引用，这个类必须是继承Mono", MessageType.Info);
            GUILayout.Space(16);
            scriptObj = (MonoScript)EditorGUILayout.ObjectField("SourceScript", scriptObj, typeof(MonoScript), true);
            root = (Transform)EditorGUILayout.ObjectField("Root", root, typeof(Transform), true);
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find"))
            {
                ClearReference();
                FindTarget(root);
            }
            if (GUILayout.Button("Clear"))
            {
                ClearReference();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
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
            GUILayout.BeginVertical();
            EditorGUILayout.HelpBox("查找一个对象拥有的子物体以及孙物体总数", MessageType.Info);
            GUILayout.Space(16);
            findChildRoot = (Transform)EditorGUILayout.ObjectField("GetChildRoot", findChildRoot, typeof(Transform), true);
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find"))
            {
                ClearChild();
                GetChild(findChildRoot);
            }
            if (GUILayout.Button("Clear"))
            {
                ClearChild();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
            GUILayout.Label("ChildCount: " + childCount);
            GUILayout.EndVertical();
        }
       void DrawMonoCount()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.HelpBox("查找对象上子物体以及孙物体所有挂载的mono脚本", MessageType.Info);
            GUILayout.Space(16);
            findMonoRoot = (Transform)EditorGUILayout.ObjectField("GetMonoRoot", findMonoRoot, typeof(Transform), true);
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find"))
            {
                monoChildTrans.Clear();
                monoObjCount = 0;
                monoResult.Clear();
                GetMonoCount(findMonoRoot);
            }
            if (GUILayout.Button("Clear"))
            {
                monoObjCount = 0;
                monoResult.Clear();
                monoChildTrans.Clear();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
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
            GUILayout.BeginVertical();
            EditorGUILayout.HelpBox("查找对象上所有与输入字段匹配的对象", MessageType.Info);
            GUILayout.Space(16);
            findTransName = EditorGUILayout.TextField("Find name", findTransName);
            findTransByNameRoot = (Transform)EditorGUILayout.ObjectField("GetMonoRoot", findTransByNameRoot, typeof(Transform), true);
            nameCaseSensitive = EditorGUILayout.Toggle("CaseSensitive", nameCaseSensitive);
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find"))
            {
                matchedTrans.Clear();
                nameSubTrans.Clear();
                nameMatchedCount = 0;
                MatchingName(findTransByNameRoot, findTransName);
            }
            if (GUILayout.Button("Clear"))
            {
                matchedTrans.Clear();
                nameSubTrans.Clear();
                nameMatchedCount = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
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
            GUILayout.BeginVertical();
            EditorGUILayout.HelpBox("查找对象上子物体以及孙物体所有丢失组件的对象", MessageType.Info);
            GUILayout.Space(16);
            findMissingRoot = (Transform)EditorGUILayout.ObjectField("FindMissingRoot", findMissingRoot, typeof(Transform), true);
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find"))
            {
                missCompTransCount = 0;
                missingCompTrans.Clear();
                FindMissingCompsIncludeRoot(findMissingRoot);
            }
            if (GUILayout.Button("Clear"))
            {
                missingCompTrans.Clear();
                missCompTransCount = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
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
