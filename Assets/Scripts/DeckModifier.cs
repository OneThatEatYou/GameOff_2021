using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckModifier : MonoBehaviour
{
    [System.Serializable]
    public class CardConditions
    {
        public int minLevel;
        public CardData card;
    }

    public Vector2 CardHolderArea
    {
        get
        {
            Vector2 min = choiceRect.rect.min;
            Vector2 max = choiceRect.rect.max;
            float resolutionScale = Camera.main.pixelWidth / BattleManager.Instance.referenceResolution.x;
            Vector2 cardHolderParentWorldSize = Camera.main.ScreenToWorldPoint(max) - Camera.main.ScreenToWorldPoint(min);
            return cardHolderParentWorldSize * resolutionScale * cardHolderAreaScale;
        }
    }

    public CardConditions[] cardPool;

    [Header("UI")]
    public RectTransform choiceRect;
    public float cardHolderAreaScale = 1;
    public CardPosition[] cardChoices;

    private void Start()
    {
        UpdateCardPositions();
    }

    public void ShowPanel()
    {
        choiceRect.gameObject.SetActive(true);
    }

    private CardData[] PickCards(int cardNum)
    {
        List<CardData> pickableCards = new List<CardData>();
        CardData[] cardsPicked = new CardData[cardNum];

        // filter out cards that cannot be choosen
        foreach (CardConditions cardCond in cardPool)
        {
            if (ProgressManager.Instance.curLevel >= cardCond.minLevel)
            {
                pickableCards.Add(cardCond.card);
            }
        }

        for (int i = 0; i < cardNum; i++)
        {
            cardsPicked[i] = pickableCards[Random.Range(0, pickableCards.Count)];
        }

        return cardsPicked;
    }

    [ContextMenu("Update Card Positions")]
    private void UpdateCardPositions()
    {
        Vector2 positionOffset = new Vector2(CardHolderArea.x / cardChoices.Length, 0);

        for (int i = 0; i < cardChoices.Length; i++)
        {
            cardChoices[i].position = (Vector2)choiceRect.position + positionOffset * i - new Vector2(positionOffset.x / 2 * (cardChoices.Length - 1), 0);
        }
    }

    private void OnValidate()
    {
        if (cardChoices.Length > 0 && choiceRect)
        {
            UpdateCardPositions();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // draw card panel
        if (choiceRect)
        {
            Gizmos.DrawWireCube(choiceRect.position, CardHolderArea);
        }

        // draw card positions
        if (cardChoices != null)
        {
            foreach (CardPosition cc in cardChoices)
            {
                Gizmos.DrawWireSphere(cc.position, 0.5f);
            }
        }
    }
}
