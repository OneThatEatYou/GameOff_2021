using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class MainMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class Line
    {
        public string line;
        public float lineWaitTime;
        public int repeats;
    }

    [Header("UI")]
    public TextMeshProUGUI text;
    public RectTransform settingsMenu;
    public Slider masterSlider;
    public Slider BGMSlider;
    public Slider SFXSlider;

    [Header("Contents")]
    public float startDelay;
    public int maxLineNum = 15;
    public List<Line> introLines;
    public List<Line> startGameLines;

    [SerializeField, ReadOnly] private bool awaitingInput;

    private MainInput input;
    private bool isSetting;
    private bool isStarting;

    private delegate void MenuDelegate();

    private void Awake()
    {
        input = new MainInput();
    }

    private void Start()
    {
        SetMasterSlider();
        SetBGMSlider();
        SetSFXSlider();

        StartShowLine(introLines, startDelay, 0, introLines.Count - 1);
        //PlayGame();
        DestroyMainSceneSingletons();
    }

    private void OnEnable()
    {
        input.Enable();
        input.MainMenu.Play.performed += ctx => PlayGame();
        input.MainMenu.Settings.performed += ctx => ToggleSettingsMenu();
    }

    private void OnDisable()
    {
        input.MainMenu.Play.performed -= ctx => PlayGame();
        input.MainMenu.Settings.performed -= ctx => ToggleSettingsMenu();
        input.Disable();
    }

    private void StartShowLine(List<Line> lines, float startDelay, int startLineNum, int endLineNum, MenuDelegate onShowedLine = null)
    {
        StartCoroutine(ShowLinesCoroutine(lines, startDelay, startLineNum, endLineNum, onShowedLine));
    }

    private IEnumerator ShowLinesCoroutine(List<Line> lines, float startDelay, int startLineNum, int endLineNum, MenuDelegate onShowedLine)
    {
        awaitingInput = false;

        if (startLineNum >= lines.Count || startLineNum < 0 || endLineNum >= lines.Count || endLineNum < startLineNum)
        {
            Debug.LogError("Invalid start or end line number");
        }
        else
        {
            yield return new WaitForSeconds(startDelay);

            for (int i = startLineNum; i <= endLineNum; i++)
            {
                for (int j = 0; j <= lines[i].repeats; j++)
                {
                    ShowLine(lines[i]);
                    if (lines[i].lineWaitTime > 0)
                    {
                        yield return new WaitForSeconds(lines[i].lineWaitTime);
                    }
                }
            }

            onShowedLine?.Invoke();
        }

        awaitingInput = true;
    }

    private void ShowLine(Line line)
    {
        // removes oldest line is needed
        // inefficient, but good enough
        List<string> printedLines = new List<string>(text.text.Split('\n'));

        if (printedLines.Count > 15)
        {
            printedLines.RemoveAt(0);
            text.text = string.Join("\n", printedLines);
        }

        text.text += "> ";
        text.text += line.line;
        text.text += "\n";
    }

    public void ToggleSettingsMenu()
    {
        if (!awaitingInput || isStarting) return;

        if (isSetting)
        {
            // hide menu
            settingsMenu.gameObject.SetActive(false);
        }
        else
        {
            // show menu
            settingsMenu.gameObject.SetActive(true);
        }

        isSetting = !isSetting;
    }

    //convert vol percent to value in mixer group
    private float VolumeToAtten(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
    }

    //convert value in mixer group to vol percent
    private float AttenToVol(float atten)
    {
        return Mathf.Pow(10, atten / 20);
    }

    public void SetMasterVolume(float newValue)
    {
        AudioManager.Instance.mainMixer.SetFloat("Vol_Master", VolumeToAtten(newValue));
    }

    private void SetMasterSlider()
    {
        if (AudioManager.Instance.mainMixer.GetFloat("Vol_Master", out float v))
        {
            masterSlider.value = AttenToVol(v);
        }
    }

    public void SetBGM(float newValue)
    {
        AudioManager.Instance.mainMixer.SetFloat("Vol_BGM", VolumeToAtten(newValue));
    }

    private void SetBGMSlider()
    {
        if (AudioManager.Instance.mainMixer.GetFloat("Vol_BGM", out float v))
        {
            BGMSlider.value = AttenToVol(v);
        }
    }

    public void SetSFX(float newValue)
    {
        AudioManager.Instance.mainMixer.SetFloat("Vol_SFX", VolumeToAtten(newValue));
    }

    private void SetSFXSlider()
    {
        if (AudioManager.Instance.mainMixer.GetFloat("Vol_SFX", out float v))
        {
            SFXSlider.value = AttenToVol(v);
        }
    }

    private void PlayGame()
    {
        input.Disable();

        StartShowLine(startGameLines, 0, 0, startGameLines.Count - 1, () => LoadGame());
    }

    private void LoadGame()
    {
        SceneTransitionManager.Instance.ChangeScene(SceneTransitionManager.MainSceneName);
    }

    private void DestroyMainSceneSingletons()
    {
        if (BattleManager.Instance)
        {
            Debug.Log("Destroying battle manager");
            Destroy(BattleManager.Instance.gameObject);
        }
        if (ProgressManager.Instance) Destroy(ProgressManager.Instance.gameObject);
        if (PlayerSelectionHandler.Instance) Destroy(PlayerSelectionHandler.Instance.gameObject);
    }
}
