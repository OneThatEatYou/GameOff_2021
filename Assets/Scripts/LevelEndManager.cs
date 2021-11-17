using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEndManager : MonoBehaviour
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
    public Vector2 CardHolderAreaOffset
    {
        get
        {
            Vector2 min = choiceRect.rect.min;
            Vector2 max = choiceRect.rect.max;
            float resolutionScale = Camera.main.pixelWidth / BattleManager.Instance.referenceResolution.x;
            Vector2 cardHolderParentWorldSize = Camera.main.ScreenToWorldPoint(max) - Camera.main.ScreenToWorldPoint(min);
            return cardHolderParentWorldSize * resolutionScale * cardHolderAreaOffset;
        }
    }
    public Vector2 CardHolderPosition
    {
        get
        {
            return (Vector2)choiceRect.position + CardHolderAreaOffset;
        }
    }

    public CardConditions[] cardPool;

    [Header("UI")]
    public TextMeshProUGUI levelEndText;
    public RectTransform choiceRect;
    public Vector2 cardHolderAreaScale = Vector2.one;
    public Vector2 cardHolderAreaOffset;
    public CardPosition[] cardChoices;

    [Header("Animation")]
    public float cardPanelSpawnDelay = 5f;
    public float cardSpawnStartDelay = 1f;
    public float cardDrawDelay = 0.5f;

    private GameObject cardHolderPrefab;
    private bool isShowingChoices;

    private void Awake()
    {
        cardHolderPrefab = (GameObject)Resources.Load("Card");
    }

    private void Start()
    {
        UpdateCardPositions();
    }

    public void EndLevel()
    {
        levelEndText.gameObject.SetActive(true);
        ShowChoices();
    }

    private void ShowChoices()
    {
        if (isShowingChoices) return;

        isShowingChoices = true;
        choiceRect.gameObject.SetActive(true);
        StartCoroutine(ShowChoicesCoroutine());
    }

    private IEnumerator ShowChoicesCoroutine()
    {
        CardData[] pickedCards = PickCards(cardChoices.Length);

        for (int i = 0; i < pickedCards.Length; i++)
        {
            CreateCard(pickedCards[i], i);
            yield return new WaitForSeconds(cardDrawDelay);
        }
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
            int pick = Random.Range(0, pickableCards.Count);
            cardsPicked[i] = pickableCards[pick];
            pickableCards.RemoveAt(pick);
        }

        return cardsPicked;
    }

    private void CreateCard(CardData cardData, int cardPosIndex)
    {
        CardHolder cardHolder = Instantiate(cardHolderPrefab, choiceRect).GetComponent<CardHolder>();
        cardHolder.Initialize(cardData);
        cardHolder.transform.position = cardChoices[cardPosIndex].position;
        cardChoices[cardPosIndex].cardHolder = cardHolder;
    }

    [ContextMenu("Update Card Positions")]
    private void UpdateCardPositions()
    {
        Vector2 positionOffset = new Vector2(CardHolderArea.x / cardChoices.Length, 0);

        for (int i = 0; i < cardChoices.Length; i++)
        {
            cardChoices[i].position = CardHolderPosition + positionOffset * i - new Vector2(positionOffset.x / 2 * (cardChoices.Length - 1), 0);
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
            Gizmos.DrawWireCube(CardHolderPosition, CardHolderArea);
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
