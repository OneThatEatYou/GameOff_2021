using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "ScriptableObjects/CardEffect/DamageEffect")]
public class DamageEffect : CardEffect
{
    [Space]

    public bool useUserAttack;
    [ConditionalField(nameof(useUserAttack), true)] public int damage;

    public override float ApplyEffect(Character character, out CardDelegate callback)
    {
        int damageDealt = useUserAttack ? character.Damage : damage;
        callback = null;
        callback += () => character.TakeDamage(damageDealt);

        return PlayAnimation(character.transform.position);
    }
}
