using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public abstract class Character : Targetable
{
    [Expandable] public CharacterData characterData;

    [SerializeField, ReadOnly] private int curHealth;
    public int CurHealth
    {
        get { return curHealth; }
        set
        {
            curHealth = Mathf.Max(0, value);
            onHealthChange?.Invoke(curHealth);

            if (curHealth == 0 || curHealth > OverflowLimit) onDeathCallback?.Invoke(this);
        }
    }

    [SerializeField, ReadOnly] private int damageBoost;
    public int Damage { get { return characterData.BaseDamage + damageBoost; } }
    public int MaxHealth { get { return characterData.BaseHealth; } }
    public int OverflowLimit { get { return characterData.OverflowLimit; } }

    [SerializeField, ReadOnly] private bool isExecutingTurn = false;
    public bool IsExecutingTurn
    {
        get { return isExecutingTurn; }
    }

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isEndingTurn;

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

    public void EndTurn(float delay = 0)
    {
        if (!isExecutingTurn || isEndingTurn) return;

        isEndingTurn = true;
        StartCoroutine(EndTurnCoroutine(delay));
    }

    IEnumerator EndTurnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        isExecutingTurn = false;
        isEndingTurn = false;
    }

    public void BoostDamage(int change)
    {
        damageBoost += change;
        onDamageChange?.Invoke(Damage);
    }

    protected abstract void Die(Character character);
}
