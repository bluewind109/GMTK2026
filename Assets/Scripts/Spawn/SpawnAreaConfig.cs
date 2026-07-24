using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "SpawnAreaConfig", menuName = "Configs/SpawnAreaConfig")]
public class SpawnAreaConfig : ScriptableObject
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
public class LevelInfo
{
    public int spawnAmount;
    public float spawnInterval = 0.5f;
    public float turnDuration = 15f;
    public int numberOfTurns = 3;

    public List<SpawnWeight> spawnWeights;
}

[Serializable]
public class SpawnWeight
{
    public EnemyType enemyType;
    public float weight;
}

public enum EnemyType
{
    TypeA,
    TypeB,
    TypeC
}