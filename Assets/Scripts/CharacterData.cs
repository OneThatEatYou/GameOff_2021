using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string enemyName;
    [SerializeField] private int baseHealth;
    public int BaseHealth { get { return baseHealth; } }
    [SerializeField] private int baseDamage;
    public int BaseDamage { get { return baseDamage; } }
}
