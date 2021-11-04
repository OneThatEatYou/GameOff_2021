using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class CardHolder : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private void Start()
    {
        SetText();    
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
