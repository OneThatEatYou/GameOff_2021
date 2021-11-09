using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "ScriptableObjects/CardEffect/DamageEffect")]
public class DamageEffect : CardEffect
{
    public bool useUserAttack;
    [ConditionalField(nameof(useUserAttack), true)] public int damage;

    public override void ApplyEffect(Character character)
    {
        int damageDealt = useUserAttack ? character.Damage : damage;
        character.TakeDamage(damageDealt);
    }
}
