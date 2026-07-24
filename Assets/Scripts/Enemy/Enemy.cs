using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 initialPosition;
    private float moveSpeed = 2f;
    private int damage = 1;
    private float attackRange = 1f;

    public void Initialize(float speed, int damageAmount, float range)
    {
        moveSpeed = speed;
        damage = damageAmount;
        attackRange = range;
    }

    public void Reset()
    {
        transform.position = initialPosition;
        // gameObject.SetActive(false);
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    private void Attack()
    {
        // TODO
    }

    public void Activate(Vector3 position)
    {
        initialPosition = position;
        transform.position = initialPosition;
    }

    public void Deactivate()
    {
        // TODO
    }

    public bool IsActive() => gameObject.activeSelf;
}
