using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AttackConfig attackConfig;
    [SerializeField] private Transform startLocation;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float globalAttackCooldown = 0.1f;

    private PlayerInput playerInput;
    private AttackTimer globalAttackTimer;
    private AttackRange attackRange;

    private eAttackType lastAttackType;

    private bool canAttack => globalAttackTimer.IsFinished();

    private List<Enemy> enemiesInRange = new List<Enemy>();

    void Start()
    {
        globalAttackTimer = new AttackTimer(globalAttackCooldown);

        playerInput = GetComponentInChildren<PlayerInput>();
        playerInput.onAttackPressed += OnAttackPressed;

        attackRange = GetComponentInChildren<AttackRange>();
        attackRange.onEnemyInRange += OnEnemyInRange;
        attackRange.onEnemyOutOfRange += OnEnemyOutOfRange;

        attackRange.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        playerInput.onAttackPressed -= OnAttackPressed;
        attackRange.onEnemyInRange -= OnEnemyInRange;
        attackRange.onEnemyOutOfRange -= OnEnemyOutOfRange;
    }

    void Update()
    {
        globalAttackTimer.Update(Time.deltaTime);
        playerInput.UpdateInput();
    }

    private void OnAttackPressed(eAttackType attackType)
    {
        if (!canAttack) return;

        Debug.Log($"Player pressed attack: {attackType}");
        lastAttackType = attackType;
        AttackData attackData = attackConfig.GetAttackData(attackType);

        Enemy nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            attackRange.SetRange(attackData.GetSnapRange());
            SnapToNearestEnemy(nearestEnemy);
            ExecuteAttack(attackData);
            return;
        }

        // If no nearest enemy, still execute attack with blank range
        ExecuteAttack(attackData);
    }

    private void SnapToNearestEnemy(Enemy nearestEnemy)
    {
        Transform contactPoint = nearestEnemy.GetContactPoint();
        if (contactPoint == null) return;

        Vector3 snapPosition = contactPoint.position;
        transform.position = snapPosition;
    }

    private void ExecuteAttack(AttackData attackData)
    {
        Attack attackPrefab = attackData.attackPrefab;
        if (attackPrefab == null)
        {
            Debug.LogError($"No attack prefab found for attack type: {attackData.type}");
            return;
        }

        Attack attack = Instantiate(attackPrefab, attackPoint.position, Quaternion.identity);
        attack.Initialize(
            attackData.GetHitboxSize(), 
            attackData.GetDamage(), 
            attackData.GetCooldown()
        );
        globalAttackTimer.Start(globalAttackCooldown);
    }

    private Enemy FindNearestEnemy()
    {
        if (enemiesInRange.Count == 0)
        {
            Debug.Log("No enemies in range.");
            // TODO still execute attack but using blank range
            return null;
        }

        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
            Debug.Log($"Nearest enemy found: {nearestEnemy.name} at distance {nearestDistance}");
        return nearestEnemy;
    }

    private void OnEnemyInRange(Enemy enemy)
    {
        if (!enemiesInRange.Contains(enemy))
            enemiesInRange.Add(enemy);
    }

    private void OnEnemyOutOfRange(Enemy enemy)
    {
        if (enemiesInRange.Contains(enemy))
            enemiesInRange.Remove(enemy);
    }
}
