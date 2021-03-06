using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackBuffEffect", menuName = "ScriptableObjects/CardEffect/AttackBuffEffect")]
public class AttackBuffEffect : CardEffect
{
    [Space]
    public int amount;

    public override float ApplyEffect(Character character, out CardDelegate callback)
    {
        callback = null;
        callback += () => character.BoostDamage(amount);
        return PlayAnimation(character.transform.position);
    }
}
