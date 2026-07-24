using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] private int health = 3;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRange = 1f;

    public int GetHealth() => health;
    public float GetMoveSpeed() => moveSpeed;
    public int GetDamage() => damage;
    public float GetAttackRange() => attackRange;
}
