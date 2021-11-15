using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public override IEnumerator Evaluate()
    {
        StartTurn();
        yield return new WaitForSeconds(1);
        Action();
        yield return new WaitForSeconds(1);
        EndTurn();
    }

    public void Action()
    {
        BattleManager.Instance.Player.TakeDamage(Damage);
    }

    protected override void Die(Character character)
    {
        Destroy(gameObject);
    }
}
