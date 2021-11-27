using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckModifier : MonoBehaviour
{
    [System.Serializable]
    public class CardConditions
    {
        public int minLevel;
        public CardData card;
        public GameObject indication;
    }

    public Vector2 CardHolderArea
    {
        get
        {
            float resolutionScale = Camera.main.pixelWidth / referenceResolution.x;
            Vector2 cardHolderParentSize = choiceRect.rect.size;
            return cardHolderParentSize * resolutionScale * cardHolderAreaScale;
        }
    }
    public Vector2 CardHolderAreaOffset
    {
        get
        {
            float resolutionScale = Camera.main.pixelWidth / referenceResolution.x;
            Vector2 cardHolderParentSize = choiceRect.rect.size;
            return cardHolderParentSize * resolutionScale * cardHolderAreaOffset;
        }
    }
    public Vector2 CardHolderPosition
    {
        get
        {
            return (Vector2)choiceRect.position + CardHolderAreaOffset;
        }
    }
    public Targetable HoveredTarget { get { return PlayerSelectionHandler.Instance.HoveredTarget; } }
    public CardHolder SelectedCard { get { return PlayerSelectionHandler.Instance.SelectedCard; } }

    public CardConditions[] cardPool;

    public Vector2 referenceResolution = new Vector2(320, 180);

    [Header("UI")]
    public TextMeshProUGUI levelEndText;
    public RectTransform choiceRect;
    public Vector2 cardHolderAreaScale = Vector2.one;
    public Vector2 cardHolderAreaOffset;
    public CardPosition[] cardChoices;
    public Button confirmButton;

    [Header("Animation")]
    public float cardPanelSpawnDelay = 5f;
    public float cardSpawnStartDelay = 1f;
    public float cardDrawDelay = 0.5f;

    [Header("Audio")]
    public AudioClip buttonSFX;
    public AudioClip levelUpSFX;

    public delegate void LevelEndDelegate();
    public LevelEndDelegate onChoiceConfirmed;

    private GameObject cardHolderPrefab;
    private MainInput input;
    private TextMeshProUGUI confirmButtonText;
    private bool isShowingChoices;

    private void Awake()
    {
        input = new MainInput();
        cardHolderPrefab = (GameObject)Resources.Load("Card");
        confirmButtonText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateCardPositions();
    }

    private void OnEnable()
    {
        input.Enable();
        input.NormalActions.Select.performed += ctx => SelectCard();
        input.NormalActions.Select.performed += ctx => UpdateButtons();
    }

    private void OnDisable()
    {
        input.Disable();
        input.NormalActions.Select.performed -= ctx => SelectCard();
        input.NormalActions.Select.performed -= ctx => UpdateButtons();
    }

    public void EndLevel()
    {
        levelEndText.gameObject.SetActive(true);
        ShowChoices();
    }

    public void ConfirmChoice()
    {
        if (SelectedCard != null)
        {
            BattleManager.Instance.deck.AddCard(SelectedCard.card);
            HideChoices();
        }
    }

    public void PlayButtonSFX()
    {
        AudioManager.PlayAudioAtPosition(buttonSFX, transform.position, AudioManager.SFXGroup);
    }

    private void SelectCard()
    {
        if (SelectedCard && HoveredTarget == SelectedCard) PlayerSelectionHandler.Instance.SelectCard(null);
        else if (HoveredTarget is CardHolder)
        {
            PlayerSelectionHandler.Instance.SelectCard(HoveredTarget as CardHolder);
        }
    }

    private void UpdateButtons()
    {
        if (SelectedCard != null)
        {
            confirmButtonText.color = Color.white;
            confirmButton.interactable = true;
        }
        else
        {
            confirmButtonText.color = Color.grey;
            confirmButton.interactable = false;
        }
    }

    private void ShowChoices()
    {
        if (isShowingChoices) return;

        isShowingChoices = true;
        choiceRect.gameObject.SetActive(true);
        AudioManager.PlayAudioAtPosition(levelUpSFX, transform.position, AudioManager.SFXGroup);
        UpdateButtons();
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

    private void HideChoices()
    {
        if (!isShowingChoices) return;

        isShowingChoices = true;
        StartCoroutine(HideChoicesCoroutine());
    }

    private IEnumerator HideChoicesCoroutine()
    {
        foreach (CardPosition cp in cardChoices)
        {
            if (cp.cardHolder && cp.cardHolder != PlayerSelectionHandler.Instance.SelectedCard)
            {
                cp.cardHolder.DestoyCard();
                yield return new WaitForSeconds(BattleManager.Instance.cardDissolveDelay);
            }
        }

        yield return new WaitForSeconds(1);

        isShowingChoices = false;

        // flushes any remaining cards
        foreach (CardPosition cp in cardChoices)
        {
            if (cp.cardHolder)
            {
                Destroy(cp.cardHolder.gameObject);
            }
        }
        choiceRect.gameObject.SetActive(false);
        levelEndText.gameObject.SetActive(false);

        onChoiceConfirmed?.Invoke();
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
