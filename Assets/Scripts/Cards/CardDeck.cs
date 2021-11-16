using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardDeck", menuName = "ScriptableObjects/CardDeck")]
public class CardDeck : ScriptableObject
{
    [SerializeField] private List<CardData> cards;

    public CardData GetRandomCard()
    {
        return cards[Random.Range(0, cards.Count)];
    }

    public void AddCard(CardData newCard)
    {
        cards.Add(newCard);
    }

    public void RemoveCard(CardData card)
    {
        cards.Remove(card);
    }
}
