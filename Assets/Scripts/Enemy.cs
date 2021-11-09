using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    protected override void OnEnable()
    {
        base.OnEnable();
        BattleManager.Instance.RegisterEnemy(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BattleManager.Instance.UnregisterEnemy(this);
    }

    public override IEnumerator Evaluate()
    {
        StartTurn();
        yield return new WaitForSeconds(1);
        Attack();
        yield return new WaitForSeconds(1);
        EndTurn();
    }

    public void Attack()
    {
        BattleManager.Instance.Player.TakeDamage(Damage);
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }
}
