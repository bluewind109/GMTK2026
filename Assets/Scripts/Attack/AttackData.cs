using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public eAttackType type;
    public AttackStats stats;
    public Attack attackPrefab;

    public int GetDamage()
    {
        return stats.damage;
    }

    public float GetSnapRange()
    {
        return stats.snapRange;
    }

    public float GetBlankRange()
    {
        return stats.blankRange;
    }

    public float GetCooldown()
    {
        return stats.cooldown;
    }

    public Vector2 GetHitboxSize()
    {
        return stats.hitboxSize;
    }
}

public enum eAttackType
{
    Standing,
    Dash,
    Swipe,
    Drill
}
