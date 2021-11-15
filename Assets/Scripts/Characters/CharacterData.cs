using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string enemyName;

    [SerializeField] private int baseHealth;
    public int BaseHealth { get { return baseHealth; } }

    [SerializeField] private int baseDamage;
    public int BaseDamage { get { return baseDamage; } }

    [SerializeField] private int overflowLimit;
    public int OverflowLimit { get { return overflowLimit; } }

    [SerializeField] private Sprite characterSprite;
    public Sprite CharacterSprite { get { return characterSprite; } }

    [SerializeField] private RuntimeAnimatorController characterAnimator;
    public RuntimeAnimatorController CharacterAnimator { get { return characterAnimator; } }

    [SerializeField] private Action[] actions;

    public Action GetRandomAction()
    {
        return actions[Random.Range(0, actions.Length)];
    }
}

public enum TargetEnum
{
    Player,
    Enemy
}

[System.Serializable]
public class Action
{
    public bool justAttack;
    [ConditionalField(nameof(justAttack), true)] public CardEffect effect;
    [ConditionalField(nameof(justAttack), true)] public TargetEnum target;
    [ConditionalField(nameof(target), false, TargetEnum.Enemy)] public bool canTargetSelf;
}
