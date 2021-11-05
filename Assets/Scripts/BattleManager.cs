using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
public class BattleManager : MonoBehaviour
{
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

    [SerializeField, ReadOnly] private Targetable hoveredTarget;
    [SerializeField, ReadOnly] private CardHolder selectedCard;
    [SerializeField, ReadOnly] private Vector2 selectOffset;
    [SerializeField, ReadOnly] private bool isHoldingSelect;

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

    private void DragCard(CardHolder cardHolder, Vector2 offest)
    {
        if (!cardHolder) return;

        cardHolder.transform.position = MouseWorldPosition + offest;
    }

    private void StartHoldingSelect()
    {
        if (!hoveredTarget || !(hoveredTarget is CardHolder)) return;

        isHoldingSelect = true;
        selectedCard = hoveredTarget as CardHolder;
        selectOffset = (Vector2)selectedCard.transform.position - MouseWorldPosition;
    }

    private void WhileHoldingSelect()
    {
        DragCard(selectedCard, selectOffset);
    }

    private void StopHoldingSelect()
    {
        if (!isHoldingSelect) return;

        isHoldingSelect = false;
        selectedCard.Reset();
        selectedCard = null;
    }
}
