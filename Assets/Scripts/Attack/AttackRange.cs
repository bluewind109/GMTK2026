using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public System.Action<Enemy> onEnemyInRange;
    public System.Action<Enemy> onEnemyOutOfRange;

    [SerializeField] private float spriteSize = 256f;
    [SerializeField] private float defaultSize = 100f;

    public void SetRange(float range)
    {
        float scale = range * (spriteSize / defaultSize);
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (enemy != null)
                onEnemyInRange?.Invoke(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (enemy != null)
                onEnemyOutOfRange?.Invoke(enemy);
        }
    }
}
