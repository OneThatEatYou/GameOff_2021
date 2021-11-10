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
    public TextMeshProUGUI descriptionText;

    [SerializeField, ReadOnly] private Vector2 basePos;

    private GraphicRaycaster graphicRaycaster;
    private Image cardImage;

    private void Awake()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        cardImage = GetComponent<Image>();
    }

    private void Start()
    {
        SetText();
        basePos = transform.position;
    }

    public void Initialize(CardData cardData)
    {
        card = Instantiate(cardData);
        if (card.cardImage) cardImage.sprite = card.cardImage;
        else Debug.LogWarning($"Card image of {card.cardName} not assigned");
    }

    public void DragCard(Vector2 offest)
    {
        transform.position = BattleManager.Instance.MouseWorldPosition + offest;
    }

    public void TryUseCard(Targetable targetable)
    {
        if (targetable is Character)
        {
            card.UseCard(targetable as Character);
            DestoyCard();
            BattleManager.Instance.Player.EndTurn();
        }
        else
        {
            ResetPosition();
        }
    }

    public void ToggleRaycastable(bool canBlockRaycast)
    {
        graphicRaycaster.enabled = canBlockRaycast;
    }

    private void ResetPosition()
    {
        transform.position = basePos;
    }

    public void DestoyCard()
    {
        Destroy(gameObject);
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
