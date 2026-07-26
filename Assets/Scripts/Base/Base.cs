using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private BaseHealthBar healthBar;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField, Min(0f)] private float damageFlashDuration = 0.08f;

    private Health health;
    private Hurtbox hurtbox;
    private Coroutine damageFlashRoutine;
    private Color defaultSpriteColor;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            defaultSpriteColor = spriteRenderer.color;
        }

        health = GetComponentInChildren<Health>();
        health.Initialize(maxHealth);
        health.onHealthChanged += OnHealthChanged;
        health.onDeath += OnDeath;

        hurtbox = GetComponentInChildren<Hurtbox>();
        hurtbox.onHit += OnHit;
    }

    void OnDestroy()
    {
        health.onDeath -= OnDeath;
        health.onHealthChanged -= OnHealthChanged;
        hurtbox.onHit -= OnHit;
    }

    void OnDisable()
    {
        if (damageFlashRoutine != null)
        {
            StopCoroutine(damageFlashRoutine);
            damageFlashRoutine = null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultSpriteColor;
        }
    }

    private void OnHealthChanged(int currentHealth)
    {
        float healthPercent = (float)currentHealth / maxHealth;
        healthBar.UpdateHealthBar(healthPercent);
    }

    private void OnHit(int damage)
    {
        TriggerDamageFlash();
        health.TakeDamage(damage);
    }

    private void OnDeath()
    {
        // Handle base death logic here
        gameObject.SetActive(false);
        GameManager.Instance.EndGame(false);
    }

    private void TriggerDamageFlash()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError($"{nameof(Base)} on {name} cannot flash because no {nameof(SpriteRenderer)} is assigned.", this);
            return;
        }

        if (damageFlashRoutine != null)
        {
            StopCoroutine(damageFlashRoutine);
        }

        damageFlashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        spriteRenderer.color = damageFlashColor;

        if (damageFlashDuration > 0f)
        {
            yield return new WaitForSeconds(damageFlashDuration);
        }

        spriteRenderer.color = defaultSpriteColor;
        damageFlashRoutine = null;
    }
}
