using System;

[Serializable]
public class AttackData
{
    public eAttackType type;
    public AttackStats stats;
    public Attack attackPrefab;
}

public enum eAttackType
{
    Standing,
    Dash,
    Swipe,
    Drill
}
