using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Card")]
public class Card : ScriptableObject
{
    public string cardName = "NewCard";
    public string description;
}
