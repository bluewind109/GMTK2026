using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action<int> onHealthChanged;
    public Action onDeath;

    private int maxHealth = 1;
    private int currentHealth;

    public void Initialize(int health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        onDeath?.Invoke();
    }
}
