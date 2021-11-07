using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackBuffEffect", menuName = "ScriptableObjects/CardEffect/AttackBuffEffect")]
public class AttackBuffEffect : CardEffect
{
    public int amount;

    public override void ApplyEffect(Character character)
    {
        character.BoostDamage(amount);
    }
}
