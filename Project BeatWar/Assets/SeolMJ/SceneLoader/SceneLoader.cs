using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeolMJ
{
    public static class SceneLoader
    {
        public const string PREFAB_KEY = "SceneLoader";

        public static bool IsLoading { get; private set; }

        public readonly static List<int> Active = new();
        public readonly static List<int> Queue = new();
        public readonly static List<Action> Reserved = new();

        public static GameObject Prefab { get; private set; }

        public static bool IsReserved => Reserved.Count > 0;

        public static int OperationIndex { get; private set; } = -1;
        public static int OperationCount { get; private set; } = 0;
        static AsyncOperation currentOperation;
        public static float OperationProgress => currentOperation != null ? currentOperation.progress : 0f;

        public static float Progress
        {
            get
            {
                if (!IsLoading) return 1f;
                if (OperationIndex < 0) return 0f;
                float progress = OperationIndex / (float)OperationCount;
                progress += Mathf.InverseLerp(OperationProgress, 0f, 0.9f) / OperationCount;
                return progress;
            }
        }

        public static Action OnLoadStart;
        public static Action OnLoadEnd;

#if UNITY_EDITOR
        static bool inited = false;

        static SceneLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                inited = false;
                IsBooted = false;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnInit()
        {
            if (inited) return;

            int current = SceneManager.GetActiveScene().buildIndex;
            if (current != 0)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }

            Active.Clear();
            Active.Add(0);

            Queue.Clear();

            Reserved.Clear();

            Prefab = null;

            OnLoadStart = null;
            OnLoadEnd = null;

            inited = true;

            CallInits();

            if (current != 0)
            {
                Load(current);
            }
        }
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnInit()
        {
            Active.Add(0);
        }
#endif

        static void CallInits()
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && t.IsAbstract && t.IsSealed)
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttribute<OnInitializeAttribute>() != null);
            foreach (var method in methods)
            {
                method.Invoke(null, null);
            }
        }

        public struct Params
        {
            public bool DontSpawnPrefab;
        }

        public static void Load(params int[] scenes)
        {
            Load(scenes.AsEnumerable());
        }

        public static void Load(IEnumerable<int> scenes, Params param = default)
        {
            if (scenes == null || !scenes.Any())
            {
                return;
            }
            if (IsLoading)
            {
                Reserve(() => Load(scenes, param));
            }
            else LoadNow(scenes, param);
        }

        public static void LoadNow(IEnumerable<int> scenes, Params param = default)
        {
            foreach (var scene in scenes)
            {
                Queue.Add(scene);
            }
#pragma warning disable CS4014
            InternalLoad(param);
#pragma warning restore CS4014
        }

        public static void Reserve(Action action)
        {
            Reserved.Add(action);
        }

        public static int GetScene()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        static bool autoActivate = true;
        public static bool AutoActivate
        {
            get => autoActivate;
            set
            {
                autoActivate = value;
                if (currentOperation != null && !currentOperation.isDone)
                {
                    currentOperation.allowSceneActivation = value;
                }
            }
        }

        public static bool IsWaiting => currentOperation != null && currentOperation.progress >= 0.9f;

        public static void Activate()
        {
            if (currentOperation != null && !currentOperation.isDone)
            {
                currentOperation.allowSceneActivation = true;
            }
        }

        public static bool IsBooted { get; private set; } = false;

        static async Awaitable InternalLoad(Params param)
        {
            using var _ = SLogger.Scope($"Loading {Queue.Count} Scenes ({Active.Count} Active)", "All Scene Loaded");
            try
            {
                try
                {
                    OnLoadStart?.Invoke();
                }
                catch (Exception e)
                {
                    SLogger.LogError(e);
                }

                IsLoading = true;

                for (int i = 0; i < Active.Count; i++)
                {
                    if (!Queue.Contains(Active[i])) continue;

                    Queue.Remove(Active[i]);
                    Active.RemoveAt(i);
                    i--;
                }

                Queue.Distinct();

                int[] oldActiveScenes = Active.ToArray();

                OperationCount = Queue.Count + oldActiveScenes.Length;
                OperationIndex = 0;

                SceneTransition transition = null;

                if (!param.DontSpawnPrefab)
                {
                    if (Prefab == null)
                    {
                        var locations = await Addressables.LoadResourceLocationsAsync(PREFAB_KEY).Task;
                        if (locations != null && locations.Count != 0)
                        {
                            Prefab = await Addressables.LoadAssetAsync<GameObject>(PREFAB_KEY).Task;
                        }
                        else
                        {
                            SLogger.Log($"Scene Loader Prefab is null. Please register a Scene Loader with a key of \"{PREFAB_KEY}\".");
                        }
#if UNITY_EDITOR
                        if (!inited)
                        {
                            throw new OperationCanceledException();
                        }
#endif
                    }
                    
                    if (Prefab != null)
                    {
                        GameObject obj = (await UnityEngine.Object.InstantiateAsync(Prefab))[0];
                        UnityEngine.Object.DontDestroyOnLoad(obj);
                        transition = obj.GetComponent<SceneTransition>();
#if UNITY_EDITOR
                        if (!inited)
                        {
                            throw new OperationCanceledException();
                        }
#endif
                    }
                }

                if (transition)
                {
                    await Awaitable.WaitForSecondsAsync(transition.Delay);
                }

                if (!IsBooted)
                {
                    IsBooted = true;
                    try
                    {
                        CallInits();
                    }
                    catch (Exception e)
                    {
                        SLogger.LogError(e);
                    }
                }

                for (int i = 0; i < oldActiveScenes.Length - 1; i++)
                {
                    var scene = oldActiveScenes[i];
                    SLogger.Log($"Unloading Scene {scene}");

                    Active.Remove(scene);

                    currentOperation = SceneManager.UnloadSceneAsync(scene);
                    try
                    {
                        await currentOperation;
                    }
                    catch (Exception e)
                    {
                        SLogger.LogError(e);
                    }
#if UNITY_EDITOR
                    if (!inited)
                    {
                        throw new OperationCanceledException();
                    }
#endif

                    OperationIndex++;
                }

                SLogger.Log($"Loading Scene {Queue[0]}");

                currentOperation = SceneManager.LoadSceneAsync(Queue[0], LoadSceneMode.Single);
                currentOperation.allowSceneActivation = AutoActivate;
                try
                {
                    await currentOperation;
                }
                catch (Exception e)
                {
                    SLogger.LogError(e);
                }
#if UNITY_EDITOR
                if (!inited)
                {
                    throw new OperationCanceledException();
                }
#endif

                OperationIndex++;

                if (oldActiveScenes.Length > 0) Active.Remove(oldActiveScenes[^1]);
                Active.Add(Queue[0]);

                for (int i = 1; i < Queue.Count; i++)
                {
                    var scene = Queue[i];

                    SLogger.Log($"Loading Scene {scene}");

                    currentOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                    currentOperation.allowSceneActivation = AutoActivate;
                    try
                    {
                        await currentOperation;
                    }
                    catch (Exception e)
                    {
                        SLogger.LogError(e);
                    }
#if UNITY_EDITOR
                    if (!inited)
                    {
                        throw new OperationCanceledException();
                    }
#endif

                    OperationIndex++;

                    Active.Add(scene);
                }

                Queue.Clear();

                currentOperation = null;
                OperationCount = 0;
                OperationIndex = -1;

                IsLoading = false;
                AutoActivate = true;

                if (Time.timeScale == 0) Time.timeScale = 1f;

                try
                {
                    OnLoadEnd?.Invoke();
                }
                catch (Exception e)
                {
                    SLogger.LogError(e);
                }

                while (Reserved.Count > 0)
                {
                    try
                    {
                        Reserved[0].Invoke();
                    }
                    catch (Exception e)
                    {
                        Reserved.RemoveAt(0);
                        SLogger.LogError(e);
                        continue;
                    }
                    Reserved.RemoveAt(0);
                    SLogger.Log("Chain Loading");
                    break;
                }
            }
#pragma warning disable CS0168
            catch (OperationCanceledException __)
#pragma warning restore CS0168
            {
                currentOperation = null;
                OperationCount = 0;
                OperationIndex = -1;

                IsLoading = false;
                AutoActivate = true;

                SLogger.LogError("Scene Loaded (Halted)");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnInitializeAttribute : Attribute { }
}