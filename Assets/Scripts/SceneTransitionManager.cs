using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string mainSceneName = "Main";
    public static string MainMenuSceneName { get { return Instance.mainMenuSceneName; } }
    public static string MainSceneName { get { return Instance.mainSceneName; } }

    public delegate void LevelLoadedDelegate();
    public LevelLoadedDelegate onLevelLoaded;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        onLevelLoaded?.Invoke();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
