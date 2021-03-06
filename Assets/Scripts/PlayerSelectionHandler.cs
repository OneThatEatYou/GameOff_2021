using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class PlayerSelectionHandler : Singleton<PlayerSelectionHandler>
{
    [SerializeField, ReadOnly] private Targetable hoveredTarget;
    public Targetable HoveredTarget { get { return hoveredTarget; } }

    [SerializeField, ReadOnly] private CardHolder selectedCard;
    public CardHolder SelectedCard { get { return selectedCard; } }

    private void Awake()
    {
        InitializeSingleton();
    }

    public void SetHoveredTarget(Targetable targetable)
    {
        hoveredTarget = targetable;
    }

    public void SelectCard(CardHolder newCard)
    {
        if (selectedCard == newCard) return;

        //Debug.Log($"Old Selection: {selectedCard}, New Selection: {newCard}");

        if (selectedCard)
        {
            selectedCard.onUnselected?.Invoke();
        }

        if (newCard)
        {
            newCard.onSelected?.Invoke();
        }

        selectedCard = newCard;
    }
}
