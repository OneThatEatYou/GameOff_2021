using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public override IEnumerator Evaluate()
    {
        StartTurn();
        yield return null;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        BattleManager.Instance.EnableInput();
    }

    public override void EndTurn(float delay = 0)
    {
        base.EndTurn(delay);
        BattleManager.Instance.DisableInput();
    }

    protected override void Die(Character character)
    {
        Debug.Log("Player died.");
        ProgressManager.Instance.PlayerDied();
    }
}
