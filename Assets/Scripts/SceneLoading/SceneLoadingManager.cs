using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    [Header("Loading Screen")]
    [SerializeField] private Canvas _loadingCanvas;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private Camera loadingCamera;
    private float targetProgress;
    bool isLoading;

    [Header("Scene Groups")]
    public SceneGroup[] sceneGroups;
    public Action OnLoadingStarted;
    public Action<string> OnSceneLoaded;
    public Action OnSceneGroupLoaded;
    public Action<string> OnSceneUnloadStarted;
    public Action OnUnloadingStarted;
    public Action OnUnloadingCompleted;
    private AsyncOperationGroup asyncOperationGroup;
    private SceneGroup activeSceneGroup;
    private int _indexToLoad;
    private List<string> loadedScenes;

    #region Unity Methods

    protected override void Awake()
    {
        base.Awake();
        LoadSceneGroup(0);
    }

    void Update()
    {
        if (!isLoading) return;

        FillLoadingBar();
    }

    void OnEnable()
    {
        //OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
        OnUnloadingCompleted += () => StartCoroutine(LoadSceneGroupCoroutine());
    }

    void OnDisable()
    {
        //OnSceneLoaded -= sceneName => Debug.Log("Loaded: " + sceneName);
        OnUnloadingCompleted -= () => StartCoroutine(LoadSceneGroupCoroutine());
    }

    #endregion

    #region Loading Screen

    private void EnableLoadingCanvas(bool enable = true)
    {
        isLoading = enable;
        _loadingCanvas.gameObject.SetActive(enable);
        loadingCamera.gameObject.SetActive(enable);
    }

    private void FillLoadingBar()
    {
        float currentFillAmount = _loadingBar.fillAmount;
        float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

        float dynamicFillSpeed = progressDifference * fillSpeed;

        _loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
    }

    #endregion

    #region  Scene Loading

    public void LoadSceneGroup(int index)
    {
        _indexToLoad = index;
        activeSceneGroup = sceneGroups[index];
        loadedScenes = new List<string>();

        OnUnloadingStarted?.Invoke();
        StartCoroutine(UnloadScenes());
    }

    private IEnumerator LoadSceneGroupCoroutine()
    {
        OnLoadingStarted?.Invoke();

        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            loadedScenes.Add(SceneManager.GetSceneAt(i).name);
        }

        var totalScenesToLoad = activeSceneGroup.Scenes.Count;
        asyncOperationGroup = new AsyncOperationGroup(totalScenesToLoad);
        for (int i = 0; i < totalScenesToLoad; i++)
        {
            var sceneData = activeSceneGroup.Scenes[i];
            if (loadedScenes.Contains(sceneData.Name)) continue;
            asyncOperationGroup.Operations.Add(SceneManager.LoadSceneAsync(sceneGroups[_indexToLoad].Scenes[i].SceneField, LoadSceneMode.Additive));
            OnSceneLoaded?.Invoke(sceneData.Name);
        }

        Scene activeScene = SceneManager.GetSceneByName(sceneGroups[_indexToLoad].FindSceneNameByType(SceneType.ActiveScene));

        EnableLoadingCanvas();
        _loadingBar.fillAmount = 0f;
        targetProgress = 1f;
        LoadingProgress progress = new LoadingProgress();
        progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
        yield return new WaitUntil(() => asyncOperationGroup.IsDone);
        if (activeScene.IsValid())
        {
            yield return 0;
            SceneManager.SetActiveScene(activeScene);
        }
        OnSceneGroupLoaded?.Invoke();
        yield return new WaitForSeconds(0.1f);
        EnableLoadingCanvas(false);
        yield return null;
    }

    public IEnumerator UnloadScenes()
    {
        var scenes = new List<string>();
        var activeScene = SceneManager.GetActiveScene();

        int sceneCount = SceneManager.sceneCount;

        for (var i = sceneCount - 1; i > 0; i--)
        {
            var sceneAt = SceneManager.GetSceneAt(i);
            if (!sceneAt.isLoaded) continue;

            var sceneName = sceneAt.name;
            if (sceneName.Equals(activeScene.name) || sceneName == "Bootstrapper") continue;
            scenes.Add(sceneName);
        }

        var operationGroup = new AsyncOperationGroup(scenes.Count);

        foreach (var scene in scenes)
        {
            OnSceneUnloadStarted?.Invoke(scene);
            var operation = SceneManager.UnloadSceneAsync(scene);
            if (operation == null) continue;

            operationGroup.Operations.Add(operation);
        }
        yield return new WaitUntil(() => operationGroup.IsDone);

        OnUnloadingCompleted?.Invoke();

        yield return null;
    }

    public Scene ReturnActiveEnvironmentalScene()
    {
        Scene s = SceneManager.GetActiveScene();

        foreach (SceneData sceneData in activeSceneGroup.Scenes)
        {
            if (sceneData.SceneType == SceneType.Environment)
            {
                s = SceneManager.GetSceneByName(sceneData.Name);
            }
        }

        return s;
    }

    #endregion

    #region Debug
    [ContextMenu("Load Main Menu")]
    private void LoadMainMenu()
    {
        LoadSceneGroup(0);
    }

    [ContextMenu("Load Dungeon Scene")]
    private void LoadDungeonScene()
    {
        LoadSceneGroup(1);
    }

    [ContextMenu("Load Dungeon 2 Scene")]
    private void LoadDungeon2Scene()
    {
        LoadSceneGroup(2);
    }
    #endregion
}

//Class for storing a group of AsyncOperations and getting progress updates on all of them
//Could make this a util class but I don't know if this will realistically be used anywhere else
public class AsyncOperationGroup
{
    public readonly List<AsyncOperation> Operations;

    public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
    public bool IsDone => Operations.All(o => o.isDone);

    public AsyncOperationGroup(int initialCapacity)
    {
        Operations = new List<AsyncOperation>(initialCapacity);
    }
}
