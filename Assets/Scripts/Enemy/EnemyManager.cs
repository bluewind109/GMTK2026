using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

public class EnemyManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private EnemyConfig enemyConfig;

    [Header("Enemy")]
    [SerializeField] private Transform enemiesContainer;
    [SerializeField] private Transform targetPosition;
    private List<Enemy> activeEnemies = new List<Enemy>();

    [Header("Spawn Locations")]
    [SerializeField] private SpawnLocation spawnLocation_Left;
    [SerializeField] private SpawnLocation spawnLocation_Right;
    [SerializeField] private SpawnLocation spawnLocation_Center;
    private List<SpawnLocation> spawnLocations = new List<SpawnLocation>();

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

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

    public void StartLevel(LevelInfo levelInfo)
    {
        spawnLocation_Left.Initialize(levelInfo.leftSpawn, levelInfo.spawnInterval);
        spawnLocation_Right.Initialize(levelInfo.rightSpawn, levelInfo.spawnInterval);
        spawnLocation_Center.Initialize(levelInfo.centerSpawn, levelInfo.spawnInterval);
    }

    private void SpawnEnemy(EnemyType enemyType, Vector3 spawnPosition)
    {
        _ = SpawnEnemyAsync(enemyType, spawnPosition);
    }

    private async UniTask SpawnEnemyAsync(EnemyType enemyType, Vector3 spawnPosition)
    {
        EnemyInfo enemyInfo = enemyConfig.GetEnemyInfo(enemyType);
        Enemy instance = Instantiate(
            enemyInfo.enemyPrefab,
            spawnPosition,
            Quaternion.identity,
            enemiesContainer
        );

        await UniTask.WaitForEndOfFrame(this); // Wait for one frame to ensure all components are initialized
        instance.Initialize(enemyInfo, targetPosition.position);
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

    public void UpdateEnemyMovements(float speedMultiplier)
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy.IsActive())
            {
                enemy.MoveTowardsTarget(speedMultiplier);
            }
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        enemy.onDeath -= OnEnemyDeath;
        Destroy(enemy.gameObject, 1f);
    }
}
