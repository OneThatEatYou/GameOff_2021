using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Character))]
public class StatUIHandler : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        character.onHealthChange += UpdateHealth;
        character.onDamageChange += UpdateDamage;
    }

    private void UpdateHealth(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }

    private void UpdateDamage(int newDamage)
    {
        damageText.text = newDamage.ToString();
    }
}
