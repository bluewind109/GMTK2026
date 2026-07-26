using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfig", menuName = "Configs/Attack/AttackConfig", order = 0)]
public class AttackConfig : ScriptableObject
{
    [SerializeField] private List<AttackData> attackDatas = new List<AttackData>();

    public AttackData GetAttackData(eAttackType attackType)
    {
        return attackDatas.Find(data => data.type == attackType);
    }
}
