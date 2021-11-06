using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Character : Targetable
{
    [SerializeField, ReadOnly] private int curHealth;
    public int CurHealth
    {
        get { return curHealth; }
        set
        {
            curHealth = Mathf.Max(0, value);
            onHealthChange?.Invoke(curHealth);

            if (curHealth == 0) onDeathCallback?.Invoke();
        }
    }

    [SerializeField, ReadOnly] private int curDamage;
    public int CurDamage
    {
        get { return curDamage; }
        set
        {
            curDamage = value;
            onDamageChange?.Invoke(curDamage);
        }
    }

    public int maxHealth = 1;
    public int baseDamage;

    public delegate void ValueChangeCallback(int val);
    public ValueChangeCallback onHealthChange;
    public ValueChangeCallback onDamageChange;
    public delegate void VoidCallback();
    public VoidCallback onDeathCallback;

    public void TakeDamage(int damage)
    {
        CurHealth -= damage;
    }
}
