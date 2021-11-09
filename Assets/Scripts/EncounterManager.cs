using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class EncounterManager : Singleton<EncounterManager>
{
    [SerializeField, ReadOnly] private bool isWandering;

    private void Awake()
    {
        InitializeSingleton();
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

    private void Encounter(Enemy[] enemies)
    {

    }

    private IEnumerator PlayerWanderCoroutine()
    {
        yield return new WaitForSeconds(5);

        isWandering = false;
    }
}
