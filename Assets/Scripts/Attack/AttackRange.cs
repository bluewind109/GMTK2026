using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public System.Action onEnemyInRange;
    public System.Action onEnemyOutOfRange;

    [SerializeField] private float spriteSize = 256f;
    [SerializeField] private float defaultSize = 100f;

    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("AttackRange requires a BoxCollider2D component.");
        }
    }

    public void SetRange(float range)
    {
        float scale = range * (spriteSize / defaultSize);
        transform.localScale = new Vector3(scale, scale, 1f);
        boxCollider.size = new Vector2(1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            onEnemyInRange?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            onEnemyOutOfRange?.Invoke();
        }
    }
}
