using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class EncounterManager : Singleton<EncounterManager>
{
    [SerializeField, ReadOnly] private bool isWandering;
    [Expandable] public CharacterData[] encounterData;
    public Vector2 encounterNumberRange;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        Encounter(new CharacterData[] { encounterData[0], encounterData[0], encounterData[0] });
    }

    private void OnEnable()
    {
        BattleManager.Instance.onBattleEndCallback += Wander;
    }

    private void OnDisable()
    {
        BattleManager.Instance.onBattleEndCallback -= Wander;
    }

    private void Wander()
    {
        if (isWandering) return;

        isWandering = true;
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
        yield return new WaitForSeconds(5);

        isWandering = false;
        Encounter();
    }
}
