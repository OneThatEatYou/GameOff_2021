using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class EncounterManager : Singleton<EncounterManager>
{
    [SerializeField, ReadOnly] private bool isWandering;
    [Expandable] public CharacterData[] encounterData;
    public Vector2 encounterNumberRange;

    [Header("Wander")]
    public Vector2 wanderTimeRange;
    public float wanderSpeed = 1;
    public float wanderAccelDur = 0.5f;
    public float wanderDeccel = 2;
    public Material groundMaterial;

    private float wanderTime;

    public const string groundOffsetRef = "_Offset";

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        //Encounter(new CharacterData[] { encounterData[0], encounterData[0], encounterData[0] });
        Wander();
    }

    private void OnEnable()
    {
        BattleManager.Instance.onBattleEndCallback += Wander;
    }

    private void OnDisable()
    {
        // if-statement stops error when exiting playmode
        if (BattleManager.Instance) BattleManager.Instance.onBattleEndCallback -= Wander;
    }

    private void Wander()
    {
        if (isWandering) return;

        isWandering = true;
        wanderTime = Random.Range(wanderTimeRange.x, wanderTimeRange.y);
        StartCoroutine(PlayerWanderCoroutine());
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

        if (fixedEncounter != null) encounters = fixedEncounter;
        else encounters = GetRandomEncounters(Random.Range(1, 3));

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

        while (velocity > 0.1f)
        {
            velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * wanderDeccel);
            elapsed += Time.deltaTime;
            Vector2 curOffset = groundMaterial.GetVector(groundOffsetRef);
            curOffset.x = Mathf.Repeat(curOffset.x + Time.deltaTime * wanderSpeed * velocity, 1);
            groundMaterial.SetVector(groundOffsetRef, new Vector4(curOffset.x, 0, 0, 0));

            yield return null;
        }

        isWandering = false;
        Encounter();
    }
}
