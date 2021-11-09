using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class BattleManager : Singleton<BattleManager>
{
    [System.Serializable]
    public struct CardPosition
    {
        [ReadOnly] public Vector2 position;
        [ReadOnly] public CardHolder cardHolder;
    }

    public enum TurnStatus
    {
        Player,
        Enemy
    }

    public Vector2 MouseWorldPosition
    {
        get { return Camera.main.ScreenToWorldPoint(input.BattleActions.MousePosition.ReadValue<Vector2>()); }
    }

    private Player player;
    public Player Player
    {
        get
        {
            if (!player)
            {
                player = FindObjectOfType<Player>();
            }

            return player;
        }
    }

    private List<Enemy> enemies = new List<Enemy>();

    public CardDeck deck;
    public GameObject cardHolderPrefab;
    public RectTransform cardHolderParent;
    public CanvasGroup cardAreaCanvasGroup;
    public Vector2 cardHolderArea;
    public CardPosition[] heldCards;

    [Header("Selection")]
    [SerializeField, ReadOnly] private Targetable hoveredTarget;
    [SerializeField, ReadOnly] private CardHolder selectedCard;
    [SerializeField, ReadOnly] private Vector2 selectOffset;
    [SerializeField, ReadOnly] private bool isHoldingSelect;

    [Header("Turn Info")]
    [SerializeField, ReadOnly] private int turnNum = 1;
    [SerializeField, ReadOnly] private TurnStatus curTurnStatus;

    private MainInput input;
    private bool isBattling;

    public delegate void BattleDelegate();
    public BattleDelegate onBattleStartCallback;
    public BattleDelegate onBattleEndCallback;

    private void Awake()
    {
        InitializeSingleton();
        input = new MainInput();
    }

    private void Start()
    {
        UpdateCardPositions();
        StartBattle();
    }

    private void OnEnable()
    {
        input.Enable();
        input.BattleActions.Select.started += ctx => StartHoldingSelect();
        input.BattleActions.Select.canceled += ctx => StopHoldingSelect();
    }

    private void OnDisable()
    {
        input.Disable();
        input.BattleActions.Select.started -= ctx => StartHoldingSelect();
        input.BattleActions.Select.canceled -= ctx => StopHoldingSelect();
    }

    private void Update()
    {
        if (isHoldingSelect)
        {
            WhileHoldingSelect();
        }
    }

    public void SetHoveredTarget(Targetable targetable)
    {
        hoveredTarget = targetable;
    }

    private void StartHoldingSelect()
    {
        isHoldingSelect = true;

        // card selection and usage logic
        if (curTurnStatus == TurnStatus.Player && hoveredTarget && (hoveredTarget is CardHolder))
        {
            selectedCard = hoveredTarget as CardHolder;
            selectOffset = (Vector2)selectedCard.transform.position - MouseWorldPosition;
            selectedCard.ToggleRaycastable(false);
        }
    }

    private void WhileHoldingSelect()
    {
        if (selectedCard) selectedCard.DragCard(selectOffset);
    }

    private void StopHoldingSelect()
    {
        isHoldingSelect = false;

        if (selectedCard)
        {
            selectedCard.TryUseCard(hoveredTarget);
            selectedCard.ToggleRaycastable(true);
            selectedCard = null;
        }
    }

    [ContextMenu("Proceed Turn")]
    public void ProceedTurn()
    {
        curTurnStatus = curTurnStatus.Next();
        if (curTurnStatus == 0) turnNum++;

        EvaluateTurn();
    }

    private void StartBattle()
    {
        isBattling = true;
        StartCoroutine(StartBattleCoroutine());
    }

    private void EndBattle()
    {
        if (!isBattling) return;

        isBattling = false;
        StartCoroutine(EndBattleCoroutine());
        Debug.Log("Ending battle");
    }

    private IEnumerator StartBattleCoroutine()
    {
        yield return new WaitForSeconds(1);

        cardAreaCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(1);

        EvaluateTurn();
        DrawCard();

        onBattleStartCallback?.Invoke();
    }

    private IEnumerator EndBattleCoroutine()
    {
        yield return new WaitForSeconds(1);

        foreach (var heldCard in heldCards)
        {
            if (heldCard.cardHolder) heldCard.cardHolder.DestoyCard();
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1);

        cardAreaCanvasGroup.alpha = 0;

        onBattleEndCallback?.Invoke();
    }

    private void DrawCard()
    {
        for (int i = 0; i < heldCards.Length; i++)
        {
            if (!heldCards[i].cardHolder)
            {
                CardData cardData = deck.GetRandomCard();
                CreateCard(cardData, i);
            }
        }
    }

    private void CreateCard(CardData cardData, int cardPosIndex)
    {
        CardHolder cardHolder = Instantiate(cardHolderPrefab, cardHolderParent).GetComponent<CardHolder>();
        cardHolder.card = Instantiate(cardData);
        cardHolder.transform.position = heldCards[cardPosIndex].position;
        heldCards[cardPosIndex].cardHolder = cardHolder;
    }

    private void EvaluateTurn()
    {
        // check if battle ended
        if (CheckBattleStatus()) return;

        switch (curTurnStatus)
        {
            case TurnStatus.Player:
                StartCoroutine(EvaluatePlayerTurn());
                break;
            case TurnStatus.Enemy:
                StartCoroutine(EvaluateEnemyTurn());
                break;
        }
    }

    private IEnumerator EvaluatePlayerTurn()
    {
        Debug.Log($"It's {Player.name}'s turn");
        DrawCard();
        StartCoroutine(Player.Evaluate());

        while (Player.IsExecutingTurn)
        {
            yield return null;
        }

        ProceedTurn();
    }

    private IEnumerator EvaluateEnemyTurn()
    {
        foreach (Enemy enemy in enemies)
        {
            Debug.Log($"It's {enemy.name}'s turn");
            StartCoroutine(enemy.Evaluate());

            // wait until the turn finished
            while (enemy.IsExecutingTurn)
            {
                yield return null;
            }
        }

        ProceedTurn();
    }

    [ContextMenu("Update Card Positions")]
    private void UpdateCardPositions()
    {
        Vector2 positionOffset = new Vector2(cardHolderArea.x / heldCards.Length, 0);

        for (int i = 0; i < heldCards.Length; i++)
        {
            heldCards[i].position = (Vector2)cardHolderParent.position + positionOffset * i - new Vector2(positionOffset.x / 2 * (heldCards.Length - 1), 0);
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    /// <summary>
    /// Returns true if there is no enemies left. False otherwise
    /// </summary>
    private bool CheckBattleStatus()
    {
        if (enemies.Count == 0)
        {
            EndBattle();
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (cardHolderParent)
        {
            Gizmos.DrawWireCube(cardHolderParent.position, cardHolderArea);
        }

        foreach (CardPosition hc in heldCards)
        {
            Gizmos.DrawWireSphere(hc.position, 0.5f);
        }
    }

    private void OnValidate()
    {
        if (heldCards.Length > 0)
        {
            UpdateCardPositions();
        }
    }
}
