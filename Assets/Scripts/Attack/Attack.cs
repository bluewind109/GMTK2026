using UnityEngine;

public class Attack : MonoBehaviour
{
    private Hitbox hitbox;
    private float attackDuration = 0.1f;
    private int damage = 1;

    private const string TAG = "Enemy";

    void Awake()
    {
        hitbox = GetComponentInChildren<Hitbox>();
        hitbox.onHit += OnHit;
    }

    void OnDestroy()
    {
        hitbox.onHit -= OnHit;
    }

    void Start()
    {
        hitbox.Initialize(TAG);
        hitbox.gameObject.SetActive(true);
        Invoke("DeactivateHitbox", attackDuration);
    }

    private void DeactivateHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }

    private void OnHit(Hurtbox hurtbox)
    {
        hurtbox.TakeDamage(damage);
    }
}