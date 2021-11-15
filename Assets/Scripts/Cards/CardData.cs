using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "ScriptableObjects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName = "NewCard";
    public string description;
    public Sprite cardImage;
    public CardEffect[] effects;

    private bool isApplyingEffects;

    public void UseCard(Character character)
    {
        Debug.Log($"Using {cardName} on {character.name}");
        ApplyEffects(character);
    }

    private void ApplyEffects(Character character)
    {
        if (isApplyingEffects) return;

        isApplyingEffects = true;
        character.StartCoroutine(ApplyEffectsCoroutine(character, BattleManager.Instance.cardEffectInitialDelay, BattleManager.Instance.cardEffectTriggerDelay));
    }

    private IEnumerator ApplyEffectsCoroutine(Character character, float initialDelay, float loopDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        foreach (CardEffect effect in effects)
        {
            effect.ApplyEffect(character);
            yield return new WaitForSeconds(loopDelay);
        }

        isApplyingEffects = false;
    }
}
