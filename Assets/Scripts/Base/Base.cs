using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private GameObject healthBar;

    private Health health;
    private Hurtbox hurtbox;

    void Awake()
    {
        health = GetComponentInChildren<Health>();
        health.Initialize(maxHealth);
        health.onDeath += OnDeath;

        hurtbox = GetComponentInChildren<Hurtbox>();
    }

    void OnDestroy()
    {
        health.onDeath -= OnDeath;
    }

    private void OnDeath()
    {
        // Handle base death logic here
        gameObject.SetActive(false);
    }
}
