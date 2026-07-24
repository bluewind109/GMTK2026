using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/Level/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<LevelInfo> levelInfos;

    public LevelInfo GetLevelInfo(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelInfos.Count)
        {
            Debug.LogError($"Invalid level index: {levelIndex}. Returning default LevelInfo.");
            return new LevelInfo();
        }
        return levelInfos[levelIndex];
    }
}

[Serializable]
public class SpawnAreaInfo
{
    public int spawnAmount = 7;
    public List<SpawnWeight> spawnWeights;

    private float GetTotalWeight()
    {
        float totalWeight = 0f;
        foreach (var spawnWeight in spawnWeights)
        {
            totalWeight += spawnWeight.weight;
        }
        return totalWeight;
    }

    public EnemyType GetRandomEnemyType()
    {
        float totalWeight = GetTotalWeight();
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var spawnWeight in spawnWeights)
        {
            cumulativeWeight += spawnWeight.weight;
            if (randomValue <= cumulativeWeight)
            {
                return spawnWeight.enemyType;
            }
        }

        return spawnWeights[spawnWeights.Count - 1].enemyType;
    }
}

[Serializable]
public class SpawnWeight
{
    public EnemyType enemyType;
    public float weight = 10f;
}

public enum EnemyType
{
    TypeA,
    TypeB,
    TypeC
}