using System;

[Serializable]
public class AttackData
{
    public eAttackType type;
    public AttackStats stats;
}

public enum eAttackType
{
    Standing,
    Dash,
    Swipe,
    Drill
}
