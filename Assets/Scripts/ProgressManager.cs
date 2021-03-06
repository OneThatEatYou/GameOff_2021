using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class ProgressManager : Singleton<ProgressManager>
{
    [SerializeField, ReadOnly] private bool isWandering;
    [SerializeField, ReadOnly] private bool levelIsOver;

    [Header("Encounter")]
    [Expandable] public CharacterData[] encounterData;
    public Vector2 encounterNumberRange;

    [Header("Wander")]
    public Vector2 wanderTimeRange;
    public float wanderSpeed = 1;
    public float wanderAccelDur = 0.5f;
    public float wanderDeccel = 2;
    public Material groundMaterial;

    [Header("Level Progress")]
    public int curLevel;
    [SerializeField, ReadOnly] private float progress;
    public float startLevelLength = 30;
    public float levelLengthIncrement = 10;
    [SerializeField, ReadOnly] private float levelLength;

    [Header("Level Clear")]
    public DeckModifier deckModifier;

    [Header("Death")]
    public PlayerDeathManager playerDeathManager;

    private float wanderTime;
    private Coroutine wanderCR;
    private float levelTimeElapsed = 0;

    public const string groundOffsetRef = "_Offset";

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        levelLength = startLevelLength;
        Wander();
        //EndLevel();
        //Encounter(new CharacterData[] { encounterData[0] });
    }

    private void OnEnable()
    {
        BattleManager.Instance.onBattleEndCallback += Wander;
        deckModifier.onChoiceConfirmed += NextLevel;
        SceneTransitionManager.Instance.onLevelLoaded += UpdateLevelLength;
        SceneTransitionManager.Instance.onLevelLoaded += ResetProgress;
        SceneTransitionManager.Instance.onLevelLoaded += Wander;
    }

    private void OnDisable()
    {
        // if-statement stops error when exiting playmode
        if (BattleManager.Instance) BattleManager.Instance.onBattleEndCallback -= Wander;
        deckModifier.onChoiceConfirmed -= NextLevel;
        if (SceneTransitionManager.Instance)
        {
            SceneTransitionManager.Instance.onLevelLoaded -= UpdateLevelLength;
            SceneTransitionManager.Instance.onLevelLoaded -= ResetProgress;
            SceneTransitionManager.Instance.onLevelLoaded -= Wander;
        }   
    }

    private void Update()
    {
        if (isWandering)
        {
            ProgressLevel();
        }
    }

    private void Wander()
    {
        if (isWandering || playerDeathManager.playerIsDead)
        {
            Debug.Log("Unable to wander");
            return;
        }

        isWandering = true;
        wanderTime = Random.Range(wanderTimeRange.x, wanderTimeRange.y);
        wanderCR = StartCoroutine(PlayerWanderCoroutine());
    }

    private CharacterData[] GetRandomEncounters(int num)
    {
        CharacterData[] encounters = new CharacterData[num];

        for (int i = 0; i < num; i++)
        {
            encounters[i] = encounterData[Random.Range(0, encounterData.Length)];
        }

        return encounters;
    }

    private void Encounter(CharacterData[] fixedEncounter = null)
    {
        CharacterData[] encounters;
        int maxEncounterNum = 1;

        if (curLevel > 4)
        {
            maxEncounterNum = 3;
        }
        else if (curLevel > 2)
        {
            maxEncounterNum = 2;
        }

        if (fixedEncounter != null) encounters = fixedEncounter;
        else encounters = GetRandomEncounters(Random.Range(1, maxEncounterNum + 1));

        BattleManager.Instance.StartBattle(encounters);
    }

    private IEnumerator PlayerWanderCoroutine()
    {
        float elapsed = 0;
        float velocity = 0;

        while (elapsed < wanderTime)
        {
            velocity = Mathf.Min(elapsed / wanderAccelDur, 1);
            elapsed += Time.deltaTime;
            Vector2 curOffset = groundMaterial.GetVector(groundOffsetRef);
            curOffset.x = Mathf.Repeat(curOffset.x + Time.deltaTime * wanderSpeed * velocity, 1);
            groundMaterial.SetVector(groundOffsetRef, new Vector4(curOffset.x, 0, 0, 0));

            yield return null;
        }

        isWandering = false;

        // deccel
        while (velocity > 0.1f)
        {
            velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * wanderDeccel);
            elapsed += Time.deltaTime;
            Vector2 curOffset = groundMaterial.GetVector(groundOffsetRef);
            curOffset.x = Mathf.Repeat(curOffset.x + Time.deltaTime * wanderSpeed * velocity, 1);
            groundMaterial.SetVector(groundOffsetRef, new Vector4(curOffset.x, 0, 0, 0));

            yield return null;
        }

        Encounter();
    }

    private void ProgressLevel()
    {
        if (levelIsOver) return;

        if (levelTimeElapsed < levelLength)
        {
            levelTimeElapsed += Time.deltaTime;
            progress = levelTimeElapsed / levelLength;
        }
        else
        {
            progress = 1;
            EndLevel();
        }
    }

    private void EndLevel()
    {
        Debug.Log("Level ended");

        levelIsOver = true;
        if (wanderCR != null) StopCoroutine(wanderCR);
        isWandering = false;

        // choose card to add to deck
        deckModifier.EndLevel();
    }

    private void NextLevel()
    {
        curLevel++;
        SceneTransitionManager.Instance.ChangeScene(SceneTransitionManager.MainSceneName);
    }

    private void ResetProgress()
    {
        Debug.Log("Reseting progress");
        levelTimeElapsed = 0;
        progress = 0;
        levelIsOver = false;
        isWandering = false;
        playerDeathManager.playerIsDead = false;
    }

    private void UpdateLevelLength()
    {
        levelLength = startLevelLength + curLevel * levelLengthIncrement;
    }

    public void PlayerDied()
    {
        playerDeathManager.playerIsDead = true;
        // show death panel after battle ends
        BattleManager.Instance.onBattleEndCallback += playerDeathManager.ShowDeathPanel;
    }
}
