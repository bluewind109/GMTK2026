using Cysharp.Threading.Tasks;
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
        // hitbox.onHit += OnHit;
    }

    void OnDestroy()
    {
        // hitbox.onHit -= OnHit;
    }

    void Start()
    {
        hitbox.Initialize(TAG);
        hitbox.gameObject.SetActive(false);
    }

    public void Initialize(Vector2 size, int damage, Vector2 attackOffset)
    {
        this.damage = damage;
        // this.attackDuration = duration;
        hitbox.SetArea(size);
        hitbox.transform.localPosition = new Vector3(
            hitbox.transform.localPosition.x + attackOffset.x, 
            hitbox.transform.localPosition.y + attackOffset.y, 
            0f
        );
        _ = Execute();
    }

    private async UniTask Execute()
    {
        await UniTask.Yield();
        hitbox.gameObject.SetActive(true);
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitbox.transform.position, hitbox.transform.localScale, 0f);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag(TAG)) continue;

            var hurtbox = hit.GetComponentInParent<Hurtbox>();
            if (hurtbox != null)
            {
                OnHit(hurtbox);
            }
        }

        await UniTask.WaitForSeconds(attackDuration);
        DeactivateHitbox();
    }

    private void DeactivateHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }

    private void OnHit(Hurtbox hurtbox)
    {
        Debug.Log($"Hitbox hit: {hurtbox.name}, dealing {damage} damage.");
        hurtbox.TakeDamage(damage);
    }
}