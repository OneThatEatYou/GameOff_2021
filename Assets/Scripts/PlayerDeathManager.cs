using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class PlayerDeathManager : MonoBehaviour
{
    [ReadOnly] public bool playerIsDead;

    [Header("UI")]
    public TextMeshProUGUI deathText;
    public RectTransform deathRect;

    private void Start()
    {
        // doing this in OnEnable causes ProgressManager.Instance to be called before initializing
        SceneTransitionManager.Instance.onLevelLoaded += ResetObject;
    }

    private void OnDisable()
    {
        SceneTransitionManager.Instance.onLevelLoaded -= ResetObject;
    }

    public void ShowDeathPanel()
    {
        deathText.gameObject.SetActive(true);
        deathRect.gameObject.SetActive(true);
    }

    public void RetryLevel()
    {
        SceneTransitionManager.Instance.ChangeScene(SceneTransitionManager.MainSceneName);
    }

    public void BackToMainMenu()
    {
        SceneTransitionManager.Instance.ChangeScene(SceneTransitionManager.MainMenuSceneName);
    }

    private void ResetObject()
    {
        playerIsDead = false;
        deathText.gameObject.SetActive(false);
        deathRect.gameObject.SetActive(false);
    }
}
