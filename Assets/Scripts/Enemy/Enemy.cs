using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Action<Enemy> onDeath;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private int health = 1;
    private float moveSpeed = 2f;
    private int damage = 1;

    public void Initialize(
        EnemyInfo info, 
        Vector3 initialPosition, 
        Vector3 targetPosition)
    {
        this.initialPosition = initialPosition;
        this.targetPosition = targetPosition;
        health = info.health;
        moveSpeed = info.moveSpeed;
        damage = info.damage;
        transform.position = initialPosition;
    }

    public void Reset()
    {
        transform.position = initialPosition;
        // gameObject.SetActive(false);
    }

    public void MoveTowardsTarget()
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
        onDeath?.Invoke(this);
    }

    public bool IsActive() => gameObject.activeSelf;
}
