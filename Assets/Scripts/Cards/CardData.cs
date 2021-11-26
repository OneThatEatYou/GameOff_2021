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

    public bool IsApplyingEffects { get; private set; }

    public void UseCard(Character character)
    {
        Debug.Log($"Using {cardName} on {character.name}");
        ApplyEffects(character);
    }

    private void ApplyEffects(Character character)
    {
        if (IsApplyingEffects)
        {
            Debug.Log($"{cardName} is already applying effect. Cannot currently apply effect on {character.name}");
            return;
        }

        IsApplyingEffects = true;
        character.StartCoroutine(ApplyEffectsCoroutine(character, BattleManager.Instance.cardEffectInitialDelay, BattleManager.Instance.cardEffectTriggerDelay));
        //IsApplyingEffects = false;
    }

    private IEnumerator ApplyEffectsCoroutine(Character character, float initialDelay, float loopDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        foreach (CardEffect effect in effects)
        {
            yield return new WaitForSeconds(effect.ApplyEffect(character));
            yield return new WaitForSeconds(loopDelay);
        }

        IsApplyingEffects = false;
        Debug.Log("Finished applying eff");
    }
}
