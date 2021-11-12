using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public abstract class Character : Targetable
{
    public CharacterData characterData;

    [SerializeField, ReadOnly] private int curHealth;
    public int CurHealth
    {
        get { return curHealth; }
        set
        {
            curHealth = Mathf.Max(0, value);
            onHealthChange?.Invoke(curHealth);

            if (curHealth == 0) onDeathCallback?.Invoke(this);
        }
    }

    [SerializeField, ReadOnly] private int damageBoost;
    public int Damage { get { return characterData.BaseDamage + damageBoost; } }
    public int MaxHealth { get { return characterData.BaseHealth; } }

    [SerializeField, ReadOnly] private bool isExecutingTurn = false;
    public bool IsExecutingTurn
    {
        get { return isExecutingTurn; }
    }

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public delegate void ValueChangeCallback(int val);
    public ValueChangeCallback onHealthChange;
    public ValueChangeCallback onDamageChange;
    public delegate void CharacterCallback(Character character);
    public CharacterCallback onDeathCallback;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        CurHealth = MaxHealth;
    }

    protected virtual void OnEnable()
    {
        onDeathCallback += Die;
    }

    protected virtual void OnDisable()
    {
        onDeathCallback -= Die;
    }

    public void Initialize(CharacterData data)
    {
        characterData = Instantiate(data);
        spriteRenderer.sprite = data.CharacterSprite;
        animator.runtimeAnimatorController = data.CharacterAnimator;
        // set health and dmg UI here
    }

    public void TakeDamage(int damage)
    {
        CurHealth -= damage;
        Debug.Log($"{name} took {damage} damage");
    }

    public virtual IEnumerator Evaluate()
    {
        isExecutingTurn = true;
        yield return null;
        isExecutingTurn = false;
    }

    public void StartTurn()
    {
        isExecutingTurn = true;
    }

    public void EndTurn()
    {
        isExecutingTurn = false;
    }

    public void BoostDamage(int change)
    {
        damageBoost += change;
        onDamageChange?.Invoke(Damage);
    }

    protected abstract void Die(Character character);
}
