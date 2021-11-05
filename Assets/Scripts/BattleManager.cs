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

    public GameObject lineObject;

    [SerializeField, ReadOnly] private Targetable hoveredTarget;
    [SerializeField, ReadOnly] private Targetable selectedTarget;
    [SerializeField, ReadOnly] private bool isHoldingSelect;

    private MainInput input;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        input = new MainInput();
        lineRenderer = lineObject.GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.BattleActions.Select.started += ctx => StartDrawLine();
        input.BattleActions.Select.canceled += ctx => StopDrawLine();
    }

    private void OnDisable()
    {
        input.Disable();
        input.BattleActions.Select.started -= ctx => StartDrawLine();
        input.BattleActions.Select.canceled -= ctx => StopDrawLine();
    }

    private void Update()
    {
        if (isHoldingSelect)
        {
            UpdateDrawLine();
        }
    }

    public void SetHoveredTarget(Targetable targetable)
    {
        hoveredTarget = targetable;
    }

    private void StartDrawLine()
    {
        if (!hoveredTarget) return;

        isHoldingSelect = true;
        selectedTarget = hoveredTarget;

        lineObject.SetActive(true);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, MouseWorldPosition);
    }

    private void UpdateDrawLine()
    {
        lineRenderer.SetPosition(1, MouseWorldPosition);
    }

    private void StopDrawLine()
    {
        if (!isHoldingSelect) return;

        isHoldingSelect = false;
        selectedTarget = null;

        lineObject.SetActive(false);
        lineRenderer.positionCount = 0;
    }
}
