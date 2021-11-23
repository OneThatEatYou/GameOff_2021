using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class BattleManager : Singleton<BattleManager>
{
    [System.Serializable]
    public class EnemyPosition
    {
        public Vector2 position;
        [ReadOnly] public Enemy enemy;
    }

    public enum TurnStatus
    {
        Player,
        Enemy
    }

    public Vector2 MouseRawPosition
    {
        get { return input.BattleActions.MousePosition.ReadValue<Vector2>(); }
    }
    public Vector2 MouseWorldPosition
    {
        get { return Camera.main.ScreenToWorldPoint(MouseRawPosition); }
    }
    public Vector2 CardHolderArea 
    { 
        get 
        {
            float resolutionScale = Camera.main.pixelWidth / referenceResolution.x;
            Vector2 cardHolderParentSize = cardHolderParent.rect.size;
            return cardHolderParentSize * resolutionScale * cardHolderAreaScale;
        }
    }
    public Targetable HoveredTarget { get { return PlayerSelectionHandler.Instance.HoveredTarget; } }
    public CardHolder SelectedCard { get { return PlayerSelectionHandler.Instance.SelectedCard; } }

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

    public Vector2 referenceResolution = new Vector2(320, 180);

    [Header("Characters")]
    public GameObject enemyPrefab;
    public List<EnemyPosition> enemies;

    [Header("Cards")]
    [Expandable] public CardDeck deck;
    public RectTransform cardHolderParent;
    public CanvasGroup cardAreaCanvasGroup;
    public Vector2 cardHolderAreaScale;
    public CardPosition[] heldCards;
    [SerializeField, ReadOnly] private bool isDrawingCard;

    [Header("Selection")]
    [SerializeField, ReadOnly] private Vector2 selectOffset;
    [SerializeField, ReadOnly] private bool isHoldingSelect;

    [Header("Turn Info")]
    [SerializeField, ReadOnly] private int turnNum = 1;
    [SerializeField, ReadOnly] private TurnStatus curTurnStatus;

    [Header("Animation")]
    public float cardPanelSpawnDelay = 1f;
    public float cardSpawnStartDelay = 1f;
    public float cardDrawDelay = 0.5f;
    public float cardDissolveDelay = 0.5f;
    public float cardEffectInitialDelay = 0.5f;
    public float cardEffectTriggerDelay = 0.3f;

    private GameObject cardHolderPrefab;
    private MainInput input;
    private bool isBattling;

    public delegate void BattleDelegate();
    public BattleDelegate onBattleStartCallback;
    public BattleDelegate onBattleEndCallback;
    //public BattleDelegate onTurnEndCallback;

    private void Awake()
    {
        InitializeSingleton();
        cardHolderPrefab = (GameObject)Resources.Load("Card");
        input = new MainInput();
        deck = Instantiate(deck);
    }

    private void Start()
    {
        UpdateCardPositions();
    }

    private void OnEnable()
    {
        //EnableInput();
        input.BattleActions.Select.started += ctx => StartHoldingSelect();
        input.BattleActions.Select.canceled += ctx => StopHoldingSelect();
    }

    private void OnDisable()
    {
        if (input != null)
        {
            DisableInput();
            input.BattleActions.Select.started -= ctx => StartHoldingSelect();
            input.BattleActions.Select.canceled -= ctx => StopHoldingSelect();
        }
    }

    private void Update()
    {
        if (isHoldingSelect)
        {
            WhileHoldingSelect();
        }
    }

    private void StartHoldingSelect()
    {
        isHoldingSelect = true;

        // card selection and usage logic
        if (curTurnStatus == TurnStatus.Player && HoveredTarget && (HoveredTarget is CardHolder))
        {
            PlayerSelectionHandler.Instance.SelectCard(HoveredTarget as CardHolder);
            selectOffset = (Vector2)SelectedCard.transform.position - MouseRawPosition;
            SelectedCard.ToggleRaycastable(false);
        }
    }

    private void WhileHoldingSelect()
    {
        if (SelectedCard) DragCard(SelectedCard.transform, selectOffset);
    }

    private void StopHoldingSelect()
    {
        isHoldingSelect = false;

        if (SelectedCard)
        {
            SelectedCard.TryUseCard(HoveredTarget);
            SelectedCard.ToggleRaycastable(true);
            PlayerSelectionHandler.Instance.SetHoveredTarget(null);
            PlayerSelectionHandler.Instance.SelectCard(null);
        }
    }

    private void DragCard(Transform cardTransform, Vector2 offest)
    {
        cardTransform.position = MouseRawPosition + offest;
    }

    public void DisableInput()
    {
        input.Disable();
    }

    public void EnableInput()
    {
        input.Enable();
    }

    #region Turn Functions
    [ContextMenu("Proceed Turn")]
    public void ProceedTurn()
    {
        curTurnStatus = curTurnStatus.Next();
        if (curTurnStatus == 0) turnNum++;

        EvaluateTurn();
        //onTurnEndCallback?.Invoke();
    }

    public void StartBattle(CharacterData[] enemies)
    {
        if (isBattling)
        {
            Debug.LogWarning("Battle has already started");
            return;
        }

        isBattling = true;
        curTurnStatus = TurnStatus.Player;
        SpawnEnemies(enemies);
        EnableInput();
        StartCoroutine(StartBattleCoroutine());
    }

    private IEnumerator StartBattleCoroutine()
    {
        yield return new WaitForSeconds(cardPanelSpawnDelay);

        //cardAreaCanvasGroup.alpha = 1;
        cardAreaCanvasGroup.gameObject.SetActive(true);

        yield return new WaitForSeconds(cardSpawnStartDelay);

        EvaluateTurn();
        DrawCards();

        onBattleStartCallback?.Invoke();
    }

    public void EndBattle()
    {
        if (!isBattling) return;

        isBattling = false;
        DisableInput();
        StartCoroutine(EndBattleCoroutine());
        Debug.Log("Ending battle");
    }

    private IEnumerator EndBattleCoroutine()
    {
        yield return new WaitForSeconds(1);

        foreach (var heldCard in heldCards)
        {
            if (heldCard.cardHolder)
            {
                heldCard.cardHolder.DestoyCard();
                yield return new WaitForSeconds(cardDissolveDelay);
            }
        }

        yield return new WaitForSeconds(1);

        cardAreaCanvasGroup.gameObject.SetActive(false);

        onBattleEndCallback?.Invoke();
    }

    // Battle status is evaluated at the end of each character's turn
    private void EvaluateTurn()
    {
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
        DrawCards();
        StartCoroutine(Player.Evaluate());

        while (Player.IsExecutingTurn)
        {
            yield return null;
        }

        if (!HasBattleEnded())
        {
            // battle continues
            ProceedTurn();
        }
    }

    private IEnumerator EvaluateEnemyTurn()
    {
        foreach (EnemyPosition enemyPos in enemies)
        {
            if (!enemyPos.enemy) continue;

            StartCoroutine(enemyPos.enemy.Evaluate());

            // wait until the turn finished
            while (enemyPos.enemy && enemyPos.enemy.IsExecutingTurn)
            {
                yield return null;
            }

            if (HasBattleEnded())
            {
                // battle ended. no need to evaluate other enemies' turns
                break;
            }
        }

        if (isBattling)
        {
            // battle continues
            ProceedTurn();
        }
    }

    /// <summary>
    /// Returns true if the battle is ending
    /// </summary>
    public bool HasBattleEnded()
    {
        if (!CanContinueBattle())
        {
            EndBattle();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if there is no enemies left. False otherwise
    /// </summary>
    private bool CanContinueBattle()
    {
        if (!isBattling) return false;
        if (ProgressManager.Instance.playerDeathManager.playerIsDead) return false;

        if (HasEnemyRemaining()) return true;

        return false;
    }

    private bool HasEnemyRemaining()
    {
        foreach (var enemyPos in enemies)
        {
            if (enemyPos.enemy)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Enemy
    private void SpawnEnemies(CharacterData[] enemies)
    {
        int maxPosIndex = enemies.Length - 1;

        foreach (var enemy in enemies)
        {
            if (GetVacantEnemyPos(maxPosIndex, out int i))
            {
                SpawnEnemy(enemy, i);
            }
        }
    }

    private void SpawnEnemy(CharacterData enemyData, int positionIndex)
    {
        Enemy enemy = Instantiate(enemyPrefab, enemies[positionIndex].position, Quaternion.identity).GetComponent<Enemy>();
        enemy.Initialize(enemyData);
        RegisterEnemy(enemy, positionIndex);
        enemy.onDeathCallback += UnregisterEnemy;
    }

    private bool GetVacantEnemyPos(int maxPosIndex, out int index)
    {
        List<int> vacantPosIndex = new List<int>();
        index = -1;

        for (int i = 0; i <= Mathf.Min(maxPosIndex, enemies.Count - 1); i++)
        {
            if (!enemies[i].enemy)
            {
                // position is vacant
                vacantPosIndex.Add(i);
            }
        }

        if (vacantPosIndex.Count > 0)
        {
            index = vacantPosIndex[Random.Range(0, vacantPosIndex.Count)];
            return true;
        }

        return false;
    }

    private void RegisterEnemy(Character enemy, int positionIndex)
    {
        if (enemy is Enemy)
        {
            enemies[positionIndex].enemy = enemy as Enemy;
        }
        else
        {
            Debug.LogError("Attempting to register non-enemy character");
        }
    }

    private void UnregisterEnemy(Character enemy)
    {
        if (enemy is Enemy)
        {
            foreach (EnemyPosition enemyPosition in enemies)
            {
                if (enemyPosition.enemy == enemy)
                {
                    enemyPosition.enemy = null;
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Attempting to unregister non-enemy character");
        }
    }
    #endregion

    private void DrawCards()
    {
        if (isDrawingCard) return;

        isDrawingCard = true;
        StartCoroutine(DrawCardsCoroutine());
    }

    private IEnumerator DrawCardsCoroutine()
    {
        for (int i = 0; i < heldCards.Length; i++)
        {
            if (!heldCards[i].cardHolder)
            {
                CardData cardData = deck.GetRandomCard();
                CreateCard(cardData, i);

                yield return new WaitForSeconds(cardDrawDelay);
            }
        }

        isDrawingCard = false;
    }

    private void CreateCard(CardData cardData, int cardPosIndex)
    {
        CardHolder cardHolder = Instantiate(cardHolderPrefab, cardHolderParent).GetComponent<CardHolder>();
        cardHolder.Initialize(cardData);
        cardHolder.transform.position = heldCards[cardPosIndex].position;
        heldCards[cardPosIndex].cardHolder = cardHolder;
    }

    [ContextMenu("Update Card Positions")]
    private void UpdateCardPositions()
    {
        Vector2 positionOffset = new Vector2(CardHolderArea.x / heldCards.Length, 0);

        for (int i = 0; i < heldCards.Length; i++)
        {
            heldCards[i].position = (Vector2)cardHolderParent.position + positionOffset * i - new Vector2(positionOffset.x / 2 * (heldCards.Length - 1), 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // draw card panel
        if (cardHolderParent)
        {
            Gizmos.DrawWireCube(cardHolderParent.position, CardHolderArea);
        }

        // draw card positions
        foreach (CardPosition hc in heldCards)
        {
            Gizmos.DrawWireSphere(hc.position, 30f);
        }

        // draw enemy positions
        foreach (EnemyPosition ep in enemies)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ep.position, 0.5f);
        }
    }

    private void OnValidate()
    {
        if (heldCards.Length > 0 && cardHolderParent)
        {
            UpdateCardPositions();
        }
    }
}

[System.Serializable]
public class CardPosition
{
    [ReadOnly] public Vector2 position;
    [ReadOnly] public CardHolder cardHolder;
}
