using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Action<Enemy> onDeath;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private int health = 1;
    private float baseMoveSpeed = 2f;
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
        baseMoveSpeed = info.moveSpeed;
        SetMoveSpeed();
        damage = info.damage;
        transform.position = initialPosition;
    }

    public void Reset()
    {
        transform.position = initialPosition;
        SetMoveSpeed();
        // gameObject.SetActive(false);
    }

    private void SetMoveSpeed()
    {
        moveSpeed = UnityEngine.Random.Range(baseMoveSpeed * 0.9f, baseMoveSpeed * 1.1f);
    }

    public void MoveTowardsTarget(float speedMultiplier)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * baseMoveSpeed * speedMultiplier * Time.deltaTime;
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
