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
        ProgressManager.Instance.onLevelLoaded += ResetObject;
    }

    private void OnDisable()
    {
        ProgressManager.Instance.onLevelLoaded -= ResetObject;
    }

    public void ShowDeathPanel()
    {
        deathText.gameObject.SetActive(true);
        deathRect.gameObject.SetActive(true);
    }

    public void RetryLevel()
    {
        ProgressManager.Instance.ChangeScene(ProgressManager.Instance.mainSceneName);
    }

    public void BackToMainMenu()
    {
        ProgressManager.Instance.ChangeScene(ProgressManager.Instance.mainMenuSceneName);
    }

    private void ResetObject()
    {
        playerIsDead = false;
        deathText.gameObject.SetActive(false);
        deathRect.gameObject.SetActive(false);
    }
}
