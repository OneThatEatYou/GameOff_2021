using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewAttackEffect", menuName = "ScriptableObjects/CardEffect/AttackEffect")]
public class AttackEffect : CardEffect
{
    public bool useUserAttack;
    [ConditionalField(nameof(useUserAttack), true)] public int damage;

    public override void ApplyEffect(Character character)
    {
        int damageDealt = useUserAttack ? character.CurDamage : damage;
        character.TakeDamage(damageDealt);
    }
}
