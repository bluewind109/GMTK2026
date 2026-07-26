using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AttackConfig attackConfig;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform startLocation;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float globalAttackCooldown = 0.1f;

    private PlayerInput playerInput;
    private AttackTimer globalAttackTimer;
    private AttackRange attackRange;

    private eAttackType lastAttackType;

    private bool canAttack => globalAttackTimer.IsFinished();

    private const string ENEMY_TAG = "Enemy";

    void Start()
    {
        globalAttackTimer = new AttackTimer(globalAttackCooldown);

        playerInput = GetComponentInChildren<PlayerInput>();
        playerInput.onAttackPressed += OnAttackPressed;
        playerInput.onDirectionPressed += OnDirectionPressed;

        attackRange = GetComponentInChildren<AttackRange>();
        attackRange.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        playerInput.onAttackPressed -= OnAttackPressed;
        playerInput.onDirectionPressed -= OnDirectionPressed;
    }

    void Update()
    {
        globalAttackTimer.Update(Time.deltaTime);
        playerInput.UpdateDirectionInput();
        playerInput.UpdateAttackInput();
    }

    private void OnAttackPressed(eAttackType attackType)
    {
        if (!canAttack) return;

        // Debug.Log($"Player pressed attack: {attackType}");
        lastAttackType = attackType;
        AttackData attackData = attackConfig.GetAttackData(attackType);

        Enemy nearestEnemy = FindNearestEnemy(attackData.GetSnapRange());
        if (nearestEnemy != null)
        {
            attackRange.SetRange(attackData.GetSnapRange());
            attackRange.gameObject.SetActive(true);
            SnapToNearestEnemy(nearestEnemy);
            ExecuteAttack(attackData);
            return;
        }

        // If no nearest enemy, move forward by blank range and execute attack
        Vector3 blankDirection = (attackPoint.position - transform.position).normalized;
        transform.position += blankDirection * attackData.GetBlankRange();
        attackRange.SetRange(attackData.GetBlankRange());
        attackRange.gameObject.SetActive(true);
        ExecuteAttack(attackData);
    }

    private void SnapToNearestEnemy(Enemy nearestEnemy)
    {
        Transform contactPoint = nearestEnemy.GetContactPoint();
        if (contactPoint == null) return;

        Vector3 snapPosition = contactPoint.position;
        Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
        transform.position = snapPosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToEnemy);
    }

    private void ExecuteAttack(AttackData attackData)
    {
        Attack attackPrefab = attackData.attackPrefab;
        if (attackPrefab == null)
        {
            Debug.LogError($"No attack prefab found for attack type: {attackData.type}");
            return;
        }

        Vector2 attackOffset = Vector2.zero;
        if (attackData.type == eAttackType.Drill)
        {
            attackOffset = new Vector2(0f, attackData.GetHitboxSize().y / 2f);
        }

        Attack attack = Instantiate(attackPrefab, attackPoint.position, Quaternion.identity);
        attack.transform.rotation = transform.rotation;
        attack.Initialize(
            attackData.GetHitboxSize(),
            attackData.GetDamage(),
            attackOffset
        );
        globalAttackTimer.Start(globalAttackCooldown);
    }

    private Enemy FindNearestEnemy(float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(ENEMY_TAG)) continue;

            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private const int LOOK_UP = 0;
    private const int LOOK_TOP_RIGHT = 45;
    private const int LOOK_RIGHT = 90;
    private const int LOOK_BOTTOM_RIGHT = 135;
    private const int LOOK_DOWN = 180;
    private const int LOOK_BOTTOM_LEFT = 225;
    private const int LOOK_LEFT = 270;
    private const int LOOK_TOP_LEFT = 315;
    private eDirection lastDirection = eDirection.Up;
    private string idlePrefix = "idle_";
    private string lastIdleAnimation;
    private int lastAngle = 45;
    private void OnDirectionPressed(eDirection direction)
    {
        if (string.IsNullOrEmpty(lastIdleAnimation))
        {
            lastIdleAnimation = idlePrefix + LOOK_UP;
            lastAngle = LOOK_UP;
        }
        string animationName = idlePrefix + LOOK_UP;
        int angle = LOOK_UP;

        switch (direction)
        {
            case eDirection.Left:
                angle = LOOK_LEFT;
                break;
            case eDirection.Right:
                angle = LOOK_RIGHT;
                break;
            case eDirection.Up:
                angle = LOOK_UP;
                break;
            case eDirection.Down:
                angle = LOOK_DOWN;
                break;
            case eDirection.TopLeft:
                angle = LOOK_TOP_LEFT;
                break;
            case eDirection.TopRight:
                angle = LOOK_TOP_RIGHT;
                break;
            case eDirection.BottomLeft:
                angle = LOOK_BOTTOM_LEFT;
                break;
            case eDirection.BottomRight:
                angle = LOOK_BOTTOM_RIGHT;
                break;
            default:
                // Debug.LogWarning($"Unhandled direction input: {direction}");
                break;
        }
        lastDirection = direction;
        lastIdleAnimation = animationName;
        lastAngle = angle;
        animationName = idlePrefix + angle;
        Debug.Log($"Direction pressed: {direction}, angle: {angle}, animation: {animationName}");
        animator.Play(animationName);
    }

    public void OnTurnStart()
    {
        playerInput.EnableDirectionInput(true);
        playerInput.EnableAttackInput(true);
    }

    public void OnTurnEnd()
    {
        // playerInput.EnableDirectionInput(false);
        playerInput.EnableAttackInput(false);
        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.position = startLocation.position;
        transform.rotation = startLocation.rotation;
    }

    [Header("Rotation Settings")]
    [SerializeField] private float rotationRateHorizontal = 45f;
    [SerializeField] private float rotationRateVertical = 180f;
    private void RotateRight()
    {
        float currentRotationZ = transform.eulerAngles.z;
        float targetRotationZ = currentRotationZ - rotationRateHorizontal;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotationZ);
    }

    private void RotateLeft()
    {
        float currentRotationZ = transform.eulerAngles.z;
        float targetRotationZ = currentRotationZ + rotationRateHorizontal;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotationZ);
    }

    private void LookUp()
    {
        float currentRotationZ = transform.eulerAngles.z;
        float targetRotationZ = currentRotationZ + rotationRateVertical;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotationZ);
    }

    private void LookDown()
    {
        float currentRotationZ = transform.eulerAngles.z;
        float targetRotationZ = currentRotationZ - rotationRateVertical;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotationZ);
    }
}