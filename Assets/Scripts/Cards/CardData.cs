using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "ScriptableObjects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName = "NewCard";
    public string description;
    public CardEffect[] effects;

    public void UseCard(Character character)
    {
        Debug.Log($"Using {cardName} on {character.name}");
        ApplyEffects(character);
    }

    private void ApplyEffects(Character character)
    {
        foreach (CardEffect effect in effects)
        {
            effect.ApplyEffect(character);
        }
    }
}
