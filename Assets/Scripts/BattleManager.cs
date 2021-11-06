using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
public class BattleManager : MonoBehaviour
{
    public enum TurnStatus
    {
        Player,
        Enemy
    }

    private static BattleManager instance;
    public static BattleManager Instance
    {
        get
        {
            // check if BattleManager exists in scene
            if (instance == null) instance = FindObjectOfType<BattleManager>();
            if (instance == null)
            {
                Debug.LogError("BattleManager not found in scene");
                instance = new GameObject("BattleManager").AddComponent<BattleManager>();
            }

            return instance;
        }
    }

    public Vector2 MouseWorldPosition
    {
        get { return Camera.main.ScreenToWorldPoint(input.BattleActions.MousePosition.ReadValue<Vector2>()); }
    }

    [Header("Selection")]
    [SerializeField, ReadOnly] private Targetable hoveredTarget;
    [SerializeField, ReadOnly] private CardHolder selectedCard;
    [SerializeField, ReadOnly] private Vector2 selectOffset;
    [SerializeField, ReadOnly] private bool isHoldingSelect;

    [Header("Turn Info")]
    [SerializeField, ReadOnly] private int turnNum = 1;
    [SerializeField, ReadOnly] private TurnStatus curTurnStatus;

    private MainInput input;

    private void Awake()
    {
        input = new MainInput();
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

        if (hoveredTarget && (hoveredTarget is CardHolder))
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
    }
}
