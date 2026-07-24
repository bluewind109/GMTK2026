using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] private List<EnemyInfo> enemyInfos;

    public EnemyInfo GetEnemyInfo(EnemyType enemyType)
    {
        foreach (var enemyInfo in enemyInfos)
        {
            if (enemyInfo.enemyType == enemyType)
            {
                return enemyInfo;
            }
        }
        Debug.LogError($"Enemy type {enemyType} not found in EnemyConfig.");
        return null;
    }
}

[Serializable]
public class EnemyInfo
{
    public EnemyType enemyType;
    public int health = 3;
    public float moveSpeed = 2f;
    public int damage = 1;
}
