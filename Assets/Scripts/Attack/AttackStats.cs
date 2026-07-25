using UnityEngine;

[CreateAssetMenu(fileName = "AttackStats", menuName = "Configs/Attack/AttackStats")]
public class AttackStats : ScriptableObject
{
    public int damage;
    public float snapRange;
    public float blankRange;
    public float cooldown;
}
