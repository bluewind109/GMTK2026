using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private EnemyConfig enemyConfig;
    [SerializeField] private LevelConfig levelConfig;

    [Header("Enemy")]
    [SerializeField] private Transform enemiesContainer;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform targetPosition;
    private List<Enemy> activeEnemies = new List<Enemy>();

    [Header("Spawn Locations")]
    [SerializeField] private SpawnLocation spawnLocation_Left;
    [SerializeField] private SpawnLocation spawnLocation_Right;
    [SerializeField] private SpawnLocation spawnLocation_Center;
    private List<SpawnLocation> spawnLocations = new List<SpawnLocation>();

    void Awake()
    {
        spawnLocations = new List<SpawnLocation>
        {
            spawnLocation_Left,
            spawnLocation_Right,
            spawnLocation_Center
        };
    }

    void Start()
    {
        spawnLocation_Left.onIntervalReached += SpawnEnemy;
        spawnLocation_Right.onIntervalReached += SpawnEnemy;
        spawnLocation_Center.onIntervalReached += SpawnEnemy;
    }

    void OnDestroy()
    {
        spawnLocation_Left.onIntervalReached -= SpawnEnemy;
        spawnLocation_Right.onIntervalReached -= SpawnEnemy;
        spawnLocation_Center.onIntervalReached -= SpawnEnemy;
    }

    public void Test_StartLevel1()
    {
        LevelInfo levelInfo = levelConfig.GetLevelInfo(0);
        spawnLocation_Left.Initialize(levelInfo.spawnInterval, levelInfo.leftSpawn.spawnAmount);
        spawnLocation_Right.Initialize(levelInfo.spawnInterval, levelInfo.rightSpawn.spawnAmount);
        spawnLocation_Center.Initialize(levelInfo.spawnInterval, levelInfo.centerSpawn.spawnAmount);
    }

    private void SpawnEnemy(Vector3 spawnPosition)
    {
        Enemy instance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemiesContainer);
        EnemyType enemyType = EnemyType.TypeA;
        EnemyInfo enemyInfo = enemyConfig.GetEnemyInfo(enemyType);
        instance.Initialize(enemyInfo, spawnPosition, targetPosition.position);
        instance.onDeath += OnEnemyDeath;
        activeEnemies.Add(instance);
    }

    public void UpdateSpawnLocations()
    {
        foreach (var spawnLocation in spawnLocations)
        {
            spawnLocation.UpdateSpawnInterval();
        }
    }

    public void UpdateEnemyMovements()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy.IsActive())
            {
                enemy.MoveTowardsTarget();
            }
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        enemy.onDeath -= OnEnemyDeath;
        Destroy(enemy.gameObject);
    }
}
