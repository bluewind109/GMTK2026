using Cysharp.Threading.Tasks;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Hitbox hitbox;
    private float attackDuration = 5f;
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
        hitbox.gameObject.SetActive(false);
    }

    public void Initialize(Vector2 size, int damage, float duration)
    {
        this.damage = damage;
        this.attackDuration = duration;
        hitbox.SetArea(size);
        _ = Execute();
    }

    private async UniTask Execute()
    {
        await UniTask.Yield();
        hitbox.gameObject.SetActive(true);
        await UniTask.WaitForSeconds(attackDuration);
        DeactivateHitbox();
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