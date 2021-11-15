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
        Action action = characterData.GetRandomAction();
        //Debug.Log($"{characterData.enemyName} is using {action.effect.name}");

        if (action.justAttack)
        {
            Attack();
        }
        else
        {
            switch (action.target)
            {
                case TargetEnum.Player:
                    action.effect.ApplyEffect(BattleManager.Instance.Player);
                    break;
                case TargetEnum.Enemy:
                    if (GetRandomEnemy(action.canTargetSelf, out Enemy enemy))
                    {
                        action.effect.ApplyEffect(enemy);
                    }
                    else
                    {
                        DoNothing();
                    }
                    break;
            }
        }
    }

    private void Attack()
    {
        BattleManager.Instance.Player.TakeDamage(Damage);
    }

    protected override void Die(Character character)
    {
        Destroy(gameObject);
    }

    private bool GetRandomEnemy(bool canTargetSelf, out Enemy randomEnemy)
    {
        List<BattleManager.EnemyPosition> enemies = new List<BattleManager.EnemyPosition>(BattleManager.Instance.enemies);
        randomEnemy = null;

        // filter the enemy positions to get valid enemies
        foreach (var enemy in enemies.ToArray())
        {
            if (enemy.enemy == null || (!canTargetSelf && enemy.enemy == this))
            {
                enemies.Remove(enemy);
                //Debug.Log("Removed position " + enemy.position + " from list");
            }
        }

        if (enemies.Count == 0)
        {
            return false;
        }

        randomEnemy = enemies[Random.Range(0, enemies.Count)].enemy;
        return true;
    }

    private void DoNothing()
    {
        Debug.LogWarning("No targetable enemy found");
    }
}
