using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardDeck", menuName = "ScriptableObjects/CardDeck")]
public class CardDeck : ScriptableObject
{
    public CardData[] cards;
}
