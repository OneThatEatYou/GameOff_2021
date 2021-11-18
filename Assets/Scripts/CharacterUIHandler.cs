using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Character))]
public class CharacterUIHandler : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public RectTransform descriptionRect;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        UpdateDescriptionText();
    }

    private void OnEnable()
    {
        character.onHealthChange += UpdateHealth;
        character.onDamageChange += UpdateDamage;
        character.onPointerEnter += ShowHoveredOver;
        character.onPointerExit += StopHoveredOver;
    }

    private void OnDisable()
    {
        character.onHealthChange -= UpdateHealth;
        character.onDamageChange -= UpdateDamage;
        character.onPointerEnter -= ShowHoveredOver;
        character.onPointerExit -= StopHoveredOver;
    }

    private void UpdateDescriptionText()
    {
        if (!descriptionText || !character || !character.characterData) return;

        nameText.text = character.characterData.enemyName;
        descriptionText.text = character.characterData.description;
    }

    private void UpdateHealth(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }

    private void UpdateDamage(int newDamage)
    {
        damageText.text = newDamage.ToString();
    }

    private void ShowHoveredOver()
    {
        descriptionRect.gameObject.SetActive(true);
    }

    private void StopHoveredOver()
    {
        descriptionRect.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        UpdateDescriptionText();
    }
}
