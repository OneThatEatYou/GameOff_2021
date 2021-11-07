using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardDeck", menuName = "ScriptableObjects/CardDeck")]
public class CardDeck : ScriptableObject
{
    public CardData[] cards;

    public CardData GetRandomCard()
    {
        return cards[Random.Range(0, cards.Length)];
    }
}
