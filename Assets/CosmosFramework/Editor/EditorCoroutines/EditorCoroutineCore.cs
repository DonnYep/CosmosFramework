using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine.Networking;

 namespace Cosmos.CosmosEditor
{
    public  class EditorCoroutineCore
    {
        static EditorCoroutineCore instance;
        public static EditorCoroutineCore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EditorCoroutineCore();
                    instance.Initialize();
                }
                return instance;
            }
        }

        DateTime previousTimeSinceStartup;
        List<List<EditorCoroutine>> coroutineCache = new List<List<EditorCoroutine>>();

        Dictionary<string, Dictionary<string, EditorCoroutine>> coroutineOwnerDict
            = new Dictionary<string, Dictionary<string, EditorCoroutine>>();

        Dictionary<string, List<EditorCoroutine>> coroutineDict
            = new Dictionary<string, List<EditorCoroutine>>();

        #region StaticMethods

        #region StartCoroutine
        public static EditorCoroutine StartCoroutine(IEnumerator routine, object thisReference)
        {
            return Instance.GoStartCoroutine(routine, thisReference);
        }
        public static EditorCoroutine StartCoroutine(string methodName, object thisReference)
        {
            return StartCoroutine(methodName, null, thisReference);
        }
        public static EditorCoroutine StartCoroutine(string methodName, object value, object thisReference)
        {
            MethodInfo methodInfo = thisReference.GetType()
                .GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                Debug.LogError("Coroutine '" + methodName + "' couldn't be started, the method doesn't exist!");
            }
            object returnValue;

            if (value == null)
            {
                returnValue = methodInfo.Invoke(thisReference, null);
            }
            else
            {
                returnValue = methodInfo.Invoke(thisReference, new object[] { value });
            }

            if (returnValue is IEnumerator)
            {
                return Instance.GoStartCoroutine((IEnumerator)returnValue, thisReference);
            }
            else
            {
                Debug.LogError("Coroutine '" + methodName + "' couldn't be started, the method doesn't return an IEnumerator!");
            }
            return null;
        }
        #endregion

        #region StopCoroutine
        public static void StopCoroutine(IEnumerator routine, object thisReference)
        {
            Instance.GoStopCoroutine(routine, thisReference);
        }
        public static void StopCoroutine(string methodName, object thisReference)
        {
            Instance.GoStopCoroutine(methodName, thisReference);
        }
        public static void StopAllCoroutines(object thisReference)
        {
            Instance.GoStopAllCoroutines(thisReference);
        }
        #endregion
        static bool MoveNext(EditorCoroutine coroutine)
        {
            if (coroutine.Routine.MoveNext())
            {
                return Process(coroutine);
            }
            return false;
        }
        static bool Process(EditorCoroutine coroutine)
        {
            object current = coroutine.Routine.Current;
            if (current == null)
            {
                coroutine.CurrentYield = new YieldDefault();
            }
            else if (current is WaitForSeconds)
            {
                float seconds = float.Parse(GetInstanceField(typeof(WaitForSeconds), current, "m_Seconds").ToString());
                coroutine.CurrentYield = new YieldWaitForSeconds() { timeLeft = seconds };
            }
            else if (current is CustomYieldInstruction)
            {
                coroutine.CurrentYield = new YieldCustomYieldInstruction()
                {
                    customYield = current as CustomYieldInstruction
                };
            }
            else if (current is UnityWebRequest)
            {
                coroutine.CurrentYield = new YieldUnityWebRequest { unityWebRequest = (UnityWebRequest)current };
            }
            else if (current is WaitForFixedUpdate || current is WaitForEndOfFrame)
            {
                coroutine.CurrentYield = new YieldDefault();
            }
            else if (current is AsyncOperation)
            {
                coroutine.CurrentYield = new YieldAsync { asyncOperation = (AsyncOperation)current };
            }
            else if (current is EditorCoroutine)
            {
                coroutine.CurrentYield = new YieldNestedCoroutine { coroutine = (EditorCoroutine)current };
            }
            else
            {
                Debug.LogException(
                    new Exception("<" + coroutine.MethodName + "> yielded an unknown or unsupported type! (" + current.GetType() + ")"),
                    null);
                coroutine.CurrentYield = new YieldDefault();
            }
            return true;
        }
        static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
        #endregion

        #region Yields
        public interface ICoroutineYield
        {
            bool IsDone(float deltaTime);
        }
        struct YieldDefault : ICoroutineYield
        {
            public bool IsDone(float deltaTime)
            {
                return true;
            }
        }
        struct YieldWaitForSeconds : ICoroutineYield
        {
            public float timeLeft;

            public bool IsDone(float deltaTime)
            {
                timeLeft -= deltaTime;
                return timeLeft < 0;
            }
        }

        struct YieldCustomYieldInstruction : ICoroutineYield
        {
            public CustomYieldInstruction customYield;

            public bool IsDone(float deltaTime)
            {
                return !customYield.keepWaiting;
            }
        }
        struct YieldUnityWebRequest : ICoroutineYield
        {
            public UnityWebRequest unityWebRequest;

            public bool IsDone(float deltaTime)
            {
                return unityWebRequest.isDone;
            }
        }
        struct YieldAsync : ICoroutineYield
        {
            public AsyncOperation asyncOperation;

            public bool IsDone(float deltaTime)
            {
                return asyncOperation.isDone;
            }
        }
        struct YieldNestedCoroutine : ICoroutineYield
        {
            public EditorCoroutine coroutine;

            public bool IsDone(float deltaTime)
            {
                return coroutine.Finished;
            }
        }
        #endregion

        #region Private Methods
        void Initialize()
        {
            previousTimeSinceStartup = DateTime.Now;
            EditorApplication.update += OnRefresh;
        }
        void OnRefresh()
        {
            float deltaTime = (float)(DateTime.Now.Subtract(previousTimeSinceStartup).TotalMilliseconds / 1000.0f);
            previousTimeSinceStartup = DateTime.Now;
            if (coroutineDict.Count == 0)
            {
                return;
            }
            coroutineCache.Clear();
            foreach (var pair in coroutineDict)
                coroutineCache.Add(pair.Value);

            for (var i = coroutineCache.Count-1; i >= 0; i--)
            {
                var coroutines = coroutineCache[i];
                for (int j = coroutines.Count - 1; j >= 0; j--)
                {
                    EditorCoroutine coroutine = coroutines[j];
                    if (!coroutine.CurrentYield.IsDone(deltaTime))
                    {
                        continue;
                    }
                    if (!MoveNext(coroutine))
                    {
                        coroutines.RemoveAt(j);
                        coroutine.CurrentYield = null;
                        coroutine.Finished = true;
                    }
                    if (coroutines.Count == 0)
                    {
                        coroutineDict.Remove(coroutine.RoutineUniqueHash);
                    }
                }
            }
        }
        EditorCoroutine GoStartCoroutine(IEnumerator routine, object thisReference)
        {
            if (routine == null)
            {
                Debug.LogException(new Exception("IEnumerator is null!"), null);
            }
            EditorCoroutine coroutine = CreateCoroutine(routine, thisReference);
            GoStartCoroutine(coroutine);
            return coroutine;
        }
        void GoStartCoroutine(EditorCoroutine coroutine)
        {
            if (!coroutineDict.ContainsKey(coroutine.RoutineUniqueHash))
            {
                List<EditorCoroutine> newCoroutineList = new List<EditorCoroutine>();
                coroutineDict.Add(coroutine.RoutineUniqueHash, newCoroutineList);
            }
            coroutineDict[coroutine.RoutineUniqueHash].Add(coroutine);

            if (!coroutineOwnerDict.ContainsKey(coroutine.OwnerUniqueHash))
            {
                Dictionary<string, EditorCoroutine> newCoroutineDict = new Dictionary<string, EditorCoroutine>();
                coroutineOwnerDict.Add(coroutine.OwnerUniqueHash, newCoroutineDict);
            }
            if (!coroutineOwnerDict[coroutine.OwnerUniqueHash].ContainsKey(coroutine.RoutineUniqueHash))
            {
                coroutineOwnerDict[coroutine.OwnerUniqueHash].Add(coroutine.RoutineUniqueHash, coroutine);
            }

            MoveNext(coroutine);
        }
        EditorCoroutine CreateCoroutine(IEnumerator routine, object thisReference)
        {
            return new EditorCoroutine(routine, thisReference.GetHashCode(), thisReference.GetType().ToString());
        }
        void GoStopCoroutine(IEnumerator routine, object thisReference)
        {
            GoStopActualRoutine(CreateCoroutine(routine, thisReference));
        }
        void GoStopCoroutine(string methodName, object thisReference)
        {
            GoStopActualRoutine(CreateCoroutineFromString(methodName, thisReference));
        }
        void GoStopActualRoutine(EditorCoroutine routine)
        {
            if (coroutineDict.ContainsKey(routine.RoutineUniqueHash))
            {
                coroutineOwnerDict[routine.OwnerUniqueHash].Remove(routine.RoutineUniqueHash);
                coroutineDict.Remove(routine.RoutineUniqueHash);
            }
        }
        void GoStopAllCoroutines(object thisReference)
        {
            EditorCoroutine coroutine = CreateCoroutine(null, thisReference);
            if (coroutineOwnerDict.ContainsKey(coroutine.OwnerUniqueHash))
            {
                foreach (var couple in coroutineOwnerDict[coroutine.OwnerUniqueHash])
                {
                    coroutineDict.Remove(couple.Value.RoutineUniqueHash);
                }
                coroutineOwnerDict.Remove(coroutine.OwnerUniqueHash);
            }
        }
        EditorCoroutine CreateCoroutineFromString(string methodName, object thisReference)
        {
            return new EditorCoroutine(methodName, thisReference.GetHashCode(), thisReference.GetType().ToString());
        }
        # endregion

    }
}