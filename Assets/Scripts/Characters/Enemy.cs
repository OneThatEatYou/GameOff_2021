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
        Character target = null;
        //Debug.Log($"{characterData.enemyName} is using {action.effect.name}");

        // assign target
        switch (action.target)
        {
            case TargetEnum.Player:
                target = BattleManager.Instance.Player;
                break;
            case TargetEnum.Enemy:
                target = GetRandomEnemy(action.canTargetSelf);
                break;
        }

        if (action.justAttack)
        {
            Attack(target);
        }
        else
        {
            if (target)
            {
                action.effect.ApplyEffect(target);
            }
            else
            {
                DoNothing();
            }
        }
    }

    private void Attack(Character target)
    {
        target.TakeDamage(Damage);
    }

    protected override void Die(Character character)
    {
        Destroy(gameObject);
    }

    private Enemy GetRandomEnemy(bool canTargetSelf)
    {
        List<BattleManager.EnemyPosition> enemies = new List<BattleManager.EnemyPosition>(BattleManager.Instance.enemies);

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
            return null;
        }

        return enemies[Random.Range(0, enemies.Count)].enemy;
    }

    private void DoNothing()
    {
        Debug.LogWarning("No targetable enemy found");
    }
}
