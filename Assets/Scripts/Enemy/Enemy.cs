using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Action<Enemy> onDeath;

    [SerializeField] private Transform contactPoint;

    private Health health;
    private Hurtbox hurtbox;
    private const string TAG = "Enemy";

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private float baseMoveSpeed = 2f;
    private float moveSpeed = 2f;
    private int damage = 1;

    void Awake()
    {
        health = GetComponentInChildren<Health>();
        health.onDeath += () => Deactivate();

        hurtbox = GetComponentInChildren<Hurtbox>();
        hurtbox.onHit += OnHit;
    }

    public void Initialize(
        EnemyInfo info, 
        Vector3 initialPosition, 
        Vector3 targetPosition)
    {
        this.initialPosition = initialPosition;
        this.targetPosition = targetPosition;
        baseMoveSpeed = info.moveSpeed;
        SetMoveSpeed();
        damage = info.damage;
        transform.position = initialPosition;
        
        health.Initialize(info.health);
        hurtbox.SetTag(TAG);
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

    private void OnHit(int damage)
    {
        health.TakeDamage(damage);
    }

    public bool IsActive() => gameObject.activeSelf;
    public Transform GetContactPoint() => contactPoint;
}
