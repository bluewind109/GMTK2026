using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private BaseHealthBar healthBar;

    private Health health;
    private Hurtbox hurtbox;

    void Awake()
    {
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

    private void OnHealthChanged(int currentHealth)
    {
        float healthPercent = (float)currentHealth / maxHealth;
        healthBar.UpdateHealthBar(healthPercent);
    }

    private void OnHit(int damage)
    {
        health.TakeDamage(damage);
    }

    private void OnDeath()
    {
        // Handle base death logic here
        gameObject.SetActive(false);
        GameManager.Instance.EndGame(false);
    }
}
