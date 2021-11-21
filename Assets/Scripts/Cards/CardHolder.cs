using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class CardHolder : Targetable
{
    [Expandable] public CardData card;
    public TextMeshProUGUI nameText;
    public RectTransform descriptionRect;
    public TextMeshProUGUI descriptionText;

    [SerializeField, ReadOnly] private Vector2 basePos;

    [Header("Animation")]
    public float hoveredMoveDistance;
    public float hoveredMoveSpeed;
    public float fillDur = 1;
    public Material dissolveMat;
    public float dissolveDur = 0.5f;
    [SerializeField, ReadOnly] private bool isSpawningCard;
    [SerializeField, ReadOnly] private bool isDestroyingCard;
    [SerializeField, ReadOnly] private bool isSelected;

    private Canvas canvas;
    private GraphicRaycaster graphicRaycaster;
    private Image cardImage;
    private Material cardMat;
    private Coroutine hoveredOverCoroutine;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        cardImage = GetComponent<Image>();
        cardImage.material = new Material(cardImage.material);
        cardMat = cardImage.material;
    }

    private void Start()
    {
        SetText();
        basePos = transform.position;
    }

    private void OnEnable()
    {
        onPointerEnter += ShowHoveredOver;
        onPointerExit += StopHoveredOver;
        onSelected += OnSelected;
        onUnselected += OnUnselected;
    }

    private void OnDisable()
    {
        onPointerExit += StopHoveredOver;
        onPointerExit -= StopHoveredOver;
        onSelected -= OnSelected;
        onUnselected -= OnUnselected;
    }

    public void Initialize(CardData cardData)
    {
        card = Instantiate(cardData);
        if (card.cardImage) cardImage.sprite = card.cardImage;
        else Debug.LogWarning($"Card image of {card.cardName} not assigned");

        PlaySpawnCardAnimation();
    }

    public void ShowHoveredOver()
    {
        if (isSelected || isDestroyingCard) return;

        if (hoveredOverCoroutine != null) StopCoroutine(hoveredOverCoroutine);
        hoveredOverCoroutine = StartCoroutine(MoveCard(basePos + Vector2.up * hoveredMoveDistance));

        descriptionRect.gameObject.SetActive(true);
    }

    public void StopHoveredOver()
    {
        if (isSelected || isDestroyingCard) return;

        if (hoveredOverCoroutine != null) StopCoroutine(hoveredOverCoroutine);
        hoveredOverCoroutine = StartCoroutine(MoveCard(basePos));

        descriptionRect.gameObject.SetActive(false);
    }

    private void OnSelected()
    {
        isSelected = true;

        if (hoveredOverCoroutine != null) StopCoroutine(hoveredOverCoroutine);
        SetLayerFront();
        descriptionRect.gameObject.SetActive(false);
    }

    private void OnUnselected()
    {
        isSelected = false;

        SetLayerBack();
    }

    private IEnumerator MoveCard(Vector2 targetPos)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.01f)
        {
            Vector2 newPos = transform.position;
            newPos.x = Mathf.Lerp(transform.position.x, targetPos.x, Time.deltaTime * hoveredMoveSpeed);
            newPos.y = Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * hoveredMoveSpeed);
            transform.position = newPos;
            yield return null;
        }
    }

    public void SetLayerFront()
    {
        SetLayer(2);
    }

    public void SetLayerBack()
    {
        SetLayer(1);
    }
    
    // returns true if the card is used
    public bool TryUseCard(Targetable targetable)
    {
        if (targetable is Character)
        {
            card.UseCard(targetable as Character);
            DestoyCard();
            BattleManager.Instance.Player.EndTurn(dissolveDur + 0.5f);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ToggleRaycastable(bool canBlockRaycast)
    {
        graphicRaycaster.enabled = canBlockRaycast;
    }

    public void DestoyCard()
    {
        if (isDestroyingCard) return;

        isDestroyingCard = true;
        targetable = false;
        cardImage.material = new Material(dissolveMat);
        dissolveMat = cardImage.material;
        dissolveMat.SetFloat("_Amount", 0);
        StartCoroutine(DestroyCardCoroutine());
    }

    private IEnumerator DestroyCardCoroutine()
    {
        float elapsed = 0;

        while (elapsed < dissolveDur)
        {
            elapsed += Time.deltaTime;
            dissolveMat.SetFloat("_Amount", elapsed / dissolveDur);

            yield return null;
        }

        dissolveMat.SetFloat("_Amount", 1);
        Destroy(gameObject);
    }

    private void SetLayer(int order)
    {
        canvas.sortingOrder = order;
    }

    private void PlaySpawnCardAnimation()
    {
        if (isSpawningCard) return;

        isSpawningCard = true;
        targetable = false;
        StartCoroutine(SpawnCardAnimationCoroutine());
    }

    private IEnumerator SpawnCardAnimationCoroutine()
    {
        float elapsed = 0;

        while (elapsed < fillDur)
        {
            elapsed += Time.deltaTime;
            cardMat.SetFloat("_Amount", elapsed / fillDur);

            yield return null;
        }

        cardMat.SetFloat("_Amount", 1);
        targetable = true;
        isSpawningCard = false;
    }

    [ButtonMethod]
    private void SetText()
    {
        if (!card)
        {
            Debug.LogError("Card not assigned");
            return;
        }

        if (nameText) nameText.text = card.cardName;
        else Debug.LogError("nameText not found");

        if (descriptionText) descriptionText.text = card.description;
        else Debug.LogError("descriptonText not found");
    }

    private void OnValidate()
    {
        if (card && nameText && descriptionText)
        {
            SetText();
        }
    }
}
