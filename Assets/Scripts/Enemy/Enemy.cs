using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Action<Enemy> onDeath;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform contactPointPivot;
    [SerializeField] private Transform contactPoint;

    private Health health;
    private Hurtbox hurtbox;
    private HitFlash hitFlash;
    private SlashEffect slashEffect;
    private const string TAG = "Enemy";

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private float baseMoveSpeed = 2f;
    private float moveSpeed = 2f;
    private int damage = 1;
    private bool isActive = false;

    void Awake()
    {
        health = GetComponentInChildren<Health>();
        health.onDeath += () => Deactivate();

        hurtbox = GetComponentInChildren<Hurtbox>();
        hurtbox.onHit += OnHit;

        hitFlash = GetComponentInChildren<HitFlash>();
        slashEffect = GetComponentInChildren<SlashEffect>();
    }

    public void Initialize(
        EnemyInfo info, 
        Vector3 targetPosition)
    {
        // Debug.Log($"Initializing enemy: {info.enemyType} at position {transform.position}");
        this.initialPosition = transform.position;
        this.targetPosition = targetPosition;
        baseMoveSpeed = info.moveSpeed;
        SetMoveSpeed();
        damage = info.damage;
        transform.position = initialPosition;
        
        health.Initialize(info.health);

        hurtbox?.SetTag(TAG);

        slashEffect.ToggleSprite(false);

        isActive = true;
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
        if (!isActive) return;

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        transform.position += directionToTarget * baseMoveSpeed * speedMultiplier * Time.deltaTime;
        Vector3 directionToPlayer = (GameManager.Instance.GetPlayer().transform.position - transform.position).normalized;
        contactPointPivot.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToPlayer);
    }

    private void Attack()
    {
        // TODO
    }

    public void Deactivate()
    {
        isActive = false;
        spriteRenderer.gameObject.SetActive(false);
        hurtbox.gameObject.SetActive(false);
        TriggerSlash();
        onDeath?.Invoke(this);
    }

    private void TriggerSlash()
    {
        slashEffect.ToggleSprite(true);
        slashEffect?.TriggerSlash();
    }

    private void OnHit(int damage)
    {
        hitFlash.TriggerHitFlash();
        health.TakeDamage(damage);
    }

    public bool IsActive() => isActive;
    public Transform GetContactPoint() => contactPoint;
}
