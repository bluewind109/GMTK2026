using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AttackConfig attackConfig;
    [SerializeField] private Transform startLocation;
    [SerializeField] private float globalAttackCooldown = 0.1f;

    private PlayerInput playerInput;
    private AttackTimer globalAttackTimer;
    private AttackRange attackRange;
    
    private eAttackType lastAttackType;

    private bool canAttack => globalAttackTimer.IsFinished();

    void Start()
    {
        globalAttackTimer = new AttackTimer(globalAttackCooldown);

        playerInput = GetComponentInChildren<PlayerInput>();
        playerInput.onAttackPressed += OnAttackPressed;
    }

    void OnDestroy()
    {
        playerInput.onAttackPressed -= OnAttackPressed;
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
        Attack attackPrefab = attackData.attackPrefab;
        if (attackPrefab == null)
        {
            Debug.LogError($"No attack prefab found for attack type: {attackType}");
            return;
        }

        Instantiate(attackPrefab, startLocation.position, Quaternion.identity);
        globalAttackTimer.Start(globalAttackCooldown);
    }

    private void FindNearestEnemy()
    {
        
    }
}
