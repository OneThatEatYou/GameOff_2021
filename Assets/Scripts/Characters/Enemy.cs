using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : Character
{
    [Header("Attack Animation")]
    public float tackleMovement;
    public float tackleTime;
    public float recoverTime;
    public float waitTime;

    public override IEnumerator Evaluate()
    {
        StartTurn();
        yield return new WaitForSeconds(1);

        yield return new WaitForSeconds(Action());
        EndTurn();
    }

    // returns the wait time of the action
    public float Action()
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
            return Attack(target);
        }
        else
        {
            if (target)
            {
                return PlayAttackSequence(() => action.effect.ApplyEffect(target));
            }
            else
            {
                DoNothing();
                return 0;
            }
        }
    }

    // returns the length of attack sequence
    private float Attack(Character target)
    {
        return PlayAttackSequence(() => target.TakeDamage(Damage));
    }

    private float PlayAttackSequence(TweenCallback callback = null)
    {
        Sequence attackSequence = DOTween.Sequence();
        attackSequence.Append(transform.DOMoveX(-tackleMovement, tackleTime).SetRelative());
        attackSequence.Append(transform.DOMoveX(tackleMovement, recoverTime).SetRelative());
        attackSequence.AppendInterval(waitTime);
        if (callback != null) attackSequence.AppendCallback(callback);

        return attackSequence.Duration();
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
