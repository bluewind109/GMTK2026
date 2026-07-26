using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AttackConfig attackConfig;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform startLocation;
    [SerializeField] private Transform attackPointPivot;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float globalAttackCooldown = 0.1f;
    [SerializeField] private GameObject playerShadow;

    private const int LOOK_UP = 0;
    private const int LOOK_UP_RIGHT = -45;
    private const int LOOK_RIGHT = -90;
    private const int LOOK_DOWN_RIGHT = -135;
    private const int LOOK_DOWN = -180;
    private const int LOOK_DOWN_LEFT = -225;
    private const int LOOK_LEFT = -270;
    private const int LOOK_UP_LEFT = -315;

    public System.Action onAttackAnimationFinished;

    private PlayerInput playerInput;
    private AttackTimer globalAttackTimer;
    private AttackRange attackRange;
    private AttackAnimationBroadcaster[] attackAnimationBroadcasters;

    private eAttackType lastAttackType;

    private bool canAttack => globalAttackTimer.IsFinished();

    private const string ENEMY_TAG = "Enemy";

    private const string STANDING_ATTACK_PREFIX = "basic_attack_";
    private const string DASH_ATTACK_PREFIX = "dash_attack_";
    private const string SWIPE_ATTACK_PREFIX = "swipe_attack_";
    private const string DRILL_ATTACK_PREFIX = "drill_attack_";

    private bool isFirstAttackExecuted = false;

    private int[] viableAngles;

    void Start()
    {
        viableAngles = new int[] { LOOK_UP, LOOK_UP_RIGHT, LOOK_RIGHT, LOOK_DOWN_RIGHT, LOOK_DOWN, LOOK_DOWN_LEFT, LOOK_LEFT, LOOK_UP_LEFT };

        globalAttackTimer = new AttackTimer(globalAttackCooldown);

        playerInput = GetComponentInChildren<PlayerInput>();
        playerInput.onAttackPressed += OnAttackPressed;
        playerInput.onDirectionPressed += OnDirectionPressed;

        attackRange = GetComponentInChildren<AttackRange>();
        attackRange.gameObject.SetActive(false);

        attackAnimationBroadcasters = animator.GetBehaviours<AttackAnimationBroadcaster>();
        foreach (var broadcaster in attackAnimationBroadcasters)
            broadcaster.onAttackAnimationFinished += OnAttackAnimationFinished;

        playerShadow.SetActive(false);
    }

    void OnDestroy()
    {
        playerInput.onAttackPressed -= OnAttackPressed;
        playerInput.onDirectionPressed -= OnDirectionPressed;

        foreach (var broadcaster in attackAnimationBroadcasters)
            broadcaster.onAttackAnimationFinished -= OnAttackAnimationFinished;
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

            Vector3 directionToEnemy = (nearestEnemy.transform.position - attackPointPivot.transform.position).normalized;
            float angleToEnemy = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
            int nearestAngle = FindNearestAngle(angleToEnemy);
            eDirection nearestDirection = MapAngleToDirection(nearestAngle);
            TurnAttackPointAt(nearestDirection);
            SnapToNearestEnemy(nearestEnemy);
            ExecuteAttack(attackData, nearestAngle);
            return;
        }

        // If no nearest enemy, move forward by blank range and execute attack
        Vector3 blankDirection = (attackPoint.position - transform.position).normalized;
        transform.position += blankDirection * attackData.GetBlankRange();
        attackRange.SetRange(attackData.GetBlankRange());
        attackRange.gameObject.SetActive(true);
        ExecuteAttack(attackData, lastAngle);
    }

    private void SnapToNearestEnemy(Enemy nearestEnemy)
    {
        Transform contactPoint = nearestEnemy.GetContactPoint();
        if (contactPoint == null) return;

        Vector3 snapPosition = contactPoint.position;
        transform.position = snapPosition;

    }

    private void ExecuteAttack(AttackData attackData, int nearestAngle)
    {
        Attack attackPrefab = attackData.attackPrefab;
        if (attackPrefab == null)
        {
            Debug.LogError($"No attack prefab found for attack type: {attackData.type}");
            return;
        }

        if (!isFirstAttackExecuted)
        {
            isFirstAttackExecuted = true;
            playerShadow.SetActive(true);
        }

        eAttackType attackType = attackData.type;
        switch (attackType)
        {
            case eAttackType.Standing:
                animator.Play(STANDING_ATTACK_PREFIX + Math.Abs(nearestAngle));
                break;
            case eAttackType.Dash:
                animator.Play(DASH_ATTACK_PREFIX + Math.Abs(nearestAngle));
                break;
            case eAttackType.Swipe:
                animator.Play(SWIPE_ATTACK_PREFIX + Math.Abs(nearestAngle));
                break;
            case eAttackType.Drill:
                animator.Play(DRILL_ATTACK_PREFIX + Math.Abs(nearestAngle));
                break;
            default:
                Debug.LogWarning($"Unhandled attack type: {attackType}");
                break;
        }

        Vector2 attackOffset = Vector2.zero;
        if (attackData.type == eAttackType.Drill)
        {
            attackOffset = new Vector2(0f, attackData.GetHitboxSize().y / 2f);
        }

        Attack attack = Instantiate(attackPrefab, attackPoint.position, Quaternion.identity);
        attack.transform.rotation = attackPointPivot.transform.rotation;
        attack.Initialize(
            attackData.GetHitboxSize(),
            attackData.GetDamage(),
            attackOffset
        );
        globalAttackTimer.Start(globalAttackCooldown);
    }

    private void OnAttackAnimationFinished()
    {
        attackRange.gameObject.SetActive(false);
        onAttackAnimationFinished?.Invoke();
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


    private eDirection lastDirection = eDirection.Up;
    private string idlePrefix = "idle_";
    private string lastIdleAnimation;
    private int lastAngle = 0;
    private void OnDirectionPressed(eDirection direction)
    {
        if (string.IsNullOrEmpty(lastIdleAnimation))
        {
            lastIdleAnimation = idlePrefix + Math.Abs(LOOK_UP);
            lastAngle = LOOK_UP;
        }
        string animationName = idlePrefix + Math.Abs(LOOK_UP);
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
            case eDirection.UpLeft:
                angle = LOOK_UP_LEFT;
                break;
            case eDirection.UpRight:
                angle = LOOK_UP_RIGHT;
                break;
            case eDirection.DownLeft:
                angle = LOOK_DOWN_LEFT;
                break;
            case eDirection.DownRight:
                angle = LOOK_DOWN_RIGHT;
                break;
            default:
                // Debug.LogWarning($"Unhandled direction input: {direction}");
                break;
        }
        lastDirection = direction;
        lastIdleAnimation = animationName;
        lastAngle = angle;
        animationName = idlePrefix + Math.Abs(angle);
        TurnAttackPointAt(direction);
        Debug.Log($"Direction pressed: {direction}, angle: {angle}, animation: {animationName}");
        animator.Play(animationName);
    }

    private int FindNearestAngle(float targetAngle)
    {
        // Convert Atan2 angle (0 = right, 90 = up) to this project's pivot convention
        // (0 = up, -90 = right, -180 = down, -270 = left).
        float normalizedTargetAngle = targetAngle - 90f;
        if (normalizedTargetAngle > 0f)
        {
            normalizedTargetAngle -= 360f;
        }

        int nearestAngle = viableAngles[0];
        float smallestDifference = Mathf.Abs(Mathf.DeltaAngle(normalizedTargetAngle, nearestAngle));

        foreach (int angle in viableAngles)
        {
            float difference = Mathf.Abs(Mathf.DeltaAngle(normalizedTargetAngle, angle));
            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                nearestAngle = angle;
            }
        }

        return nearestAngle;
    }

    public void OnTurnStart()
    {
        playerInput?.EnableDirectionInput(true);
        playerInput?.EnableAttackInput(true);
    }

    public void OnTurnEnd()
    {
        // playerInput?.EnableDirectionInput(false);
        playerInput?.EnableAttackInput(true);
        ResetPosition();
        playerShadow.SetActive(false);
        isFirstAttackExecuted = false;
    }

    private void ResetPosition()
    {
        transform.position = startLocation.position;
        transform.rotation = startLocation.rotation;
    }

    [Header("Rotation Settings")]
    [SerializeField] private float rotationRateHorizontal = 45f;
    [SerializeField] private float rotationRateVertical = 180f;
    private void TurnAttackPointAt(eDirection direction)
    {
        float targetRotationZ = LOOK_RIGHT;
        switch (direction)
        {
            case eDirection.Left:
                targetRotationZ = LOOK_LEFT;
                break;
            case eDirection.Right:
                targetRotationZ = LOOK_RIGHT;
                break;
            case eDirection.Up:
                targetRotationZ = LOOK_UP; ;
                break;
            case eDirection.Down:
                targetRotationZ = LOOK_DOWN;
                break;
            case eDirection.UpLeft:
                targetRotationZ = LOOK_UP_LEFT;
                break;
            case eDirection.UpRight:
                targetRotationZ = LOOK_UP_RIGHT;
                break;
            case eDirection.DownLeft:
                targetRotationZ = LOOK_DOWN_LEFT;
                break;
            case eDirection.DownRight:
                targetRotationZ = LOOK_DOWN_RIGHT;
                break;
            default:
                Debug.LogWarning($"Unhandled direction input: {direction}");
                break;
        }
        attackPointPivot.rotation = Quaternion.Euler(0f, 0f, targetRotationZ);
    }

    private eDirection MapAngleToDirection(int angle)
    {
        switch (angle)
        {
            case LOOK_LEFT:
                return eDirection.Left;
            case LOOK_RIGHT:
                return eDirection.Right;
            case LOOK_UP:
                return eDirection.Up;
            case LOOK_DOWN:
                return eDirection.Down;
            case LOOK_UP_LEFT:
                return eDirection.UpLeft;
            case LOOK_UP_RIGHT:
                return eDirection.UpRight;
            case LOOK_DOWN_LEFT:
                return eDirection.DownLeft;
            case LOOK_DOWN_RIGHT:
                return eDirection.DownRight;
            default:
                Debug.LogWarning($"Unhandled angle: {angle}");
                return lastDirection; // Return the last known direction if angle is unhandled
        }
    }

}