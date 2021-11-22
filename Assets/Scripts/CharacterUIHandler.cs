using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Character))]
public class CharacterUIHandler : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public Image damageImage;
    public RectTransform descriptionRect;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI descriptionText;
    [Space]
    public Sprite attackIconSprite;
    public Sprite healIconSprite;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        UpdateDescriptionText();
        UpdateStatsText();
        UpdateDamage(character.Damage);
    }

    private void OnEnable()
    {
        character.onHealthChange += UpdateHealth;
        character.onDamageChange += UpdateDamage;
        character.onDamageChange += ctx => UpdateStatsText();
        character.onPointerEnter += ShowHoveredOver;
        character.onPointerExit += StopHoveredOver;
    }

    private void OnDisable()
    {
        character.onHealthChange -= UpdateHealth;
        character.onDamageChange -= UpdateDamage;
        character.onDamageChange -= ctx => UpdateStatsText();
        character.onPointerEnter -= ShowHoveredOver;
        character.onPointerExit -= StopHoveredOver;
    }

    private void UpdateDescriptionText()
    {
        if (!descriptionText || !character || !character.characterData) return;

        nameText.text = character.characterData.enemyName;
        descriptionText.text = character.characterData.description;
    }

    private void UpdateStatsText()
    {
        if (!descriptionText || !character || !character.characterData) return;

        statsText.text = "HP Limit: " + character.HealthOverflowLimit + "\n";
        statsText.text += "DMG Limit: " + character.DamageOverflowLimit;
    }

    private void UpdateHealth(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }

    private void UpdateDamage(int newDamage)
    {
        damageText.text = Mathf.Abs(newDamage).ToString();

        if (newDamage < 0)
        {
            damageImage.sprite = healIconSprite;
        }
        else
        {
            damageImage.sprite = attackIconSprite;
        }
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
